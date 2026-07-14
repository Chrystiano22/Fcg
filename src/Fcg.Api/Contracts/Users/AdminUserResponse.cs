using Fcg.Application.Users.ChangeUserRole;
using Fcg.Application.Users.ListUsers;

namespace Fcg.Api.Contracts.Users;

public sealed class AdminUserResponse
{
    public Guid Id { get; init; }

    public string Nome { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Perfil { get; init; } = string.Empty;

    public DateTime CriadoEm { get; init; }

    public DateTime? AtualizadoEm { get; init; }

    public static AdminUserResponse FromListResult(ListUsersResult result)
    {
        return new AdminUserResponse
        {
            Id = result.UserId,
            Nome = result.Name,
            Email = result.Email,
            Perfil = result.Role.ToString(),
            CriadoEm = result.CreatedAt
        };
    }

    public static AdminUserResponse FromChangeRoleResult(ChangeUserRoleResult result)
    {
        return new AdminUserResponse
        {
            Id = result.UserId,
            Nome = result.Name,
            Email = result.Email,
            Perfil = result.Role.ToString(),
            AtualizadoEm = result.UpdatedAt
        };
    }
}
