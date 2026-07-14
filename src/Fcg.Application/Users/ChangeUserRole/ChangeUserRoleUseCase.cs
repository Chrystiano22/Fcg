using Fcg.Application.Common;
using Fcg.Domain.Users;

namespace Fcg.Application.Users.ChangeUserRole;

public sealed class ChangeUserRoleUseCase : IChangeUserRoleUseCase
{
    private readonly IUserRepository _userRepository;

    public ChangeUserRoleUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ChangeUserRoleResult> ExecuteAsync(
        Guid userId,
        ChangeUserRoleCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            throw new ResourceNotFoundException("User was not found.");
        }

        user.ChangeRole(command.Role);

        await _userRepository.UpdateAsync(user, cancellationToken);

        return new ChangeUserRoleResult(
            user.Id,
            user.Name,
            user.Email,
            user.Role,
            user.UpdatedAt);
    }
}
