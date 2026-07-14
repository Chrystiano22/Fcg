using Fcg.Domain.Common;

namespace Fcg.Domain.Libraries;

public sealed class LibraryItem
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid GameId { get; private set; }
    public DateTime AcquiredAt { get; private set; }

    private LibraryItem()
    {
    }

    private LibraryItem(Guid id, Guid userId, Guid gameId, DateTime acquiredAt)
    {
        Id = id;
        UserId = userId;
        GameId = gameId;
        AcquiredAt = acquiredAt;
    }

    public static LibraryItem Acquire(
        Guid userId,
        Guid gameId,
        DateTime? acquiredAt = null)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainValidationException("User id is required.");
        }

        if (gameId == Guid.Empty)
        {
            throw new DomainValidationException("Game id is required.");
        }

        return new LibraryItem(
            Guid.NewGuid(),
            userId,
            gameId,
            acquiredAt ?? DateTime.UtcNow);
    }
}
