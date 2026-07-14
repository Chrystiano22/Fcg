namespace Fcg.Application.Users.ChangeUserRole;

public interface IChangeUserRoleUseCase
{
    Task<ChangeUserRoleResult> ExecuteAsync(
        Guid userId,
        ChangeUserRoleCommand command,
        CancellationToken cancellationToken = default);
}
