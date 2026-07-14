using Fcg.Application.Common;
using Fcg.Application.Users.ChangeUserRole;
using Fcg.Domain.Users;

namespace Fcg.UnitTests.Users;

public sealed class ChangeUserRoleUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithExistingUser_UpdatesRole()
    {
        var user = User.Register(
            "Alice Johnson",
            "alice@example.com",
            "Secure@123",
            password => $"HASH::{password}");

        var repository = new FakeUserRepository(user);
        var useCase = new ChangeUserRoleUseCase(repository);

        var result = await useCase.ExecuteAsync(
            user.Id,
            new ChangeUserRoleCommand(UserRole.Administrator));

        Assert.Equal(UserRole.Administrator, result.Role);
        Assert.Equal(UserRole.Administrator, user.Role);
        Assert.True(repository.UpdateCalled);
    }

    [Fact]
    public async Task ExecuteAsync_WithMissingUser_ThrowsResourceNotFoundException()
    {
        var useCase = new ChangeUserRoleUseCase(new FakeUserRepository(null));

        var action = () => useCase.ExecuteAsync(
            Guid.NewGuid(),
            new ChangeUserRoleCommand(UserRole.Administrator));

        var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(action);

        Assert.Equal("User was not found.", exception.Message);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly User? _user;

        public bool UpdateCalled { get; private set; }

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
            UpdateCalled = true;
            return Task.CompletedTask;
        }
    }
}
