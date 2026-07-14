using Fcg.Domain.Users;

namespace Fcg.Application.Authentication.AuthenticateUser;

public sealed record AuthenticateUserResult(
    Guid UserId,
    string Name,
    string Email,
    UserRole Role,
    string Token,
    DateTime ExpiresAtUtc);
