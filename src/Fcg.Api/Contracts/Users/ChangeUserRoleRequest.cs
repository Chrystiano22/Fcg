using System.ComponentModel.DataAnnotations;

namespace Fcg.Api.Contracts.Users;

public sealed class ChangeUserRoleRequest
{
    [Required(AllowEmptyStrings = false)]
    public string Perfil { get; init; } = string.Empty;
}
