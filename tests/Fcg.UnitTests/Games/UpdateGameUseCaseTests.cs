using Fcg.Application.Common;
using Fcg.Application.Games.UpdateGame;
using Fcg.Domain.Games;

namespace Fcg.UnitTests.Games;

public sealed class UpdateGameUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithExistingGame_UpdatesGame()
    {
        var existingGame = Game.Create(
            "Architecture Quest",
            "Educational game.",
            79.90m,
            "Education");

        var gameRepository = new FakeGameRepository(existingGame);
        var useCase = new UpdateGameUseCase(gameRepository);

        var result = await useCase.ExecuteAsync(existingGame.Id, new UpdateGameCommand(
            "Architecture Quest Reloaded",
            "Updated educational game.",
            89.90m,
            "Strategy"));

        Assert.Equal(existingGame.Id, result.GameId);
        Assert.Equal("Architecture Quest Reloaded", result.Title);
        Assert.Equal("Strategy", existingGame.Category);
        Assert.True(gameRepository.UpdateCalled);
    }

    [Fact]
    public async Task ExecuteAsync_WithMissingGame_ThrowsResourceNotFoundException()
    {
        var useCase = new UpdateGameUseCase(new FakeGameRepository(null));

        var action = () => useCase.ExecuteAsync(Guid.NewGuid(), new UpdateGameCommand(
            "Architecture Quest Reloaded",
            "Updated educational game.",
            89.90m,
            "Strategy"));

        var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(action);

        Assert.Equal("Game was not found.", exception.Message);
    }

    private sealed class FakeGameRepository : IGameRepository
    {
        private readonly Game? _game;

        public bool UpdateCalled { get; private set; }

        public FakeGameRepository(Game? game)
        {
            _game = game;
        }

        public Task AddAsync(Game game, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<Game?> GetByIdAsync(Guid gameId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_game?.Id == gameId ? _game : null);
        }

        public Task<IReadOnlyCollection<Game>> ListAsync(CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<Game> result = _game is null ? [] : [_game];
            return Task.FromResult(result);
        }

        public Task UpdateAsync(Game game, CancellationToken cancellationToken = default)
        {
            UpdateCalled = true;
            return Task.CompletedTask;
        }
    }
}
