namespace NotaryPlatform.Domain.Common.Exceptions;

public sealed class BusinessRuleValidationException : DomainException
{
    public BusinessRuleValidationException(string message) : base(message)
    {
    }

    public BusinessRuleValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
