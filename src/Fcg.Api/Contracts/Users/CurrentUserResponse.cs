namespace Fcg.Api.Contracts.Users;

public sealed class CurrentUserResponse
{
    public Guid Id { get; init; }

    public string Nome { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Perfil { get; init; } = string.Empty;
}
