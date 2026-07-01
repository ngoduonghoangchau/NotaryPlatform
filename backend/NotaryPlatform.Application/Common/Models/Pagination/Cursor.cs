using System.Text;
using System.Text.Json;

namespace NotaryPlatform.Application.Common.Models.Pagination;

/// <summary>
/// Encodes and decodes opaque cursor tokens for cursor-based (keyset) pagination.
/// The token is a Base64url-encoded JSON value — safe in URLs, opaque to clients.
///
/// LEARNING — Why opaque?
///   If we exposed the raw sort key ("2026-06-25T10:30:00Z"), clients could construct
///   cursors manually and bypass server-side filtering. Encoding hides implementation details
///   and allows changing the sort key format without breaking client contracts.
///
/// LEARNING — Base64url vs standard Base64:
///   Standard Base64 uses '+' and '/' which need percent-encoding in URLs.
///   Base64url (RFC 4648 §5) replaces them with '-' and '_' — URL-safe, no encoding needed.
///
/// Usage:
///   string cursor = Cursor.Encode(lastItem.CreatedAt);       // in CursorPagedResult.Create
///   DateTimeOffset? value = Cursor.Decode&lt;DateTimeOffset&gt;(request.Cursor);  // in handler
/// </summary>
public static class Cursor
{
    public static string Encode<T>(T value)
    {
        var json  = JsonSerializer.Serialize(value);
        var bytes = Encoding.UTF8.GetBytes(json);
        return Base64UrlEncode(bytes);
    }

    /// <summary>
    /// Returns null (default) when cursor is null/empty or cannot be decoded.
    /// Invalid cursors are silently treated as "first page" — not a user-facing error.
    /// </summary>
    public static T? Decode<T>(string? cursor)
    {
        if (string.IsNullOrWhiteSpace(cursor))
            return default;

        try
        {
            var bytes = Base64UrlDecode(cursor);
            var json  = Encoding.UTF8.GetString(bytes);
            return JsonSerializer.Deserialize<T>(json);
        }
        catch
        {
            return default;
        }
    }

    private static string Base64UrlEncode(byte[] bytes) =>
        Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');

    private static byte[] Base64UrlDecode(string base64Url)
    {
        var padded  = base64Url.Replace('-', '+').Replace('_', '/');
        var padding = (4 - padded.Length % 4) % 4;
        return Convert.FromBase64String(padded + new string('=', padding));
    }
}
