using Fcg.Application.Users.RegisterUser;

namespace Fcg.Api.Contracts.Users;

public sealed class RegisterUserResponse
{
    public Guid Id { get; init; }

    public string Nome { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Perfil { get; init; } = string.Empty;

    public DateTime CriadoEm { get; init; }

    public static RegisterUserResponse FromResult(RegisterUserResult result)
    {
        return new RegisterUserResponse
        {
            Id = result.UserId,
            Nome = result.Name,
            Email = result.Email,
            Perfil = result.Role.ToString(),
            CriadoEm = result.CreatedAt
        };
    }
}
