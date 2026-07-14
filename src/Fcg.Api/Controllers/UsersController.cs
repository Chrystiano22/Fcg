using System.Security.Claims;
using Fcg.Api.Contracts.Users;
using Fcg.Application.Users.ChangeUserRole;
using Fcg.Application.Users.ListUsers;
using Fcg.Application.Users.RegisterUser;
using Fcg.Domain.Common;
using Fcg.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.Api.Controllers;

[ApiController]
[Route("usuarios")]
public sealed class UsersController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(RegisterUserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RegisterUserResponse>> Post(
        [FromBody] RegisterUserRequest request,
        [FromServices] IRegisterUserUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new RegisterUserCommand(request.Nome, request.Email, request.Senha),
            cancellationToken);

        return StatusCode(StatusCodes.Status201Created, RegisterUserResponse.FromResult(result));
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(CurrentUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<CurrentUserResponse> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var name = User.FindFirstValue(ClaimTypes.Name);
        var email = User.FindFirstValue(ClaimTypes.Email);
        var role = User.FindFirstValue(ClaimTypes.Role);

        if (!Guid.TryParse(userId, out var parsedUserId) ||
            string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(role))
        {
            return Unauthorized();
        }

        return Ok(new CurrentUserResponse
        {
            Id = parsedUserId,
            Nome = name,
            Email = email,
            Perfil = role
        });
    }

    [Authorize(Roles = nameof(UserRole.Administrator))]
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<AdminUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyCollection<AdminUserResponse>>> Get(
        [FromServices] IListUsersUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(cancellationToken);

        return Ok(result.Select(AdminUserResponse.FromListResult).ToArray());
    }

    [Authorize(Roles = nameof(UserRole.Administrator))]
    [HttpPatch("{userId:guid}/perfil")]
    [ProducesResponseType(typeof(AdminUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<AdminUserResponse>> PatchRole(
        Guid userId,
        [FromBody] ChangeUserRoleRequest request,
        [FromServices] IChangeUserRoleUseCase useCase,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<UserRole>(request.Perfil, ignoreCase: true, out var parsedRole))
        {
            throw new DomainValidationException("Role is invalid.");
        }

        var result = await useCase.ExecuteAsync(
            userId,
            new ChangeUserRoleCommand(parsedRole),
            cancellationToken);

        return Ok(AdminUserResponse.FromChangeRoleResult(result));
    }
}
