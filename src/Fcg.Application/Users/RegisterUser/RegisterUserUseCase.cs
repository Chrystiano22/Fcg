using Fcg.Application.Security;
using Fcg.Domain.Common;
using Fcg.Domain.Users;

namespace Fcg.Application.Users.RegisterUser;

public sealed class RegisterUserUseCase : IRegisterUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserUseCase(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterUserResult> ExecuteAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var normalizedEmail = Email.Create(command.Email).Value;

        if (await _userRepository.EmailExistsAsync(normalizedEmail, cancellationToken))
        {
            throw new DomainValidationException("Email is already registered.");
        }

        var user = User.Register(
            command.Name,
            normalizedEmail,
            command.Password,
            _passwordHasher.Hash,
            command.Role);

        await _userRepository.AddAsync(user, cancellationToken);

        return new RegisterUserResult(
            user.Id,
            user.Name,
            user.Email,
            user.Role,
            user.CreatedAt);
    }
}
