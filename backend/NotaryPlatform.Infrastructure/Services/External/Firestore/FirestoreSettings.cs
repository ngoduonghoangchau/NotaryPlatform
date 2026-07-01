namespace NotaryPlatform.Infrastructure.Services.External.Firestore;

/// <summary>Firestore configuration — bound from appsettings.json → "Firestore" section.</summary>
public sealed class FirestoreSettings
{
    public const string SectionName = "Firestore";

    /// <summary>Google Cloud project ID that owns the Firestore database.</summary>
    public required string ProjectId { get; init; }

    // Credentials are resolved via Application Default Credentials (ADC) — no file path needed.
    // Dev  : set GOOGLE_APPLICATION_CREDENTIALS env var to a service-account JSON path,
    //        or run `gcloud auth application-default login`.
    // GCP  : Workload Identity (Cloud Run / GKE) or instance service account (GCE).
}
