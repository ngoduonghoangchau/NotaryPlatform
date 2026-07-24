using NotaryPlatform.Application.Shared.Interfaces;

namespace NotaryPlatform.Application.Features.Core.Commands.CompletePasswordReset;

/// <summary>
/// UC-AUTH-05 Step B — the user consumes the emailed reset token to set a new password. Anonymous: the
/// token is the presented credential. A write ⇒ <see cref="ICommand"/> (transactional). On success the
/// token is marked used (single-use, BR-AUTH-09) and all the user's sessions are revoked (BR-AUTH-06).
/// </summary>
public sealed record CompletePasswordResetCommand(string Token, string NewPassword) : ICommand;
