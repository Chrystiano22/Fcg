using Fcg.Application.Security;
using Fcg.Domain.Users;

namespace Fcg.Application.Authentication.AuthenticateUser;

public sealed class AuthenticateUserUseCase : IAuthenticateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAccessTokenGenerator _accessTokenGenerator;

    public AuthenticateUserUseCase(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IAccessTokenGenerator accessTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _accessTokenGenerator = accessTokenGenerator;
    }

    public async Task<AuthenticateUserResult> ExecuteAsync(
        AuthenticateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var normalizedEmail = Email.Create(command.Email).Value;
        var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null || !_passwordHasher.Verify(command.Password, user.PasswordHash))
        {
            throw new AuthenticationFailedException("Invalid email or password.");
        }

        var accessToken = _accessTokenGenerator.Generate(user);

        return new AuthenticateUserResult(
            user.Id,
            user.Name,
            user.Email,
            user.Role,
            accessToken.Value,
            accessToken.ExpiresAtUtc);
    }
}
