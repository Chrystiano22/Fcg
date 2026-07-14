namespace Fcg.Domain.Games;

public interface IGameRepository
{
    Task AddAsync(Game game, CancellationToken cancellationToken = default);

    Task<Game?> GetByIdAsync(Guid gameId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Game>> ListAsync(CancellationToken cancellationToken = default);

    Task UpdateAsync(Game game, CancellationToken cancellationToken = default);
}
