using Fcg.Application.Common;
using Fcg.Domain.Common;
using Fcg.Domain.Games;
using Fcg.Domain.Promotions;

namespace Fcg.Application.Promotions.CreatePromotion;

public sealed class CreatePromotionUseCase : ICreatePromotionUseCase
{
    private readonly IGameRepository _gameRepository;
    private readonly IPromotionRepository _promotionRepository;

    public CreatePromotionUseCase(
        IGameRepository gameRepository,
        IPromotionRepository promotionRepository)
    {
        _gameRepository = gameRepository;
        _promotionRepository = promotionRepository;
    }

    public async Task<CreatePromotionResult> ExecuteAsync(
        CreatePromotionCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var game = await _gameRepository.GetByIdAsync(command.GameId, cancellationToken);
        if (game is null)
        {
            throw new ResourceNotFoundException("Game was not found.");
        }

        if (!game.Active)
        {
            throw new DomainValidationException("Promotion can only be created for an active game.");
        }

        var promotion = Promotion.Create(
            command.GameId,
            command.Name,
            command.Description,
            command.DiscountPercentage,
            command.StartsAt,
            command.EndsAt);

        await _promotionRepository.AddAsync(promotion, cancellationToken);

        return new CreatePromotionResult(
            promotion.Id,
            game.Id,
            game.Title,
            promotion.Name,
            promotion.Description,
            game.Price,
            promotion.DiscountPercentage,
            CalculatePromotionalPrice(game.Price, promotion.DiscountPercentage),
            promotion.StartsAt,
            promotion.EndsAt,
            promotion.Active,
            promotion.CreatedAt);
    }

    private static decimal CalculatePromotionalPrice(decimal originalPrice, decimal discountPercentage)
    {
        return decimal.Round(
            originalPrice * (1 - (discountPercentage / 100m)),
            2,
            MidpointRounding.AwayFromZero);
    }
}
