using Fcg.Application.Common;
using Fcg.Application.Libraries.AcquireGameForUser;
using Fcg.Domain.Common;
using Fcg.Domain.Games;
using Fcg.Domain.Libraries;
using Fcg.Domain.Users;

namespace Fcg.UnitTests.Libraries;

public sealed class AcquireGameForUserUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_AddsGameToLibrary()
    {
        var user = User.Register(
            "Alice Johnson",
            "alice@example.com",
            "Secure@123",
            password => $"HASH::{password}");
        var game = Game.Create(
            "Architecture Quest",
            "Educational game.",
            79.90m,
            "Education");

        var libraryRepository = new FakeLibraryRepository();
        var useCase = new AcquireGameForUserUseCase(
            new FakeUserRepository(user),
            new FakeGameRepository(game),
            libraryRepository);

        var result = await useCase.ExecuteAsync(user.Id, game.Id);

        Assert.Equal(user.Id, result.UserId);
        Assert.Equal(game.Id, result.GameId);
        Assert.Single(libraryRepository.AddedItems);
    }

    [Fact]
    public async Task ExecuteAsync_WithDuplicateGame_ThrowsDomainValidationException()
    {
        var user = User.Register(
            "Alice Johnson",
            "alice@example.com",
            "Secure@123",
            password => $"HASH::{password}");
        var game = Game.Create(
            "Architecture Quest",
            "Educational game.",
            79.90m,
            "Education");

        var useCase = new AcquireGameForUserUseCase(
            new FakeUserRepository(user),
            new FakeGameRepository(game),
            new FakeLibraryRepository { Exists = true });

        var action = () => useCase.ExecuteAsync(user.Id, game.Id);

        var exception = await Assert.ThrowsAsync<DomainValidationException>(action);

        Assert.Equal("Game is already in the user's library.", exception.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WithMissingUser_ThrowsResourceNotFoundException()
    {
        var game = Game.Create(
            "Architecture Quest",
            "Educational game.",
            79.90m,
            "Education");

        var useCase = new AcquireGameForUserUseCase(
            new FakeUserRepository(null),
            new FakeGameRepository(game),
            new FakeLibraryRepository());

        var action = () => useCase.ExecuteAsync(Guid.NewGuid(), game.Id);

        var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(action);

        Assert.Equal("User was not found.", exception.Message);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly User? _user;

        public FakeUserRepository(User? user)
        {
            _user = user;
        }

        public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_user?.Email == email);
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_user?.Email == email ? _user : null);
        }

        public Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_user?.Id == userId ? _user : null);
        }

        public Task<IReadOnlyCollection<User>> ListAsync(CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<User> result = _user is null ? [] : [_user];
            return Task.FromResult(result);
        }

        public Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class FakeGameRepository : IGameRepository
    {
        private readonly Game? _game;

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
            return Task.CompletedTask;
        }
    }

    private sealed class FakeLibraryRepository : ILibraryItemRepository
    {
        public bool Exists { get; init; }

        public List<LibraryItem> AddedItems { get; } = [];

        public Task<bool> ExistsAsync(Guid userId, Guid gameId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Exists);
        }

        public Task AddAsync(LibraryItem libraryItem, CancellationToken cancellationToken = default)
        {
            AddedItems.Add(libraryItem);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<LibraryItem>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<LibraryItem>>(AddedItems);
        }
    }
}
