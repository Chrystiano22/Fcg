namespace Fcg.Infrastructure.Security;

public sealed class JwtTokenSettings
{
    public string Issuer { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    public string SecretKey { get; init; } = string.Empty;

    public int ExpirationInMinutes { get; init; } = 60;
}
