using Fcg.Domain.Libraries;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Infrastructure.Persistence.Repositories;

public sealed class LibraryItemRepository : ILibraryItemRepository
{
    private readonly FcgDbContext _dbContext;

    public LibraryItemRepository(FcgDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsAsync(
        Guid userId,
        Guid gameId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.LibraryItems.AnyAsync(
            libraryItem => libraryItem.UserId == userId && libraryItem.GameId == gameId,
            cancellationToken);
    }

    public async Task AddAsync(LibraryItem libraryItem, CancellationToken cancellationToken = default)
    {
        await _dbContext.LibraryItems.AddAsync(libraryItem, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<LibraryItem>> ListByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.LibraryItems
            .Where(libraryItem => libraryItem.UserId == userId)
            .OrderBy(libraryItem => libraryItem.AcquiredAt)
            .ToListAsync(cancellationToken);
    }
}
