namespace Fcg.Application.Games.DeleteGame;

public interface IDeleteGameUseCase
{
    Task ExecuteAsync(Guid gameId, CancellationToken cancellationToken = default);
}
