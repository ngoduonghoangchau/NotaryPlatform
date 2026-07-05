using System.Text;
using Npgsql;
using NotaryPlatform.Domain.Features.Core.Enums;

namespace NotaryPlatform.Infrastructure.Persistence;

/// <summary>
/// Registers every domain CLR enum with its PostgreSQL enum type on the Npgsql data source.
///
/// WHY: <c>modelBuilder.HasPostgresEnum(...)</c> teaches EF about the enums for *migrations*
/// only. For runtime query translation and parameter binding the CLR enum must also be mapped
/// on the data source — otherwise EF sends the value as a plain integer and PostgreSQL rejects
/// it (<c>42883: operator does not exist: core.user_status = integer</c>).
///
/// Mappings are derived by reflection so new enums are picked up automatically:
///   CLR type   NotaryPlatform.Domain.Features.&lt;Schema&gt;.Enums.&lt;Name&gt;
///   PG  type   &lt;schema&gt;.&lt;snake_case(Name)&gt;
/// Deriving the schema from the namespace also disambiguates same-named enums across schemas
/// (e.g. Billing, Communication, Compliance, and Security each define an <c>AuditEventType</c>).
///
/// A mapping whose PostgreSQL type does not exist is harmless: Npgsql resolves enum types
/// lazily on first use, so an unused mapping is never resolved.
/// </summary>
public static class NpgsqlEnumMappings
{
    private const string EnumsNamespacePrefix = "NotaryPlatform.Domain.Features.";
    private const string EnumsNamespaceSuffix = ".Enums";

    public static void MapDomainEnums(NpgsqlDataSourceBuilder builder)
    {
        // The open generic NpgsqlDataSourceBuilder.MapEnum<TEnum>(string? pgName, INpgsqlNameTranslator?).
        var mapEnum = typeof(NpgsqlDataSourceBuilder).GetMethods()
            .First(m => m.Name == nameof(NpgsqlDataSourceBuilder.MapEnum)
                        && m.IsGenericMethodDefinition
                        && m.GetParameters() is { Length: 2 } ps
                        && ps[0].ParameterType == typeof(string));

        // typeof(UserStatus) anchors the Domain assembly; every enum below lives in it.
        foreach (var enumType in typeof(UserStatus).Assembly.GetTypes())
        {
            if (!enumType.IsEnum) continue;

            var ns = enumType.Namespace;
            if (ns is null
                || !ns.StartsWith(EnumsNamespacePrefix, StringComparison.Ordinal)
                || !ns.EndsWith(EnumsNamespaceSuffix, StringComparison.Ordinal))
                continue;

            var pgName = $"{SchemaFromNamespace(ns)}.{ToSnakeCase(enumType.Name)}";
            mapEnum.MakeGenericMethod(enumType).Invoke(builder, new object?[] { pgName, null });
        }
    }

    // "NotaryPlatform.Domain.Features.Core.Enums" -> "core"; "...CRM.Enums" -> "crm".
    private static string SchemaFromNamespace(string ns)
    {
        var parts = ns.Split('.');
        return parts[^2].ToLowerInvariant(); // segment between "Features" and "Enums"
    }

    // PascalCase -> snake_case (domain enum type names have no consecutive capitals).
    private static string ToSnakeCase(string name)
    {
        var sb = new StringBuilder(name.Length + 8);
        for (var i = 0; i < name.Length; i++)
        {
            var c = name[i];
            if (char.IsUpper(c))
            {
                if (i > 0) sb.Append('_');
                sb.Append(char.ToLowerInvariant(c));
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }
}
