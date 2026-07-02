using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace NotaryPlatform.Infrastructure.Persistence.DbContexts;

public partial class NotaryPlatformDbContext
{
    // The scaffolded entities use DB column names (snake_case) in string-based annotation
    // attributes ([InverseProperty], [Index], [ForeignKey], [PrimaryKey]) instead of C#
    // property names (PascalCase). All PK, FK, relationship, and index configurations are
    // already present in OnModelCreating via Fluent API, so these attribute-based conventions
    // are redundant and must be suppressed to prevent EF from throwing on unresolvable names.
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Public convention types — remove directly.
        configurationBuilder.Conventions.Remove<InversePropertyAttributeConvention>();
        configurationBuilder.Conventions.Remove<IndexAttributeConvention>();
        configurationBuilder.Conventions.Remove<ForeignKeyAttributeConvention>();

        // KeyAttributeConvention handles both [Key] (property level) and [PrimaryKey] (entity
        // level) in EF Core 8. The scaffolded junction tables use [PrimaryKey("snake_case"...)]
        // but only PascalCase properties exist — removing this convention prevents the throw.
        // All PKs (single and composite) are already configured via HasKey() in OnModelCreating.
        configurationBuilder.Conventions.Remove<KeyAttributeConvention>();

        // In case PrimaryKeyAttributeConvention is a separate internal type: try via reflection.
        RemoveConventionByName(configurationBuilder.Conventions, "PrimaryKeyAttributeConvention");
    }

    private static void RemoveConventionByName(ConventionSetBuilder builder, string conventionTypeName)
    {
        var conventionType = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a =>
            {
                try { return a.GetTypes(); }
                catch { return Array.Empty<Type>(); }
            })
            .FirstOrDefault(t => t.Name == conventionTypeName);

        if (conventionType is null) return;

        var removeMethod = typeof(ConventionSetBuilder)
            .GetMethods()
            .FirstOrDefault(m => m.Name == "Remove" && m.IsGenericMethodDefinition);

        removeMethod?.MakeGenericMethod(conventionType).Invoke(builder, null);
    }
}
