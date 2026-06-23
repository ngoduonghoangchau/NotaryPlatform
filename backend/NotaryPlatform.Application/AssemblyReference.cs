using System.Reflection;

namespace NotaryPlatform.Application;

/// <summary>
/// Exposes the Application assembly reference for use in DI registration
/// (MediatR, FluentValidation, AutoMapper assembly scanning).
/// </summary>
public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
