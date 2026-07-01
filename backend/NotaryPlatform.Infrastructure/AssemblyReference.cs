namespace NotaryPlatform.Infrastructure;

/// <summary>
/// Marker class used to reference the Infrastructure assembly without coupling
/// to any concrete type. Used by the API layer for MediatR / other assembly-
/// scanning registrations that need to discover types in this project.
///
/// Usage in Program.cs:
/// <code>
/// builder.Services.AddMediatR(cfg =>
///     cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly));
/// </code>
/// </summary>
public sealed class AssemblyReference;
