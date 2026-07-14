using Fcg.Domain.Games;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Infrastructure.Persistence.Repositories;

public sealed class GameRepository : IGameRepository
{
    private readonly FcgDbContext _dbContext;

    public GameRepository(FcgDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Game game, CancellationToken cancellationToken = default)
    {
        await _dbContext.Games.AddAsync(game, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Game?> GetByIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Games.FirstOrDefaultAsync(game => game.Id == gameId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Game>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Games
            .AsNoTracking()
            .OrderBy(game => game.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Game game, CancellationToken cancellationToken = default)
    {
        _dbContext.Games.Update(game);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
