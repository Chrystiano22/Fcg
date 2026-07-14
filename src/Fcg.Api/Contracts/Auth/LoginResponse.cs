using Fcg.Application.Authentication.AuthenticateUser;

namespace Fcg.Api.Contracts.Auth;

public sealed class LoginResponse
{
    public string Token { get; init; } = string.Empty;

    public string TipoToken { get; init; } = "Bearer";

    public DateTime ExpiraEm { get; init; }

    public Guid UsuarioId { get; init; }

    public string Nome { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Perfil { get; init; } = string.Empty;

    public static LoginResponse FromResult(AuthenticateUserResult result)
    {
        return new LoginResponse
        {
            Token = result.Token,
            ExpiraEm = result.ExpiresAtUtc,
            UsuarioId = result.UserId,
            Nome = result.Name,
            Email = result.Email,
            Perfil = result.Role.ToString()
        };
    }
}
