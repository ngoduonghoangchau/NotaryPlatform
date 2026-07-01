using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using NotaryPlatform.Application.Common.Interfaces;

namespace NotaryPlatform.Infrastructure.Services.Files;

/// <summary>
/// Local filesystem implementation of IFileStorageService for development.
/// Files are stored under wwwroot/{UploadPath} and served as static files by UseStaticFiles().
/// Implements IFileStorageService (Application) — swap for CloudFileStorageService in production.
///
/// LEARNING — Why a dev-only storage implementation?
///   CloudFileStorageService requires GCP credentials and network access.
///   LocalFileStorageService lets developers work offline without any cloud setup.
///   Because both implement IFileStorageService, switching to cloud for production
///   requires only a DI registration change — zero application code changes.
///   This is the Open/Closed Principle and DIP applied together.
///
/// LEARNING — Static file serving:
///   Files under wwwroot/ are served by UseStaticFiles() at the root URL path.
///   wwwroot/uploads/tenant/docs/2025/06/abc_file.pdf
///   → https://localhost:5001/uploads/tenant/docs/2025/06/abc_file.pdf
/// </summary>
public sealed class LocalFileStorageService : IFileStorageService
{
    private readonly string _rootPath;
    private readonly string _baseUrl;

    public LocalFileStorageService(IWebHostEnvironment env, IOptions<LocalStorageSettings> options)
    {
        var settings = options.Value;
        _rootPath    = Path.Combine(env.WebRootPath, settings.UploadPath);
        _baseUrl     = settings.BaseUrl.TrimEnd('/') + "/" + settings.UploadPath;

        Directory.CreateDirectory(_rootPath);
    }

    public async Task<FileUploadResult> UploadAsync(
        Stream stream,
        string fileName,
        string folder,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var storagePath  = FileNameGenerator.Generate(fileName, folder, tenantId);
        var absolutePath = ToAbsolutePath(storagePath);
        var contentType  = FileNameGenerator.GetContentType(fileName);

        Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);

        await using var dest = File.Create(absolutePath);
        await stream.CopyToAsync(dest, cancellationToken);

        return new FileUploadResult
        {
            StoragePath = storagePath,
            FileName    = Path.GetFileName(absolutePath),
            ContentType = contentType,
            SizeBytes   = dest.Length,
        };
    }

    public Task<string> GetDownloadUrlAsync(
        string storagePath,
        TimeSpan? expiresIn = null,
        CancellationToken cancellationToken = default)
    {
        // Local files don't expire — expiresIn is meaningful only for cloud signed URLs.
        var url = $"{_baseUrl}/{storagePath.Replace('\\', '/')}";
        return Task.FromResult(url);
    }

    public Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        var absolutePath = ToAbsolutePath(storagePath);

        if (File.Exists(absolutePath))
            File.Delete(absolutePath);

        return Task.CompletedTask;
    }

    private string ToAbsolutePath(string storagePath) =>
        Path.Combine(_rootPath, storagePath.Replace('/', Path.DirectorySeparatorChar));
}
