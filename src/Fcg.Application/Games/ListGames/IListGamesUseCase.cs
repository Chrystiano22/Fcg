namespace Fcg.Application.Games.ListGames;

public interface IListGamesUseCase
{
    Task<IReadOnlyCollection<ListGamesResult>> ExecuteAsync(
        CancellationToken cancellationToken = default);
}
