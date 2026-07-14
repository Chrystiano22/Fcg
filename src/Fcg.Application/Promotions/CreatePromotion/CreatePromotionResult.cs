namespace Fcg.Application.Promotions.CreatePromotion;

public sealed record CreatePromotionResult(
    Guid PromotionId,
    Guid GameId,
    string GameTitle,
    string Name,
    string Description,
    decimal OriginalPrice,
    decimal DiscountPercentage,
    decimal PromotionalPrice,
    DateTime StartsAt,
    DateTime EndsAt,
    bool Active,
    DateTime CreatedAt);
