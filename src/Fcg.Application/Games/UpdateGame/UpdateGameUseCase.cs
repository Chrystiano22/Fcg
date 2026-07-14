using Fcg.Application.Common;
using Fcg.Domain.Games;

namespace Fcg.Application.Games.UpdateGame;

public sealed class UpdateGameUseCase : IUpdateGameUseCase
{
    private readonly IGameRepository _gameRepository;

    public UpdateGameUseCase(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async Task<UpdateGameResult> ExecuteAsync(
        Guid gameId,
        UpdateGameCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var game = await _gameRepository.GetByIdAsync(gameId, cancellationToken);
        if (game is null)
        {
            throw new ResourceNotFoundException("Game was not found.");
        }

        game.UpdateDetails(
            command.Title,
            command.Description,
            command.Price,
            command.Category);

        await _gameRepository.UpdateAsync(game, cancellationToken);

        return new UpdateGameResult(
            game.Id,
            game.Title,
            game.Description,
            game.Price,
            game.Category,
            game.Active,
            game.CreatedAt);
    }
}
