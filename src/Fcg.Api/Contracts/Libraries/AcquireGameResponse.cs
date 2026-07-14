using Fcg.Application.Libraries.AcquireGameForUser;

namespace Fcg.Api.Contracts.Libraries;

public sealed class AcquireGameResponse
{
    public Guid ItemId { get; init; }

    public Guid UsuarioId { get; init; }

    public Guid JogoId { get; init; }

    public string Titulo { get; init; } = string.Empty;

    public string Descricao { get; init; } = string.Empty;

    public decimal Preco { get; init; }

    public string Categoria { get; init; } = string.Empty;

    public DateTime AdquiridoEm { get; init; }

    public static AcquireGameResponse FromResult(AcquireGameForUserResult result)
    {
        return new AcquireGameResponse
        {
            ItemId = result.LibraryItemId,
            UsuarioId = result.UserId,
            JogoId = result.GameId,
            Titulo = result.Title,
            Descricao = result.Description,
            Preco = result.Price,
            Categoria = result.Category,
            AdquiridoEm = result.AcquiredAt
        };
    }
}
