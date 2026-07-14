using Fcg.Application.Common;
using Fcg.Domain.Games;

namespace Fcg.Application.Games.DeleteGame;

public sealed class DeleteGameUseCase : IDeleteGameUseCase
{
    private readonly IGameRepository _gameRepository;

    public DeleteGameUseCase(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async Task ExecuteAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        var game = await _gameRepository.GetByIdAsync(gameId, cancellationToken);
        if (game is null)
        {
            throw new ResourceNotFoundException("Game was not found.");
        }

        game.Deactivate();

        await _gameRepository.UpdateAsync(game, cancellationToken);
    }
}
