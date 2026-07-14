using Fcg.Domain.Users;

namespace Fcg.Application.Users.ListUsers;

public sealed class ListUsersUseCase : IListUsersUseCase
{
    private readonly IUserRepository _userRepository;

    public ListUsersUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyCollection<ListUsersResult>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.ListAsync(cancellationToken);

        return users
            .OrderBy(user => user.Name)
            .Select(user => new ListUsersResult(
                user.Id,
                user.Name,
                user.Email,
                user.Role,
                user.CreatedAt))
            .ToArray();
    }
}
