using Fcg.Domain.Games;

namespace Fcg.Application.Games.ListGames;

public sealed class ListGamesUseCase : IListGamesUseCase
{
    private readonly IGameRepository _gameRepository;

    public ListGamesUseCase(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async Task<IReadOnlyCollection<ListGamesResult>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        var games = await _gameRepository.ListAsync(cancellationToken);

        return games
            .Where(game => game.Active)
            .OrderBy(game => game.Title)
            .Select(game => new ListGamesResult(
                game.Id,
                game.Title,
                game.Description,
                game.Price,
                game.Category,
                game.Active,
                game.CreatedAt))
            .ToArray();
    }
}
