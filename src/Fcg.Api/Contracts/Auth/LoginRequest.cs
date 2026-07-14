using System.ComponentModel.DataAnnotations;

namespace Fcg.Api.Contracts.Auth;

public sealed class LoginRequest
{
    [Required(AllowEmptyStrings = false)]
    public string Email { get; init; } = string.Empty;

    [Required(AllowEmptyStrings = false)]
    public string Senha { get; init; } = string.Empty;
}
