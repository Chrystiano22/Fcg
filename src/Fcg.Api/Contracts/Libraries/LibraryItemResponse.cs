using Fcg.Application.Libraries.GetUserLibrary;

namespace Fcg.Api.Contracts.Libraries;

public sealed class LibraryItemResponse
{
    public Guid ItemId { get; init; }

    public Guid JogoId { get; init; }

    public string Titulo { get; init; } = string.Empty;

    public string Descricao { get; init; } = string.Empty;

    public decimal Preco { get; init; }

    public string Categoria { get; init; } = string.Empty;

    public DateTime AdquiridoEm { get; init; }

    public static LibraryItemResponse FromResult(GetUserLibraryResult result)
    {
        return new LibraryItemResponse
        {
            ItemId = result.LibraryItemId,
            JogoId = result.GameId,
            Titulo = result.Title,
            Descricao = result.Description,
            Preco = result.Price,
            Categoria = result.Category,
            AdquiridoEm = result.AcquiredAt
        };
    }
}
