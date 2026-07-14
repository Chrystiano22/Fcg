namespace Fcg.Application.Games.UpdateGame;

public sealed record UpdateGameResult(
    Guid GameId,
    string Title,
    string Description,
    decimal Price,
    string Category,
    bool Active,
    DateTime CreatedAt);
