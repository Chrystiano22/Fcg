namespace Fcg.Application.Promotions.CreatePromotion;

public sealed record CreatePromotionCommand(
    Guid GameId,
    string Name,
    string Description,
    decimal DiscountPercentage,
    DateTime StartsAt,
    DateTime EndsAt);
