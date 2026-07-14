using Fcg.Domain.Common;
using Fcg.Domain.Libraries;

namespace Fcg.UnitTests.Libraries;

public sealed class LibraryItemTests
{
    [Fact]
    public void Acquire_WithValidData_CreatesLibraryItem()
    {
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();

        var libraryItem = LibraryItem.Acquire(userId, gameId);

        Assert.NotEqual(Guid.Empty, libraryItem.Id);
        Assert.Equal(userId, libraryItem.UserId);
        Assert.Equal(gameId, libraryItem.GameId);
        Assert.True(libraryItem.AcquiredAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Acquire_WithProvidedDate_UsesProvidedDate()
    {
        var acquiredAt = new DateTime(2026, 5, 3, 12, 0, 0, DateTimeKind.Utc);

        var libraryItem = LibraryItem.Acquire(Guid.NewGuid(), Guid.NewGuid(), acquiredAt);

        Assert.Equal(acquiredAt, libraryItem.AcquiredAt);
    }

    [Fact]
    public void Acquire_WithEmptyUserId_ThrowsDomainValidationException()
    {
        var action = () => LibraryItem.Acquire(Guid.Empty, Guid.NewGuid());

        var exception = Assert.Throws<DomainValidationException>(action);

        Assert.Equal("User id is required.", exception.Message);
    }

    [Fact]
    public void Acquire_WithEmptyGameId_ThrowsDomainValidationException()
    {
        var action = () => LibraryItem.Acquire(Guid.NewGuid(), Guid.Empty);

        var exception = Assert.Throws<DomainValidationException>(action);

        Assert.Equal("Game id is required.", exception.Message);
    }
}
