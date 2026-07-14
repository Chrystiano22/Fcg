using Fcg.Application.Games.CreateGame;
using Fcg.Application.Games.ListGames;
using Fcg.Application.Games.UpdateGame;

namespace Fcg.Api.Contracts.Games;

public sealed class GameResponse
{
    public Guid Id { get; init; }

    public string Titulo { get; init; } = string.Empty;

    public string Descricao { get; init; } = string.Empty;

    public decimal Preco { get; init; }

    public string Categoria { get; init; } = string.Empty;

    public bool Ativo { get; init; }

    public DateTime CriadoEm { get; init; }

    public static GameResponse FromCreateResult(CreateGameResult result)
    {
        return new GameResponse
        {
            Id = result.GameId,
            Titulo = result.Title,
            Descricao = result.Description,
            Preco = result.Price,
            Categoria = result.Category,
            Ativo = result.Active,
            CriadoEm = result.CreatedAt
        };
    }

    public static GameResponse FromListResult(ListGamesResult result)
    {
        return new GameResponse
        {
            Id = result.GameId,
            Titulo = result.Title,
            Descricao = result.Description,
            Preco = result.Price,
            Categoria = result.Category,
            Ativo = result.Active,
            CriadoEm = result.CreatedAt
        };
    }

    public static GameResponse FromUpdateResult(UpdateGameResult result)
    {
        return new GameResponse
        {
            Id = result.GameId,
            Titulo = result.Title,
            Descricao = result.Description,
            Preco = result.Price,
            Categoria = result.Category,
            Ativo = result.Active,
            CriadoEm = result.CreatedAt
        };
    }
}
