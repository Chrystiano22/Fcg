using Fcg.Domain.Users;

namespace Fcg.Application.Users.ChangeUserRole;

public sealed record ChangeUserRoleResult(
    Guid UserId,
    string Name,
    string Email,
    UserRole Role,
    DateTime UpdatedAt);
