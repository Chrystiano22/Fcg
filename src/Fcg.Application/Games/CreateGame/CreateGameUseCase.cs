using Fcg.Domain.Games;

namespace Fcg.Application.Games.CreateGame;

public sealed class CreateGameUseCase : ICreateGameUseCase
{
    private readonly IGameRepository _gameRepository;

    public CreateGameUseCase(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async Task<CreateGameResult> ExecuteAsync(
        CreateGameCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var game = Game.Create(
            command.Title,
            command.Description,
            command.Price,
            command.Category);

        await _gameRepository.AddAsync(game, cancellationToken);

        return new CreateGameResult(
            game.Id,
            game.Title,
            game.Description,
            game.Price,
            game.Category,
            game.Active,
            game.CreatedAt);
    }
}
