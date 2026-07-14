using Fcg.Domain.Common;
using Fcg.Domain.Games;

namespace Fcg.UnitTests.Games;

public sealed class GameTests
{
    [Fact]
    public void Create_WithValidData_CreatesActiveGame()
    {
        var game = Game.Create(
            "  Clean Architecture Adventure  ",
            "  Educational platformer for software architecture.  ",
            99.90m,
            "  Education  ");

        Assert.NotEqual(Guid.Empty, game.Id);
        Assert.Equal("Clean Architecture Adventure", game.Title);
        Assert.Equal("Educational platformer for software architecture.", game.Description);
        Assert.Equal(99.90m, game.Price);
        Assert.Equal("Education", game.Category);
        Assert.True(game.Active);
        Assert.True(game.CreatedAt <= DateTime.UtcNow);
        Assert.Equal(game.CreatedAt, game.UpdatedAt);
    }

    [Fact]
    public void Create_WithNegativePrice_ThrowsDomainValidationException()
    {
        var action = () => Game.Create(
            "Game",
            "Description",
            -1m,
            "Education");

        var exception = Assert.Throws<DomainValidationException>(action);

        Assert.Equal("Price cannot be negative.", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankTitle_ThrowsDomainValidationException(string title)
    {
        var action = () => Game.Create(
            title,
            "Description",
            10m,
            "Education");

        var exception = Assert.Throws<DomainValidationException>(action);

        Assert.Equal("Title is required.", exception.Message);
    }

    [Fact]
    public void UpdateDetails_WithValidData_UpdatesGame()
    {
        var game = Game.Create(
            "Original Title",
            "Original Description",
            10m,
            "Education");

        var previousUpdatedAt = game.UpdatedAt;

        game.UpdateDetails(
            "  Updated Title  ",
            "  Updated Description  ",
            29.90m,
            "  Strategy  ");

        Assert.Equal("Updated Title", game.Title);
        Assert.Equal("Updated Description", game.Description);
        Assert.Equal(29.90m, game.Price);
        Assert.Equal("Strategy", game.Category);
        Assert.True(game.UpdatedAt >= previousUpdatedAt);
    }

    [Fact]
    public void Deactivate_AndActivate_ChangesAvailability()
    {
        var game = Game.Create(
            "Original Title",
            "Original Description",
            10m,
            "Education");

        game.Deactivate();
        Assert.False(game.Active);

        game.Activate();
        Assert.True(game.Active);
    }
}
