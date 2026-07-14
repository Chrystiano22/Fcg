namespace Fcg.Application.Libraries.GetUserLibrary;

public interface IGetUserLibraryUseCase
{
    Task<IReadOnlyCollection<GetUserLibraryResult>> ExecuteAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
