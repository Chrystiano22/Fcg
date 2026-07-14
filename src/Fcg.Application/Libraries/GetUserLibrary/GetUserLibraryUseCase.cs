using Fcg.Domain.Games;
using Fcg.Domain.Libraries;

namespace Fcg.Application.Libraries.GetUserLibrary;

public sealed class GetUserLibraryUseCase : IGetUserLibraryUseCase
{
    private readonly ILibraryItemRepository _libraryItemRepository;
    private readonly IGameRepository _gameRepository;

    public GetUserLibraryUseCase(
        ILibraryItemRepository libraryItemRepository,
        IGameRepository gameRepository)
    {
        _libraryItemRepository = libraryItemRepository;
        _gameRepository = gameRepository;
    }

    public async Task<IReadOnlyCollection<GetUserLibraryResult>> ExecuteAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var libraryItems = await _libraryItemRepository.ListByUserIdAsync(userId, cancellationToken);
        var results = new List<GetUserLibraryResult>(libraryItems.Count);

        foreach (var libraryItem in libraryItems.OrderByDescending(item => item.AcquiredAt))
        {
            var game = await _gameRepository.GetByIdAsync(libraryItem.GameId, cancellationToken);
            if (game is null)
            {
                continue;
            }

            results.Add(new GetUserLibraryResult(
                libraryItem.Id,
                game.Id,
                game.Title,
                game.Description,
                game.Price,
                game.Category,
                libraryItem.AcquiredAt));
        }

        return results;
    }
}
