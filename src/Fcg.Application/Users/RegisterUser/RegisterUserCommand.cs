using Fcg.Domain.Users;

namespace Fcg.Application.Users.RegisterUser;

public sealed record RegisterUserCommand(
    string Name,
    string Email,
    string Password,
    UserRole Role = UserRole.User);
