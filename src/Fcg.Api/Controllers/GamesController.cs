using Fcg.Api.Contracts.Games;
using Fcg.Application.Games.CreateGame;
using Fcg.Application.Games.DeleteGame;
using Fcg.Application.Games.ListGames;
using Fcg.Application.Games.UpdateGame;
using Fcg.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.Api.Controllers;

[ApiController]
[Route("jogos")]
[Authorize]
public sealed class GamesController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<GameResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyCollection<GameResponse>>> Get(
        [FromServices] IListGamesUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(cancellationToken);

        return Ok(result.Select(GameResponse.FromListResult).ToArray());
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Administrator))]
    [ProducesResponseType(typeof(GameResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<GameResponse>> Post(
        [FromBody] CreateGameRequest request,
        [FromServices] ICreateGameUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new CreateGameCommand(
                request.Titulo,
                request.Descricao,
                request.Preco,
                request.Categoria),
            cancellationToken);

        return StatusCode(StatusCodes.Status201Created, GameResponse.FromCreateResult(result));
    }

    [HttpPut("{gameId:guid}")]
    [Authorize(Roles = nameof(UserRole.Administrator))]
    [ProducesResponseType(typeof(GameResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<GameResponse>> Put(
        Guid gameId,
        [FromBody] UpdateGameRequest request,
        [FromServices] IUpdateGameUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            gameId,
            new UpdateGameCommand(
                request.Titulo,
                request.Descricao,
                request.Preco,
                request.Categoria),
            cancellationToken);

        return Ok(GameResponse.FromUpdateResult(result));
    }

    [HttpDelete("{gameId:guid}")]
    [Authorize(Roles = nameof(UserRole.Administrator))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(
        Guid gameId,
        [FromServices] IDeleteGameUseCase useCase,
        CancellationToken cancellationToken)
    {
        await useCase.ExecuteAsync(gameId, cancellationToken);

        return NoContent();
    }
}
