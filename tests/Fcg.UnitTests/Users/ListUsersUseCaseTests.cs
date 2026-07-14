using Fcg.Application.Users.ListUsers;
using Fcg.Domain.Users;

namespace Fcg.UnitTests.Users;

public sealed class ListUsersUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ReturnsUsersOrderedByName()
    {
        var users = new[]
        {
            User.Register("Zoe", "zoe@example.com", "Secure@123", password => $"HASH::{password}"),
            User.Register("Alice", "alice@example.com", "Secure@123", password => $"HASH::{password}")
        };

        var useCase = new ListUsersUseCase(new FakeUserRepository(users));

        var result = await useCase.ExecuteAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("Alice", result.First().Name);
        Assert.Equal("Zoe", result.Last().Name);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly IReadOnlyCollection<User> _users;

        public FakeUserRepository(IReadOnlyCollection<User> users)
        {
            _users = users;
        }

        public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_users.Any(user => user.Email == email));
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_users.FirstOrDefault(user => user.Email == email));
        }

        public Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_users.FirstOrDefault(user => user.Id == userId));
        }

        public Task<IReadOnlyCollection<User>> ListAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_users);
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
}
