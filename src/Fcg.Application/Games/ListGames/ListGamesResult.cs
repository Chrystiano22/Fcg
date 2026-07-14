namespace Fcg.Application.Games.ListGames;

public sealed record ListGamesResult(
    Guid GameId,
    string Title,
    string Description,
    decimal Price,
    string Category,
    bool Active,
    DateTime CreatedAt);
