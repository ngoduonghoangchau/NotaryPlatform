using System.Net.Http.Headers;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;
using NotaryPlatform.Application.Abstractions.Storage;

namespace NotaryPlatform.Infrastructure.Services.Files;

/// <summary>
/// Firebase Storage (Google Cloud Storage) implementation of IFileStorageService.
/// Implements IFileStorageService (Application) — callers never reference this class directly.
///
/// LEARNING — Why inject GoogleCredential instead of accessing FirebaseApp.DefaultInstance?
///   FirebaseApp.DefaultInstance is a static singleton — a hidden dependency that:
///   • Cannot be replaced in unit tests without process-level side effects.
///   • Makes the class harder to reason about (dependency not visible in constructor).
///   Injecting GoogleCredential follows DIP: the class declares what it needs
///   explicitly; the composition root (DependencyInjection.cs) wires it up.
///
/// LEARNING — Firebase Storage IS Google Cloud Storage:
///   Firebase Storage is GCS with Firebase Security Rules layered on top.
///   We bypass Firebase client SDKs and call the GCS JSON API directly,
///   authenticating with the same service-account credential as the rest of Firebase.
///
/// LEARNING — GCS JSON API endpoints used:
///   Upload  : POST /upload/storage/v1/b/{bucket}/o?uploadType=media&amp;name={encodedPath}
///   Download: GET  /storage/v1/b/{bucket}/o/{encodedPath}?alt=media  (+ Bearer token)
///   Delete  : DELETE /storage/v1/b/{bucket}/o/{encodedPath}
/// </summary>
public sealed class CloudFileStorageService : IFileStorageService
{
    private const string GcsUploadBase = "https://storage.googleapis.com/upload/storage/v1/b";
    private const string GcsApiBase = "https://storage.googleapis.com/storage/v1/b";

    private readonly HttpClient _httpClient;
    private readonly FirebaseStorageSettings _settings;
    private readonly GoogleCredential _credential;

    public CloudFileStorageService(
        IHttpClientFactory httpClientFactory,
        GoogleCredential credential,
        IOptions<FirebaseStorageSettings> options)
    {
        _httpClient = httpClientFactory.CreateClient(nameof(CloudFileStorageService));
        _credential = credential;
        _settings = options.Value;
    }

    public async Task<FileUploadResult> UploadAsync(
        Stream stream,
        string fileName,
        string folder,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var storagePath = FileNameGenerator.Generate(fileName, folder, tenantId);
        var contentType = FileNameGenerator.GetContentType(fileName);
        var encodedPath = Uri.EscapeDataString(storagePath);
        var uploadUrl = $"{GcsUploadBase}/{_settings.BucketName}/o?uploadType=media&name={encodedPath}";

        var request = new HttpRequestMessage(HttpMethod.Post, uploadUrl);
        await SetAuthHeaderAsync(request, cancellationToken);
        request.Content = new StreamContent(stream);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return new FileUploadResult
        {
            StoragePath = storagePath,
            FileName = Path.GetFileName(storagePath),
            ContentType = contentType,
            SizeBytes = stream.Length,
        };
    }

    public async Task<string> GetDownloadUrlAsync(
        string storagePath,
        TimeSpan? expiresIn = null,
        CancellationToken cancellationToken = default)
    {
        // Returns an authenticated download URL valid for the duration of the Bearer token (~1 h).
        // For long-lived client-accessible URLs, implement GCS V4 signed URLs instead.
        var encodedPath = Uri.EscapeDataString(storagePath);
        var token = await GetAccessTokenAsync(cancellationToken);

        return $"{GcsApiBase}/{_settings.BucketName}/o/{encodedPath}?alt=media&access_token={token}";
    }

    public async Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        var encodedPath = Uri.EscapeDataString(storagePath);
        var request = new HttpRequestMessage(
            HttpMethod.Delete,
            $"{GcsApiBase}/{_settings.BucketName}/o/{encodedPath}");

        await SetAuthHeaderAsync(request, cancellationToken);
        var response = await _httpClient.SendAsync(request, cancellationToken);

        // 404 means the object was already deleted — treat as success in our domain.
        if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
            response.EnsureSuccessStatusCode();
    }

    private async Task SetAuthHeaderAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var token = await GetAccessTokenAsync(ct);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private Task<string> GetAccessTokenAsync(CancellationToken ct) =>
        _credential.UnderlyingCredential.GetAccessTokenForRequestAsync(cancellationToken: ct);
}
