using System.Net;
using MediatR;
using Microsoft.Extensions.Options;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Abstractions.Messaging;
using NotaryPlatform.Application.Abstractions.System;
using NotaryPlatform.Application.Shared.Constants;
using NotaryPlatform.Application.Shared.Exceptions;
using NotaryPlatform.Application.Shared.Settings;

namespace NotaryPlatform.Application.Features.Core.Commands.InitiatePasswordReset;

/// <summary>
/// Executes UC-AUTH-05 Step A. See <c>docs/usecase/Auth/UC-AUTH-05/implement_plan.md</c>.
///
/// Resolves the target user within the admin's tenant (tenant isolation, BR-AUTH-10), retires any prior
/// unused reset token (one active at a time), persists a new token's SHA-256 hash with a 1-hour expiry
/// (BR-AUTH-09), and emails the reset link. The email is sent in-handler (pre-commit): a send failure
/// rolls the transaction back, so a persisted token never exists without its email.
/// </summary>
internal sealed class InitiatePasswordResetCommandHandler : IRequestHandler<InitiatePasswordResetCommand>
{
    private readonly IAuthRepository _auth;
    private readonly IResetTokenService _tokens;
    private readonly IEmailSender _email;
    private readonly ICurrentUser _currentUser;
    private readonly IDateTime _clock;
    private readonly AppUrls _urls;

    public InitiatePasswordResetCommandHandler(
        IAuthRepository auth,
        IResetTokenService tokens,
        IEmailSender email,
        ICurrentUser currentUser,
        IDateTime clock,
        IOptions<AppUrls> urls)
    {
        _auth = auth;
        _tokens = tokens;
        _email = email;
        _currentUser = currentUser;
        _clock = clock;
        _urls = urls.Value;
    }

    public async Task Handle(InitiatePasswordResetCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId ?? throw new UnauthorizedException();

        // Tenant-scoped: an admin can only reset a user in their own tenant (BR-AUTH-10). A user in
        // another tenant (or none) surfaces as 404 — no cross-tenant enumeration.
        var user = await _auth.FindActiveUserByIdAsync(request.UserId, tenantId, cancellationToken)
            ?? throw new NotFoundException("User", request.UserId);

        // One active reset token per user: retire any prior unused token before issuing a new one.
        await _auth.InvalidatePasswordResetTokensForUserAsync(user.UserId, cancellationToken);

        var rawToken = _tokens.CreateResetToken();
        var tokenHash = _tokens.HashResetToken(rawToken);
        var expiresAt = _clock.UtcNow.Add(AppDefaults.Security.PasswordResetTokenExpiry);   // BR-AUTH-09 (1h)

        await _auth.AddPasswordResetTokenAsync(
            new PasswordResetTokenCreate(
                TenantId: tenantId,
                UserId: user.UserId,
                TokenHash: tokenHash,
                ExpiresAtUtc: expiresAt.UtcDateTime,
                CreatedByUserId: _currentUser.UserId,
                CreatedIp: ParseIp(_currentUser.IpAddress)),
            cancellationToken);

        // Send the reset link (in-handler, pre-commit — a failure rolls back so no orphan token remains).
        await _email.SendAsync(BuildResetEmail(user.Email, user.DisplayName, rawToken), cancellationToken);
        // TransactionBehavior commits.
    }

    private EmailMessage BuildResetEmail(string to, string displayName, string rawToken)
    {
        var link = $"{_urls.PublicBaseUrl.TrimEnd('/')}/reset-password?token={Uri.EscapeDataString(rawToken)}";
        var name = WebUtility.HtmlEncode(displayName);

        // Email HTML uses table layout + inline styles only (no <style> block or external CSS/fonts),
        // which is what email clients render reliably. A brand-coloured button is the primary CTA, with a
        // copy-paste fallback link below it.
        const string font = "font-family:'Segoe UI',Roboto,Helvetica,Arial,sans-serif;";
        var htmlBody =
            "<!DOCTYPE html>" +
            "<html lang=\"en\"><body style=\"margin:0;padding:0;background-color:#f4f5f7;\">" +
            "<table role=\"presentation\" width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" style=\"background-color:#f4f5f7;padding:24px 0;\">" +
            "<tr><td align=\"center\">" +
            "<table role=\"presentation\" width=\"480\" cellpadding=\"0\" cellspacing=\"0\" style=\"max-width:480px;width:100%;background-color:#ffffff;border:1px solid #e5e7eb;border-radius:12px;\">" +
            $"<tr><td style=\"padding:28px 32px 6px 32px;{font}\">" +
            "<div style=\"font-size:13px;font-weight:700;letter-spacing:1px;color:#4f46e5;\">NOTARYPLATFORM</div></td></tr>" +
            $"<tr><td style=\"padding:6px 32px 0 32px;{font}\">" +
            "<h1 style=\"margin:0;font-size:22px;line-height:28px;color:#111827;\">Reset your password</h1></td></tr>" +
            $"<tr><td style=\"padding:16px 32px 0 32px;{font}font-size:15px;line-height:22px;color:#374151;\">" +
            $"<p style=\"margin:0 0 14px 0;\">Hi {name},</p>" +
            "<p style=\"margin:0;\">An administrator started a password reset for your account. " +
            "Click the button below to choose a new password.</p></td></tr>" +
            "<tr><td align=\"center\" style=\"padding:22px 32px 6px 32px;\">" +
            $"<a href=\"{link}\" style=\"display:inline-block;background-color:#4f46e5;color:#ffffff;{font}" +
            "font-size:15px;font-weight:600;text-decoration:none;padding:12px 30px;border-radius:8px;\">Reset password</a></td></tr>" +
            $"<tr><td style=\"padding:8px 32px 0 32px;{font}font-size:13px;line-height:20px;color:#6b7280;\">" +
            "<p style=\"margin:0;\">This link expires in <strong>60 minutes</strong> and can be used once.</p></td></tr>" +
            $"<tr><td style=\"padding:16px 32px 0 32px;{font}font-size:12px;line-height:18px;color:#9ca3af;\">" +
            "<p style=\"margin:0 0 4px 0;\">Or paste this link into your browser:</p>" +
            $"<p style=\"margin:0;word-break:break-all;color:#4f46e5;\">{link}</p></td></tr>" +
            "<tr><td style=\"padding:20px 32px 26px 32px;\">" +
            "<hr style=\"border:none;border-top:1px solid #e5e7eb;margin:0 0 14px 0;\" />" +
            $"<p style=\"margin:0;{font}font-size:12px;line-height:18px;color:#9ca3af;\">" +
            "Didn't request this? You can safely ignore this email — your password won't change.</p></td></tr>" +
            "</table></td></tr></table></body></html>";

        return new EmailMessage
        {
            To = to,
            Subject = "Reset your NotaryPlatform password",
            HtmlBody = htmlBody,
            TextBody =
                $"Hi {displayName},\n\n" +
                "An administrator started a password reset for your account.\n" +
                "Open this link to choose a new password (valid 60 minutes, single use):\n\n" +
                $"{link}\n\n" +
                "Didn't request this? You can safely ignore this email — your password won't change.",
        };
    }

    private static IPAddress? ParseIp(string? ip) =>
        IPAddress.TryParse(ip, out var address) ? address : null;
}
