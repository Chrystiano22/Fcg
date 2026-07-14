namespace Fcg.Application.Promotions.ListPromotions;

public interface IListPromotionsUseCase
{
    Task<IReadOnlyCollection<ListPromotionsResult>> ExecuteAsync(
        CancellationToken cancellationToken = default);
}
