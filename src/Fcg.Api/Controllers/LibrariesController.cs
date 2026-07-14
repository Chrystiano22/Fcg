using System.Security.Claims;
using Fcg.Api.Contracts.Libraries;
using Fcg.Application.Libraries.AcquireGameForUser;
using Fcg.Application.Libraries.GetUserLibrary;
using Fcg.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.Api.Controllers;

[ApiController]
public sealed class LibrariesController : ControllerBase
{
    [Authorize]
    [HttpGet("biblioteca")]
    [ProducesResponseType(typeof(IReadOnlyCollection<LibraryItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyCollection<LibraryItemResponse>>> Get(
        [FromServices] IGetUserLibraryUseCase useCase,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out var parsedUserId))
        {
            return Unauthorized();
        }

        var result = await useCase.ExecuteAsync(parsedUserId, cancellationToken);

        return Ok(result.Select(LibraryItemResponse.FromResult).ToArray());
    }

    [Authorize(Roles = nameof(UserRole.Administrator))]
    [HttpPost("usuarios/{userId:guid}/biblioteca/{gameId:guid}")]
    [ProducesResponseType(typeof(AcquireGameResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<AcquireGameResponse>> Post(
        Guid userId,
        Guid gameId,
        [FromServices] IAcquireGameForUserUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(userId, gameId, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, AcquireGameResponse.FromResult(result));
    }
}
