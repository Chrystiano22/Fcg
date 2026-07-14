using Fcg.Domain.Promotions;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Infrastructure.Persistence.Repositories;

public sealed class PromotionRepository : IPromotionRepository
{
    private readonly FcgDbContext _dbContext;

    public PromotionRepository(FcgDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Promotion promotion, CancellationToken cancellationToken = default)
    {
        await _dbContext.Promotions.AddAsync(promotion, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Promotion>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Promotions
            .AsNoTracking()
            .OrderBy(promotion => promotion.StartsAt)
            .ToListAsync(cancellationToken);
    }
}
