using Fcg.Application.Promotions.CreatePromotion;
using Fcg.Application.Promotions.ListPromotions;

namespace Fcg.Api.Contracts.Promotions;

public sealed class PromotionResponse
{
    public Guid Id { get; init; }

    public Guid JogoId { get; init; }

    public string TituloJogo { get; init; } = string.Empty;

    public string Nome { get; init; } = string.Empty;

    public string Descricao { get; init; } = string.Empty;

    public decimal PrecoOriginal { get; init; }

    public decimal PercentualDesconto { get; init; }

    public decimal PrecoPromocional { get; init; }

    public DateTime InicioEm { get; init; }

    public DateTime FimEm { get; init; }

    public bool AtivaNoMomento { get; init; }

    public DateTime CriadaEm { get; init; }

    public static PromotionResponse FromCreateResult(CreatePromotionResult result)
    {
        return new PromotionResponse
        {
            Id = result.PromotionId,
            JogoId = result.GameId,
            TituloJogo = result.GameTitle,
            Nome = result.Name,
            Descricao = result.Description,
            PrecoOriginal = result.OriginalPrice,
            PercentualDesconto = result.DiscountPercentage,
            PrecoPromocional = result.PromotionalPrice,
            InicioEm = result.StartsAt,
            FimEm = result.EndsAt,
            AtivaNoMomento = result.Active,
            CriadaEm = result.CreatedAt
        };
    }

    public static PromotionResponse FromListResult(ListPromotionsResult result)
    {
        return new PromotionResponse
        {
            Id = result.PromotionId,
            JogoId = result.GameId,
            TituloJogo = result.GameTitle,
            Nome = result.Name,
            Descricao = result.Description,
            PrecoOriginal = result.OriginalPrice,
            PercentualDesconto = result.DiscountPercentage,
            PrecoPromocional = result.PromotionalPrice,
            InicioEm = result.StartsAt,
            FimEm = result.EndsAt,
            AtivaNoMomento = result.IsCurrentlyActive,
            CriadaEm = result.CreatedAt
        };
    }
}
