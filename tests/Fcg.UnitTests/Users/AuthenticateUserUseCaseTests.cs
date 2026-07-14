using Fcg.Application.Authentication.AuthenticateUser;
using Fcg.Application.Security;
using Fcg.Domain.Users;

namespace Fcg.UnitTests.Users;

public sealed class AuthenticateUserUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidCredentials_ReturnsToken()
    {
        var user = User.Register(
            "Alice Johnson",
            "alice@example.com",
            "Secure@123",
            password => $"HASH::{password}");

        var userRepository = new FakeUserRepository(user);
        var passwordHasher = new FakePasswordHasher();
        var accessTokenGenerator = new FakeAccessTokenGenerator();
        var useCase = new AuthenticateUserUseCase(userRepository, passwordHasher, accessTokenGenerator);

        var result = await useCase.ExecuteAsync(new AuthenticateUserCommand(
            "ALICE@example.com",
            "Secure@123"));

        Assert.Equal(user.Id, result.UserId);
        Assert.Equal("Alice Johnson", result.Name);
        Assert.Equal("alice@example.com", result.Email);
        Assert.Equal("TOKEN::123", result.Token);
        Assert.Equal(new DateTime(2026, 5, 4, 12, 0, 0, DateTimeKind.Utc), result.ExpiresAtUtc);
    }

    [Fact]
    public async Task ExecuteAsync_WithUnknownEmail_ThrowsAuthenticationFailedException()
    {
        var useCase = new AuthenticateUserUseCase(
            new FakeUserRepository(null),
            new FakePasswordHasher(),
            new FakeAccessTokenGenerator());

        var action = () => useCase.ExecuteAsync(new AuthenticateUserCommand(
            "alice@example.com",
            "Secure@123"));

        var exception = await Assert.ThrowsAsync<AuthenticationFailedException>(action);

        Assert.Equal("Invalid email or password.", exception.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WithWrongPassword_ThrowsAuthenticationFailedException()
    {
        var user = User.Register(
            "Alice Johnson",
            "alice@example.com",
            "Secure@123",
            password => $"HASH::{password}");

        var useCase = new AuthenticateUserUseCase(
            new FakeUserRepository(user),
            new FakePasswordHasher(),
            new FakeAccessTokenGenerator());

        var action = () => useCase.ExecuteAsync(new AuthenticateUserCommand(
            "alice@example.com",
            "Wrong@123"));

        var exception = await Assert.ThrowsAsync<AuthenticationFailedException>(action);

        Assert.Equal("Invalid email or password.", exception.Message);
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

    private sealed class FakePasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            return $"HASH::{password}";
        }

        public bool Verify(string password, string passwordHash)
        {
            return $"HASH::{password}" == passwordHash;
        }
    }

    private sealed class FakeAccessTokenGenerator : IAccessTokenGenerator
    {
        public AccessToken Generate(User user)
        {
            return new AccessToken(
                "TOKEN::123",
                new DateTime(2026, 5, 4, 12, 0, 0, DateTimeKind.Utc));
        }
    }
}
