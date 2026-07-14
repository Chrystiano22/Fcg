namespace Fcg.Api.Contracts.Promotions;

public sealed class CreatePromotionRequest
{
    public Guid JogoId { get; init; }

    public string Nome { get; init; } = string.Empty;

    public string Descricao { get; init; } = string.Empty;

    public decimal PercentualDesconto { get; init; }

    public DateTime InicioEm { get; init; }

    public DateTime FimEm { get; init; }
}
