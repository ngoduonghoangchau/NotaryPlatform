using FluentAssertions;
using NotaryPlatform.Application.Features.Core.Commands.RemoveTrustedDevice;
using NotaryPlatform.Application.Features.Core.Validators;
using Xunit;

namespace NotaryPlatform.Application.Tests.Features.Core.Validators;

/// <summary>Unit tests for <see cref="RemoveTrustedDeviceCommandValidator"/> (TC-VAL-03).</summary>
public sealed class RemoveTrustedDeviceCommandValidatorTests
{
    private readonly RemoveTrustedDeviceCommandValidator _validator = new();

    [Fact] // empty id is invalid
    public void Empty_device_id_is_invalid()
    {
        var result = _validator.Validate(new RemoveTrustedDeviceCommand(Guid.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RemoveTrustedDeviceCommand.TrustedDeviceId));
    }

    [Fact]
    public void Non_empty_device_id_is_valid()
    {
        var result = _validator.Validate(new RemoveTrustedDeviceCommand(Guid.NewGuid()));

        result.IsValid.Should().BeTrue();
    }
}
