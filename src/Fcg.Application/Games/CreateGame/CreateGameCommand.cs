namespace Fcg.Application.Games.CreateGame;

public sealed record CreateGameCommand(
    string Title,
    string Description,
    decimal Price,
    string Category);
