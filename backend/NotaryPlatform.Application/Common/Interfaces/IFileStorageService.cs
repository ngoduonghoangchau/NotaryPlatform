namespace NotaryPlatform.Application.Common.Interfaces;

/// <summary>
/// Abstracts cloud / local file storage. Implemented by
/// Infrastructure.Services.Files.CloudFileStorageService (GCS/S3)
/// and LocalFileStorageService (dev).
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Uploads a file stream and returns its permanent storage URL.
    /// </summary>
    /// <param name="stream">File byte stream.</param>
    /// <param name="fileName">Original file name (used to derive content type).</param>
    /// <param name="folder">Logical storage folder/prefix (e.g., "notary-docs", "invoices").</param>
    /// <param name="tenantId">Used to namespace storage paths — prevents cross-tenant access.</param>
    Task<FileUploadResult> UploadAsync(
        Stream stream,
        string fileName,
        string folder,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a short-lived pre-signed URL allowing a client to download the file directly.
    /// The URL expires after <paramref name="expiresIn"/> (default: 15 minutes).
    /// </summary>
    Task<string> GetDownloadUrlAsync(
        string storagePath,
        TimeSpan? expiresIn = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Permanently deletes a file from storage.
    /// </summary>
    Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default);
}

public sealed record FileUploadResult
{
    public required string StoragePath { get; init; }
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
    public required long SizeBytes { get; init; }
}
