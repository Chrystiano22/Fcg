using Fcg.Application.Security;
using Fcg.Application.Users.RegisterUser;
using Fcg.Domain.Common;
using Fcg.Domain.Users;

namespace Fcg.UnitTests.Users;

public sealed class RegisterUserUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidCommand_PersistsUserAndReturnsResult()
    {
        var userRepository = new FakeUserRepository();
        var passwordHasher = new FakePasswordHasher();
        var useCase = new RegisterUserUseCase(userRepository, passwordHasher);

        var result = await useCase.ExecuteAsync(new RegisterUserCommand(
            "  Alice Johnson  ",
            "ALICE@Example.com",
            "Secure@123"));

        Assert.NotEqual(Guid.Empty, result.UserId);
        Assert.Equal("Alice Johnson", result.Name);
        Assert.Equal("alice@example.com", result.Email);
        Assert.Equal(UserRole.User, result.Role);
        Assert.Single(userRepository.AddedUsers);
        Assert.Equal("alice@example.com", userRepository.CheckedEmails.Single());
        Assert.Equal("HASH::Secure@123", userRepository.AddedUsers.Single().PasswordHash);
        Assert.Equal("Secure@123", passwordHasher.HashedPasswords.Single());
    }

    [Fact]
    public async Task ExecuteAsync_WhenEmailAlreadyExists_ThrowsDomainValidationException()
    {
        var userRepository = new FakeUserRepository { ExistingEmails = ["alice@example.com"] };
        var useCase = new RegisterUserUseCase(userRepository, new FakePasswordHasher());

        var action = () => useCase.ExecuteAsync(new RegisterUserCommand(
            "Alice Johnson",
            "ALICE@Example.com",
            "Secure@123"));

        var exception = await Assert.ThrowsAsync<DomainValidationException>(action);

        Assert.Equal("Email is already registered.", exception.Message);
        Assert.Empty(userRepository.AddedUsers);
    }

    [Fact]
    public async Task ExecuteAsync_WithNullCommand_ThrowsArgumentNullException()
    {
        var useCase = new RegisterUserUseCase(new FakeUserRepository(), new FakePasswordHasher());

        await Assert.ThrowsAsync<ArgumentNullException>(() => useCase.ExecuteAsync(null!));
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        public HashSet<string> ExistingEmails { get; init; } = [];
        public List<string> CheckedEmails { get; } = [];
        public List<User> AddedUsers { get; } = [];

        public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            CheckedEmails.Add(email);
            return Task.FromResult(ExistingEmails.Contains(email));
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<User?>(null);
        }

        public Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<User?>(null);
        }

        public Task<IReadOnlyCollection<User>> ListAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<User>>(AddedUsers);
        }

        public Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            AddedUsers.Add(user);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class FakePasswordHasher : IPasswordHasher
    {
        public List<string> HashedPasswords { get; } = [];

        public string Hash(string password)
        {
            HashedPasswords.Add(password);
            return $"HASH::{password}";
        }

        public bool Verify(string password, string passwordHash)
        {
            return $"HASH::{password}" == passwordHash;
        }
    }
}
