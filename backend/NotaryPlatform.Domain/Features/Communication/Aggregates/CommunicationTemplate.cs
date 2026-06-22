using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Communication.Enums;

namespace NotaryPlatform.Domain.Features.Communication.Aggregates;

public sealed class CommunicationTemplate : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public TemplateType TemplateType { get; private set; }
    public ChannelType ChannelType { get; private set; }
    public bool IsActive { get; private set; }
    public string SubjectTemplate { get; private set; } = string.Empty;
    public string BodyTemplate { get; private set; } = string.Empty;
    public string? Notes { get; private set; }

    private CommunicationTemplate()
    {
    }

    private CommunicationTemplate(Guid id, Guid tenantId, string code, string name, TemplateType templateType, ChannelType channelType, string subjectTemplate, string bodyTemplate)
        : base(id)
    {
        TenantId = tenantId;
        Code = code;
        Name = name;
        TemplateType = templateType;
        ChannelType = channelType;
        SubjectTemplate = subjectTemplate;
        BodyTemplate = bodyTemplate;
        IsActive = true;
    }

    public static CommunicationTemplate Create(
        Guid tenantId,
        string code,
        string name,
        TemplateType templateType,
        ChannelType channelType,
        string subjectTemplate,
        string bodyTemplate,
        string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (string.IsNullOrWhiteSpace(code)) throw new BusinessRuleValidationException("Template code is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new BusinessRuleValidationException("Template name is required.");
        if (string.IsNullOrWhiteSpace(subjectTemplate)) throw new BusinessRuleValidationException("Subject template is required.");
        if (string.IsNullOrWhiteSpace(bodyTemplate)) throw new BusinessRuleValidationException("Body template is required.");

        return new CommunicationTemplate(
            Guid.NewGuid(),
            tenantId,
            code.Trim().ToUpperInvariant(),
            name.Trim(),
            templateType,
            channelType,
            subjectTemplate.Trim(),
            bodyTemplate.Trim())
        {
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }
}
