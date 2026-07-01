using Google.Cloud.Firestore;

namespace NotaryPlatform.Infrastructure.Services.External.Firestore;

/// <summary>
/// Abstract base class providing generic CRUD operations for Firestore-backed repositories.
///
/// LEARNING — When to use Firestore (vs PostgreSQL in this project)?
///   PostgreSQL  : Primary datastore. Relational, transactional, schema-enforced.
///                 All core business data (users, roles, documents, etc.) lives here.
///   Firestore   : Real-time, document-oriented datastore. Use for:
///                 • Live collaboration data (e.g. users currently editing a document)
///                 • Real-time notifications / event feeds
///                 • Denormalized read models / materialized views for the React UI
///                 • User presence / typing indicators
///                 • Device FCM token storage and topic subscriptions
///
/// LEARNING — Firestore data model:
///   Collection → Document → optional Subcollection (recursive)
///   Each document is a JSON-like map of fields. Documents in the same collection
///   can have different shapes (unlike relational tables).
///   Access path: {collection}/{documentId}/{subcollection}/{docId}/...
///
/// LEARNING — How to annotate a POCO for Firestore:
///   [FirestoreData]           — marks the class as a Firestore document
///   [FirestoreProperty("id")] — maps a property to a Firestore field name
///   [FirestoreDocumentId]     — the document ID (a string, set by Firestore)
///   The SDK serializes/deserializes using these attributes automatically.
///
/// LEARNING — Firestore transactions vs batch writes:
///   Transaction : Read + conditional write. Retried automatically on contention.
///                 Use when the write depends on current document state.
///   Batch write : Multiple writes atomically, no reads. Cannot be retried.
///                 Use when you need to write N documents in one atomic operation.
///
/// Usage: inherit from this class and pass the collection name.
/// Example:
///   [FirestoreData]
///   public sealed class DeviceToken
///   {
///       [FirestoreDocumentId]     public string? Id        { get; set; }
///       [FirestoreProperty("userId")] public string UserId { get; set; } = "";
///       [FirestoreProperty("token")]  public string Token  { get; set; } = "";
///   }
///
///   public sealed class DeviceTokenRepository : FirestoreRepositoryBase{DeviceToken}
///   {
///       public DeviceTokenRepository(FirestoreClientFactory factory)
///           : base(factory, "device_tokens") { }
///   }
/// </summary>
public abstract class FirestoreRepositoryBase<T> where T : class
{
    protected readonly CollectionReference Collection;

    protected FirestoreRepositoryBase(FirestoreClientFactory factory, string collectionName)
    {
        Collection = factory.GetDatabase().Collection(collectionName);
    }

    /// <summary>Returns the document with the given ID, or null if it does not exist.</summary>
    public async Task<T?> GetByIdAsync(string documentId, CancellationToken cancellationToken = default)
    {
        var snapshot = await Collection.Document(documentId).GetSnapshotAsync(cancellationToken);
        return snapshot.Exists ? snapshot.ConvertTo<T>() : null;
    }

    /// <summary>Creates a document with a server-generated ID. Returns the new document ID.</summary>
    public async Task<string> CreateAsync(T document, CancellationToken cancellationToken = default)
    {
        var docRef = await Collection.AddAsync(document, cancellationToken);
        return docRef.Id;
    }

    /// <summary>
    /// Creates or overwrites the document with the given ID.
    /// If the document exists it is replaced; if not, it is created.
    /// Use SetAsync (not UpdateAsync) when the caller controls the document ID.
    /// </summary>
    public async Task SetAsync(string documentId, T document, CancellationToken cancellationToken = default)
    {
        await Collection.Document(documentId).SetAsync(document, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Partially updates the document — only the supplied fields are written.
    /// The document must already exist; use SetAsync to create-or-replace.
    ///
    /// LEARNING — SetOptions.MergeAll vs UpdateAsync:
    ///   SetAsync(doc, SetOptions.MergeAll) : merges fields; creates if missing.
    ///   UpdateAsync(updates)              : merges fields; fails if doc is missing.
    ///   Use MergeAll for upsert semantics; UpdateAsync when absence is an error.
    /// </summary>
    public async Task UpdateAsync(
        string documentId,
        Dictionary<string, object> updates,
        CancellationToken cancellationToken = default)
    {
        await Collection.Document(documentId).UpdateAsync(updates, cancellationToken: cancellationToken);
    }

    /// <summary>Deletes the document. No-op if the document does not exist.</summary>
    public async Task DeleteAsync(string documentId, CancellationToken cancellationToken = default)
    {
        await Collection.Document(documentId).DeleteAsync(cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Runs a read+write operation inside a Firestore transaction.
    /// The operation is retried automatically if a contention error occurs.
    ///
    /// LEARNING — Firestore transactions:
    ///   All reads in the transaction must happen before any writes.
    ///   Reads outside the transaction see the "before" snapshot of the data.
    ///   If any document read inside the transaction is modified concurrently,
    ///   Firestore aborts and retries the transaction (up to 5 times by default).
    /// </summary>
    public async Task<TResult> RunTransactionAsync<TResult>(
        Func<Transaction, Task<TResult>> operation,
        CancellationToken cancellationToken = default)
    {
        return await Collection.Database.RunTransactionAsync(operation, cancellationToken: cancellationToken);
    }
}
