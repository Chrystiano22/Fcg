namespace Fcg.Domain.Libraries;

public interface ILibraryItemRepository
{
    Task<bool> ExistsAsync(
        Guid userId,
        Guid gameId,
        CancellationToken cancellationToken = default);

    Task AddAsync(LibraryItem libraryItem, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<LibraryItem>> ListByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
