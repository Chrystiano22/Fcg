using Fcg.Domain.Users;

namespace Fcg.Application.Users.RegisterUser;

public sealed record RegisterUserResult(
    Guid UserId,
    string Name,
    string Email,
    UserRole Role,
    DateTime CreatedAt);
