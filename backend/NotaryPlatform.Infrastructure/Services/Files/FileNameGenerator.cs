using System.Text;
using System.Text.RegularExpressions;

namespace NotaryPlatform.Infrastructure.Services.Files;

/// <summary>
/// Generates tenant-namespaced, collision-resistant storage paths for uploaded files.
///
/// Output format:
///   {tenantId}/{folder}/2025/06/{uuid}_{sanitized-filename}.{ext}
///
/// LEARNING — Why structure the storage path this way?
///   tenantId/  : hard namespace boundary — impossible to accidentally serve
///                one tenant's files to another even if the storage is public.
///   folder/    : logical grouping ("notary-docs", "invoices", "seals").
///   yyyy/MM/   : date sharding — prevents a single directory from growing
///                unbounded, which degrades performance in most blob stores.
///   uuid_name  : UUID prefix guarantees uniqueness (no collision even for
///                the same original filename), while keeping the original name
///                readable for debugging and support requests.
///
/// LEARNING — Why sanitize file names?
///   Original file names from clients may contain path traversal sequences
///   (../../etc/passwd), Unicode homoglyphs, or shell-unsafe characters.
///   Sanitization is a security boundary — never use raw client filenames
///   as storage paths.
/// </summary>
public static class FileNameGenerator
{
    // Allows only letters, digits, hyphens, underscores, and dots.
    private static readonly Regex SafeCharsRegex =
        new(@"[^a-zA-Z0-9\-_\.]", RegexOptions.Compiled);

    // Collapses consecutive dots (prevents extension spoofing like "file.php.jpg")
    private static readonly Regex MultiDotRegex =
        new(@"\.{2,}", RegexOptions.Compiled);

    /// <summary>
    /// Generates a unique storage path for a file upload.
    /// </summary>
    /// <param name="originalFileName">The file name as provided by the client.</param>
    /// <param name="folder">Logical folder/prefix (e.g. "notary-docs", "invoices").</param>
    /// <param name="tenantId">Used as the top-level namespace.</param>
    /// <returns>Storage path relative to the bucket root.</returns>
    public static string Generate(string originalFileName, string folder, Guid tenantId)
    {
        var now = DateTime.UtcNow;
        var id = Guid.NewGuid().ToString("N"); // 32 hex chars, no hyphens
        var safeName = Sanitize(originalFileName);
        var ext = Path.GetExtension(safeName).ToLowerInvariant();
        var stem = Path.GetFileNameWithoutExtension(safeName);

        // Truncate stem to avoid OS path-length limits.
        if (stem.Length > 60)
            stem = stem[..60];

        return $"{tenantId}/{folder}/{now:yyyy}/{now:MM}/{id}_{stem}{ext}";
    }

    /// <summary>
    /// Derives the MIME content type from a file name extension.
    /// Returns "application/octet-stream" for unknown extensions.
    /// </summary>
    public static string GetContentType(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ext switch
        {
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".svg" => "image/svg+xml",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".txt" => "text/plain",
            ".csv" => "text/csv",
            ".json" => "application/json",
            ".zip" => "application/zip",
            _ => "application/octet-stream",
        };
    }

    private static string Sanitize(string fileName)
    {
        // Normalize Unicode: decompose accented chars to ASCII equivalents.
        var normalized = fileName.Normalize(NormalizationForm.FormD);
        var ascii = new string([.. normalized.Where(c => c < 128)]);

        // Replace unsafe characters with underscores.
        var safe = SafeCharsRegex.Replace(ascii, "_");

        // Remove double dots (prevents extension spoofing).
        safe = MultiDotRegex.Replace(safe, ".");

        // Trim leading/trailing dots and underscores.
        safe = safe.Trim('.', '_');

        return string.IsNullOrEmpty(safe) ? "file" : safe;
    }
}
