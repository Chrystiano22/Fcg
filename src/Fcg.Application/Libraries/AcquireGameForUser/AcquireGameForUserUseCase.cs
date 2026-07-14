using Fcg.Application.Common;
using Fcg.Domain.Common;
using Fcg.Domain.Games;
using Fcg.Domain.Libraries;
using Fcg.Domain.Users;

namespace Fcg.Application.Libraries.AcquireGameForUser;

public sealed class AcquireGameForUserUseCase : IAcquireGameForUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IGameRepository _gameRepository;
    private readonly ILibraryItemRepository _libraryItemRepository;

    public AcquireGameForUserUseCase(
        IUserRepository userRepository,
        IGameRepository gameRepository,
        ILibraryItemRepository libraryItemRepository)
    {
        _userRepository = userRepository;
        _gameRepository = gameRepository;
        _libraryItemRepository = libraryItemRepository;
    }

    public async Task<AcquireGameForUserResult> ExecuteAsync(
        Guid userId,
        Guid gameId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            throw new ResourceNotFoundException("User was not found.");
        }

        var game = await _gameRepository.GetByIdAsync(gameId, cancellationToken);
        if (game is null)
        {
            throw new ResourceNotFoundException("Game was not found.");
        }

        if (!game.Active)
        {
            throw new DomainValidationException("Only active games can be added to the library.");
        }

        var alreadyOwned = await _libraryItemRepository.ExistsAsync(userId, gameId, cancellationToken);
        if (alreadyOwned)
        {
            throw new DomainValidationException("Game is already in the user's library.");
        }

        var libraryItem = LibraryItem.Acquire(userId, gameId);

        await _libraryItemRepository.AddAsync(libraryItem, cancellationToken);

        return new AcquireGameForUserResult(
            libraryItem.Id,
            userId,
            gameId,
            game.Title,
            game.Description,
            game.Price,
            game.Category,
            libraryItem.AcquiredAt);
    }
}
