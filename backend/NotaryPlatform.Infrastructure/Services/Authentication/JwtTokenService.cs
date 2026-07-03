using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Shared.Interfaces;
using NotaryPlatform.Application.Shared.Models.Auth;

namespace NotaryPlatform.Infrastructure.Services.Authentication;

/// <summary>
/// Produces and validates JWT access tokens and opaque refresh tokens.
/// Implements IJwtTokenService (defined in Application).
///
/// LEARNING — Access token vs Refresh token:
///   Access token  : Short-lived JWT (60 min by default). Stateless — validated by
///                   checking the cryptographic signature and expiry. No DB lookup.
///   Refresh token : Long-lived random bytes (30 days). Stored HASHED in the DB
///                   (refresh_tokens.token_hash). If the DB is compromised, the
///                   attacker cannot replay raw tokens without reversing SHA-256.
///
/// LEARNING — Refresh token rotation:
///   On each use, issue a new refresh token and revoke the old one
///   (set RevokedAt + ReplacedByTokenId). If a revoked token is replayed,
///   it means the token was stolen — invalidate the entire session family.
///
/// LEARNING — Why SHA-256 (not bcrypt) for refresh tokens?
///   Refresh tokens are 64 random bytes (high entropy). Dictionary attacks are
///   not possible — SHA-256 is sufficient and much faster. bcrypt's slowness is
///   only needed when the input is low-entropy (like passwords).
/// </summary>
public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _settings;
    private readonly SymmetricSecurityKey _signingKey;

    public JwtTokenService(IOptions<JwtSettings> options)
    {
        _settings = options.Value;
        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
    }

    public AccessTokenResult CreateAccessToken(JwtTokenClaims claims)
    {
        var now = DateTime.UtcNow;
        var expiresAt = now.AddMinutes(_settings.AccessTokenExpiryMinutes);
        var jwtClaims = BuildClaims(claims);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: jwtClaims,
            notBefore: now,
            expires: expiresAt,
            signingCredentials: new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256));

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        // Report the token's own expiry (single source of truth) — never a separate constant.
        return new AccessTokenResult(tokenString, new DateTimeOffset(expiresAt, TimeSpan.Zero));
    }

    public string CreateRefreshToken()
    {
        // 64 cryptographically random bytes → 86-char URL-safe base64 string.
        // This is the RAW token returned to the client. Never store this directly.
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public string HashRefreshToken(string rawToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public ClaimsPrincipal? ValidateAccessToken(string token)
    {
        try
        {
            return new JwtSecurityTokenHandler().ValidateToken(
                token,
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _settings.Issuer,
                    ValidAudience = _settings.Audience,
                    IssuerSigningKey = _signingKey,
                    ClockSkew = TimeSpan.Zero,
                },
                out _);
        }
        catch
        {
            return null;
        }
    }

    private static List<Claim> BuildClaims(JwtTokenClaims claims)
    {
        var list = new List<Claim>
        {
            new(JwtClaimTypes.UserId, claims.UserId.ToString()),
            new(JwtClaimTypes.TenantId, claims.TenantId.ToString()),
            new(ClaimTypes.Name, claims.UserName),
            new(ClaimTypes.Email, claims.Email),
            // jti — unique per token; useful for per-token revocation.
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        if (claims.BranchId.HasValue)
            list.Add(new Claim(JwtClaimTypes.BranchId, claims.BranchId.Value.ToString()));

        if (claims.RegionId.HasValue)
            list.Add(new Claim(JwtClaimTypes.RegionId, claims.RegionId.Value.ToString()));

        // Roles and permissions: one Claim per value (not comma-separated).
        // FindAll() in CurrentUserService retrieves them correctly this way.
        foreach (var role in claims.Roles)
            list.Add(new Claim(ClaimTypes.Role, role));

        foreach (var permission in claims.Permissions)
            list.Add(new Claim(JwtClaimTypes.Permissions, permission));

        return list;
    }
}
