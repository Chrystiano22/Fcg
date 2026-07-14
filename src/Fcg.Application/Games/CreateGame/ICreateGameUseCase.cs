namespace Fcg.Application.Games.CreateGame;

public interface ICreateGameUseCase
{
    Task<CreateGameResult> ExecuteAsync(
        CreateGameCommand command,
        CancellationToken cancellationToken = default);
}
