using Fcg.Domain.Users;

namespace Fcg.Application.Users.ChangeUserRole;

public sealed record ChangeUserRoleCommand(UserRole Role);
