namespace Fcg.Application.Promotions.ListPromotions;

public sealed record ListPromotionsResult(
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
    bool IsCurrentlyActive,
    DateTime CreatedAt);
