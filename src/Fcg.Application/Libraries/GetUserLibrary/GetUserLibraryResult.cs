namespace Fcg.Application.Libraries.GetUserLibrary;

public sealed record GetUserLibraryResult(
    Guid LibraryItemId,
    Guid GameId,
    string Title,
    string Description,
    decimal Price,
    string Category,
    DateTime AcquiredAt);
