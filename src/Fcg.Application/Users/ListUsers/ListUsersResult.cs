using Fcg.Domain.Users;

namespace Fcg.Application.Users.ListUsers;

public sealed record ListUsersResult(
    Guid UserId,
    string Name,
    string Email,
    UserRole Role,
    DateTime CreatedAt);
