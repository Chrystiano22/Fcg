namespace Fcg.Application.Users.ListUsers;

public interface IListUsersUseCase
{
    Task<IReadOnlyCollection<ListUsersResult>> ExecuteAsync(
        CancellationToken cancellationToken = default);
}
