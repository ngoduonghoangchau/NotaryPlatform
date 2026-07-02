using FluentValidation.Results;
using NotaryPlatform.Application.Shared.Models.Responses;

namespace NotaryPlatform.Application.Shared.Exceptions;

/// <summary>
/// Thrown by ValidationBehavior when one or more FluentValidation rules fail.
/// Maps to HTTP 400 Bad Request.
/// </summary>
public sealed class ValidationException : Exception
{
    public IReadOnlyList<FieldError> Errors { get; }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base("One or more validation failures occurred.")
    {
        Errors = failures
            .Select(f => new FieldError
            {
                Field = ToCamelCase(f.PropertyName),
                Message = f.ErrorMessage,
                Code = f.ErrorCode
            })
            .ToList()
            .AsReadOnly();
    }

    public ValidationException(string field, string message, string? code = null)
        : base(message)
    {
        Errors = new List<FieldError>
        {
            new() { Field = ToCamelCase(field), Message = message, Code = code }
        }.AsReadOnly();
    }

    private static string ToCamelCase(string propertyName)
    {
        if (string.IsNullOrEmpty(propertyName)) return propertyName;
        var parts = propertyName.Split('.');
        return string.Join(".", parts.Select(p =>
            p.Length == 0 ? p : char.ToLowerInvariant(p[0]) + p[1..]));
    }
}
