namespace NotaryPlatform.Application.Shared.Constants;

/// <summary>
/// Reusable FluentValidation error messages.
/// Import these in validators instead of inline strings to keep messages consistent.
/// </summary>
public static class ValidationMessages
{
    public static string Required(string fieldName) => $"{fieldName} is required.";
    public static string MaxLength(string fieldName, int max) => $"{fieldName} must not exceed {max} characters.";
    public static string MinLength(string fieldName, int min) => $"{fieldName} must be at least {min} characters.";
    public static string InvalidFormat(string fieldName) => $"{fieldName} format is invalid.";
    public static string MustBePositive(string fieldName) => $"{fieldName} must be a positive value.";
    public static string MustBeInRange(string fieldName, int min, int max) => $"{fieldName} must be between {min} and {max}.";
    public static string MustBeFutureDate(string fieldName) => $"{fieldName} must be a future date.";
    public static string MustBePastDate(string fieldName) => $"{fieldName} must be a past date.";
    public static string InvalidEmail => "A valid email address is required.";
    public static string InvalidPhoneNumber => "A valid US phone number is required (e.g., +1-555-123-4567).";
    public static string InvalidGuid(string fieldName) => $"{fieldName} must be a valid identifier.";
    public static string MustBeUnique(string fieldName) => $"{fieldName} already exists.";
    public static string InvalidEnumValue(string fieldName) => $"{fieldName} contains an unrecognized value.";
    public static string FileTooLarge(string fieldName, long maxMb) => $"{fieldName} exceeds the maximum allowed size of {maxMb} MB.";
    public static string UnsupportedFileType(string fieldName) => $"{fieldName} file type is not supported.";
}
