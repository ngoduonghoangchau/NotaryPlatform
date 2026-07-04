using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using NotaryPlatform.Application.Abstractions.Storage;

namespace NotaryPlatform.Infrastructure.Services.Files;

/// <summary>
/// Cloudinary implementation of <see cref="IFileStorageService"/>.
///
/// SECURITY — files are uploaded as PRIVATE (delivery type "authenticated") because this
/// platform stores sensitive legal documents (ID scans, commissions, journal signatures).
/// A plain/public Cloudinary URL therefore never grants access; downloads are served only via
/// short-lived, cryptographically SIGNED URLs produced by <see cref="GetDownloadUrlAsync"/>.
///
/// StoragePath == the Cloudinary <c>public_id</c> (tenant-namespaced by <see cref="FileNameGenerator"/>).
/// It is what gets persisted in the database and later used to download or delete the file.
///
/// LEARNING — resource_type = "raw":
///   Uploaded docs are mixed types (PDF, images, docx). Storing everything as "raw" keeps the
///   original bytes intact (no image processing) and gives one uniform code path.
/// </summary>
public sealed class CloudinaryFileStorageService : IFileStorageService
{
    // "authenticated" = private delivery; the URL must be signed to be usable.
    private const string DeliveryType = "authenticated";
    private const string RawResourceType = "raw";
    private static readonly TimeSpan DefaultUrlLifetime = TimeSpan.FromMinutes(15);

    private readonly Cloudinary _cloudinary;

    public CloudinaryFileStorageService(Cloudinary cloudinary) => _cloudinary = cloudinary;

    public async Task<FileUploadResult> UploadAsync(
        Stream stream,
        string fileName,
        string folder,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var storagePath = FileNameGenerator.Generate(fileName, folder, tenantId);
        var contentType = FileNameGenerator.GetContentType(fileName);

        var uploadParams = new RawUploadParams
        {
            File = new FileDescription(fileName, stream),
            PublicId = storagePath,
            Type = DeliveryType,        // private
            Overwrite = true,
            UseFilename = false,
            UniqueFilename = false,
        };

        var result = await _cloudinary.UploadAsync(uploadParams, RawResourceType, cancellationToken);
        if (result.Error is not null)
            throw new InvalidOperationException($"Cloudinary upload failed: {result.Error.Message}");

        return new FileUploadResult
        {
            StoragePath = storagePath,
            FileName = Path.GetFileName(storagePath),
            ContentType = contentType,
            SizeBytes = result.Bytes,
        };
    }

    public Task<string> GetDownloadUrlAsync(
        string storagePath,
        TimeSpan? expiresIn = null,
        CancellationToken cancellationToken = default)
    {
        // Signed, time-limited URL to fetch the original file. It cannot be forged (requires the
        // API secret) and stops working after expiresAt — the Cloudinary equivalent of a GCS/S3
        // pre-signed URL.
        var expiresAt = DateTimeOffset.UtcNow.Add(expiresIn ?? DefaultUrlLifetime).ToUnixTimeSeconds();

        var url = _cloudinary.DownloadPrivate(
            publicId: storagePath,
            attachment: false,
            format: string.Empty,
            type: DeliveryType,
            expiresAt: expiresAt,
            resourceType: RawResourceType);

        return Task.FromResult(url);
    }

    public async Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        var result = await _cloudinary.DestroyAsync(new DeletionParams(storagePath)
        {
            ResourceType = ResourceType.Raw,
            Type = DeliveryType,
        });

        // "not found" means it was already deleted — treat as success in our domain.
        if (result.Error is not null &&
            !string.Equals(result.Result, "not found", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Cloudinary delete failed: {result.Error.Message}");
        }
    }
}
