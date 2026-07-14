using Fcg.Api.Contracts.Promotions;
using Fcg.Application.Promotions.CreatePromotion;
using Fcg.Application.Promotions.ListPromotions;
using Fcg.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.Api.Controllers;

[ApiController]
[Route("promocoes")]
[Authorize]
public sealed class PromotionsController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<PromotionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyCollection<PromotionResponse>>> Get(
        [FromServices] IListPromotionsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(cancellationToken);

        return Ok(result.Select(PromotionResponse.FromListResult).ToArray());
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Administrator))]
    [ProducesResponseType(typeof(PromotionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PromotionResponse>> Post(
        [FromBody] CreatePromotionRequest request,
        [FromServices] ICreatePromotionUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new CreatePromotionCommand(
                request.JogoId,
                request.Nome,
                request.Descricao,
                request.PercentualDesconto,
                request.InicioEm,
                request.FimEm),
            cancellationToken);

        return StatusCode(StatusCodes.Status201Created, PromotionResponse.FromCreateResult(result));
    }
}
