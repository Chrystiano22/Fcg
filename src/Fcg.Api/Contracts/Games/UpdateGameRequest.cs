using System.ComponentModel.DataAnnotations;

namespace Fcg.Api.Contracts.Games;

public sealed class UpdateGameRequest
{
    [Required(AllowEmptyStrings = false)]
    public string Titulo { get; init; } = string.Empty;

    [Required(AllowEmptyStrings = false)]
    public string Descricao { get; init; } = string.Empty;

    [Range(0, 999999.99)]
    public decimal Preco { get; init; }

    [Required(AllowEmptyStrings = false)]
    public string Categoria { get; init; } = string.Empty;
}
