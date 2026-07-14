namespace Fcg.Application.Users.RegisterUser;

public interface IRegisterUserUseCase
{
    Task<RegisterUserResult> ExecuteAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken = default);
}
