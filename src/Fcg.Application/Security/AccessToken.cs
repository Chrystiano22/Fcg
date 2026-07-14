namespace Fcg.Application.Security;

public sealed record AccessToken(
    string Value,
    DateTime ExpiresAtUtc);
