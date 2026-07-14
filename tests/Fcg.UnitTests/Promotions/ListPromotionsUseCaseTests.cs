using Fcg.Application.Promotions.ListPromotions;
using Fcg.Domain.Games;
using Fcg.Domain.Promotions;

namespace Fcg.UnitTests.Promotions;

public sealed class ListPromotionsUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithPromotions_ReturnsCatalog()
    {
        var game = Game.Create(
            "Architecture Quest",
            "Educational game about software architecture.",
            79.90m,
            "Education");
        var promotion = Promotion.Create(
            game.Id,
            "Semana Tech",
            "Desconto especial para alunos.",
            20,
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddDays(5));
        var gameRepository = new FakeGameRepository(game);
        var promotionRepository = new FakePromotionRepository(promotion);
        var useCase = new ListPromotionsUseCase(promotionRepository, gameRepository);

        var result = await useCase.ExecuteAsync();

        Assert.Single(result);
        Assert.Equal("Architecture Quest", result.Single().GameTitle);
        Assert.Equal(63.92m, result.Single().PromotionalPrice);
        Assert.True(result.Single().IsCurrentlyActive);
    }

    private sealed class FakeGameRepository : IGameRepository
    {
        private readonly IReadOnlyCollection<Game> _games;

        public FakeGameRepository(params Game[] games)
        {
            _games = games;
        }

        public Task AddAsync(Game game, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<Game?> GetByIdAsync(Guid gameId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Game?>(_games.FirstOrDefault(game => game.Id == gameId));
        }

        public Task<IReadOnlyCollection<Game>> ListAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_games);
        }

        public Task UpdateAsync(Game game, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }
    }

    private sealed class FakePromotionRepository : IPromotionRepository
    {
        private readonly IReadOnlyCollection<Promotion> _promotions;

        public FakePromotionRepository(params Promotion[] promotions)
        {
            _promotions = promotions;
        }

        public Task AddAsync(Promotion promotion, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<IReadOnlyCollection<Promotion>> ListAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_promotions);
        }
    }
}
