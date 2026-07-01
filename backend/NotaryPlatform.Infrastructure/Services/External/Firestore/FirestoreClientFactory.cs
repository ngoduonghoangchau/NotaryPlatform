using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NotaryPlatform.Infrastructure.Services.External.Firestore;

/// <summary>
/// Singleton factory that creates and caches one FirestoreDb instance for the lifetime
/// of the application.
///
/// LEARNING — Why Singleton?
///   FirestoreDb holds an underlying gRPC channel and connection pool.
///   Creating it per-request would be as wasteful as creating a new HttpClient
///   per-request. One channel is reused across all requests (it is thread-safe).
///   We use Lazy{FirestoreDb} for thread-safe, on-demand initialization — the
///   gRPC channel is only opened when the first repository makes a Firestore call.
///
/// LEARNING — Application Default Credentials (ADC) priority:
///   1. GOOGLE_APPLICATION_CREDENTIALS env var → path to service-account JSON file.
///   2. gcloud CLI credentials → for local dev after `gcloud auth application-default login`.
///   3. GCP Metadata Server → when running on GCE / Cloud Run / GKE (Workload Identity).
///
/// LEARNING — Firestore emulator:
///   Set FIRESTORE_EMULATOR_HOST=localhost:8080 and the SDK automatically routes
///   all requests to the emulator. No code changes needed.
/// </summary>
public sealed class FirestoreClientFactory
{
    private readonly Lazy<FirestoreDb> _db;
    private readonly ILogger<FirestoreClientFactory> _logger;

    public FirestoreClientFactory(
        IOptions<FirestoreSettings> options,
        ILogger<FirestoreClientFactory> logger)
    {
        _logger = logger;
        var settings = options.Value;
        _db = new Lazy<FirestoreDb>(
            () => CreateDb(settings),
            LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public FirestoreDb GetDatabase() => _db.Value;

    private FirestoreDb CreateDb(FirestoreSettings settings)
    {
        _logger.LogInformation(
            "Initializing FirestoreDb for project '{ProjectId}'", settings.ProjectId);

        // Credentials resolved automatically via ADC — no explicit credential setup needed.
        return new FirestoreDbBuilder { ProjectId = settings.ProjectId }.Build();
    }
}
