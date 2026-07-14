namespace Fcg.Application.Authentication.AuthenticateUser;

public interface IAuthenticateUserUseCase
{
    Task<AuthenticateUserResult> ExecuteAsync(
        AuthenticateUserCommand command,
        CancellationToken cancellationToken = default);
}
