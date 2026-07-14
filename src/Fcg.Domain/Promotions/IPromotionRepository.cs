namespace Fcg.Domain.Promotions;

public interface IPromotionRepository
{
    Task AddAsync(Promotion promotion, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Promotion>> ListAsync(CancellationToken cancellationToken = default);
}
