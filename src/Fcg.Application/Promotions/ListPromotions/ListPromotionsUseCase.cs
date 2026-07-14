using Fcg.Domain.Games;
using Fcg.Domain.Promotions;

namespace Fcg.Application.Promotions.ListPromotions;

public sealed class ListPromotionsUseCase : IListPromotionsUseCase
{
    private readonly IPromotionRepository _promotionRepository;
    private readonly IGameRepository _gameRepository;

    public ListPromotionsUseCase(
        IPromotionRepository promotionRepository,
        IGameRepository gameRepository)
    {
        _promotionRepository = promotionRepository;
        _gameRepository = gameRepository;
    }

    public async Task<IReadOnlyCollection<ListPromotionsResult>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        var promotions = await _promotionRepository.ListAsync(cancellationToken);
        var games = await _gameRepository.ListAsync(cancellationToken);
        var activeGames = games
            .Where(game => game.Active)
            .ToDictionary(game => game.Id);
        var utcNow = DateTime.UtcNow;

        return promotions
            .Where(promotion => promotion.Active && activeGames.ContainsKey(promotion.GameId))
            .OrderBy(promotion => promotion.StartsAt)
            .Select(promotion =>
            {
                var game = activeGames[promotion.GameId];
                var promotionalPrice = decimal.Round(
                    game.Price * (1 - (promotion.DiscountPercentage / 100m)),
                    2,
                    MidpointRounding.AwayFromZero);

                return new ListPromotionsResult(
                    promotion.Id,
                    game.Id,
                    game.Title,
                    promotion.Name,
                    promotion.Description,
                    game.Price,
                    promotion.DiscountPercentage,
                    promotionalPrice,
                    promotion.StartsAt,
                    promotion.EndsAt,
                    promotion.IsCurrentlyActive(utcNow),
                    promotion.CreatedAt);
            })
            .ToArray();
    }
}
