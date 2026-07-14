using Fcg.Api.Contracts.Auth;
using Fcg.Application.Authentication.AuthenticateUser;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.Api.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequest request,
        [FromServices] IAuthenticateUserUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new AuthenticateUserCommand(request.Email, request.Senha),
            cancellationToken);

        return Ok(LoginResponse.FromResult(result));
    }
}
