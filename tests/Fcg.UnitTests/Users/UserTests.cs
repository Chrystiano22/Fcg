using Fcg.Domain.Common;
using Fcg.Domain.Users;

namespace Fcg.UnitTests.Users;

public sealed class UserTests
{
    [Fact]
    public void Register_WithValidData_CreatesUser()
    {
        var user = User.Register(
            "  Alice Johnson  ",
            "ALICE@Example.com",
            "Secure@123",
            password => $"HASH::{password}");

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal("Alice Johnson", user.Name);
        Assert.Equal("alice@example.com", user.Email);
        Assert.Equal("HASH::Secure@123", user.PasswordHash);
        Assert.Equal(UserRole.User, user.Role);
        Assert.False(user.IsAdministrator);
        Assert.False(user.CanManagePlatform());
        Assert.True(user.CreatedAt <= DateTime.UtcNow);
        Assert.Equal(user.CreatedAt, user.UpdatedAt);
    }

    [Fact]
    public void Register_WithAdministratorRole_AllowsPlatformManagement()
    {
        var user = User.Register(
            "Admin User",
            "admin@fcg.com",
            "Admin@123",
            password => $"HASH::{password}",
            UserRole.Administrator);

        Assert.Equal(UserRole.Administrator, user.Role);
        Assert.True(user.IsAdministrator);
        Assert.True(user.CanManagePlatform());
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("user@")]
    [InlineData("user.com")]
    public void Register_WithInvalidEmail_ThrowsDomainValidationException(string email)
    {
        var action = () => User.Register(
            "Alice Johnson",
            email,
            "Secure@123",
            password => $"HASH::{password}");

        var exception = Assert.Throws<DomainValidationException>(action);

        Assert.Equal("Email format is invalid.", exception.Message);
    }

    [Fact]
    public void Register_WithEmptyEmail_ThrowsDomainValidationException()
    {
        var action = () => User.Register(
            "Alice Johnson",
            string.Empty,
            "Secure@123",
            password => $"HASH::{password}");

        var exception = Assert.Throws<DomainValidationException>(action);

        Assert.Equal("Email is required.", exception.Message);
    }

    [Theory]
    [InlineData("short")]
    [InlineData("NoNumber!")]
    [InlineData("12345678!")]
    [InlineData("NoSpecial123")]
    public void Register_WithWeakPassword_ThrowsDomainValidationException(string password)
    {
        var action = () => User.Register(
            "Alice Johnson",
            "alice@example.com",
            password,
            rawPassword => $"HASH::{rawPassword}");

        Assert.Throws<DomainValidationException>(action);
    }

    [Fact]
    public void Register_WithBlankName_ThrowsDomainValidationException()
    {
        var action = () => User.Register(
            "   ",
            "alice@example.com",
            "Secure@123",
            password => $"HASH::{password}");

        var exception = Assert.Throws<DomainValidationException>(action);

        Assert.Equal("Name is required.", exception.Message);
    }

    [Fact]
    public void Register_WithEmptyPasswordHash_ThrowsDomainValidationException()
    {
        var action = () => User.Register(
            "Alice Johnson",
            "alice@example.com",
            "Secure@123",
            _ => string.Empty);

        var exception = Assert.Throws<DomainValidationException>(action);

        Assert.Equal("Password hash is required.", exception.Message);
    }
}
