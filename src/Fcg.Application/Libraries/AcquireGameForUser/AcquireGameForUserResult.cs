namespace Fcg.Application.Libraries.AcquireGameForUser;

public sealed record AcquireGameForUserResult(
    Guid LibraryItemId,
    Guid UserId,
    Guid GameId,
    string Title,
    string Description,
    decimal Price,
    string Category,
    DateTime AcquiredAt);
