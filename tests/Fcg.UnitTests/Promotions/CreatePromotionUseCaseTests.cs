using Fcg.Application.Common;
using Fcg.Application.Promotions.CreatePromotion;
using Fcg.Domain.Games;
using Fcg.Domain.Promotions;

namespace Fcg.UnitTests.Promotions;

public sealed class CreatePromotionUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidCommand_PersistsPromotion()
    {
        var game = Game.Create(
            "Architecture Quest",
            "Educational game about software architecture.",
            79.90m,
            "Education");
        var gameRepository = new FakeGameRepository(game);
        var promotionRepository = new FakePromotionRepository();
        var useCase = new CreatePromotionUseCase(gameRepository, promotionRepository);

        var result = await useCase.ExecuteAsync(new CreatePromotionCommand(
            game.Id,
            "Semana Tech",
            "Desconto especial para alunos.",
            20,
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddDays(5)));

        Assert.NotEqual(Guid.Empty, result.PromotionId);
        Assert.Equal("Architecture Quest", result.GameTitle);
        Assert.Equal(63.92m, result.PromotionalPrice);
        Assert.Single(promotionRepository.AddedPromotions);
    }

    [Fact]
    public async Task ExecuteAsync_WithUnknownGame_ThrowsException()
    {
        var gameRepository = new FakeGameRepository();
        var promotionRepository = new FakePromotionRepository();
        var useCase = new CreatePromotionUseCase(gameRepository, promotionRepository);

        var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(() => useCase.ExecuteAsync(
            new CreatePromotionCommand(
                Guid.NewGuid(),
                "Semana Tech",
                "Desconto especial para alunos.",
                20,
                DateTime.UtcNow.AddDays(-1),
                DateTime.UtcNow.AddDays(5))));

        Assert.Equal("Game was not found.", exception.Message);
    }

    private sealed class FakeGameRepository : IGameRepository
    {
        private readonly List<Game> _games;

        public FakeGameRepository(params Game[] games)
        {
            _games = games.ToList();
        }

        public Task AddAsync(Game game, CancellationToken cancellationToken = default)
        {
            _games.Add(game);
            return Task.CompletedTask;
        }

        public Task<Game?> GetByIdAsync(Guid gameId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Game?>(_games.FirstOrDefault(game => game.Id == gameId));
        }

        public Task<IReadOnlyCollection<Game>> ListAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<Game>>(_games);
        }

        public Task UpdateAsync(Game game, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class FakePromotionRepository : IPromotionRepository
    {
        public List<Promotion> AddedPromotions { get; } = [];

        public Task AddAsync(Promotion promotion, CancellationToken cancellationToken = default)
        {
            AddedPromotions.Add(promotion);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<Promotion>> ListAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<Promotion>>(AddedPromotions);
        }
    }
}
