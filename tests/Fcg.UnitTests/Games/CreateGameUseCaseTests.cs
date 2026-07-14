using Fcg.Application.Games.CreateGame;
using Fcg.Domain.Games;

namespace Fcg.UnitTests.Games;

public sealed class CreateGameUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidCommand_PersistsGame()
    {
        var gameRepository = new FakeGameRepository();
        var useCase = new CreateGameUseCase(gameRepository);

        var result = await useCase.ExecuteAsync(new CreateGameCommand(
            "Architecture Quest",
            "Educational game about software architecture.",
            79.90m,
            "Education"));

        Assert.NotEqual(Guid.Empty, result.GameId);
        Assert.Equal("Architecture Quest", result.Title);
        Assert.Single(gameRepository.AddedGames);
        Assert.Equal("Architecture Quest", gameRepository.AddedGames.Single().Title);
    }

    private sealed class FakeGameRepository : IGameRepository
    {
        public List<Game> AddedGames { get; } = [];

        public Task AddAsync(Game game, CancellationToken cancellationToken = default)
        {
            AddedGames.Add(game);
            return Task.CompletedTask;
        }

        public Task<Game?> GetByIdAsync(Guid gameId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Game?>(AddedGames.FirstOrDefault(game => game.Id == gameId));
        }

        public Task<IReadOnlyCollection<Game>> ListAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<Game>>(AddedGames);
        }

        public Task UpdateAsync(Game game, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
