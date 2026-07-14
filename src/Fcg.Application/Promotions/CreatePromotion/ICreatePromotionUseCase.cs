namespace Fcg.Application.Promotions.CreatePromotion;

public interface ICreatePromotionUseCase
{
    Task<CreatePromotionResult> ExecuteAsync(
        CreatePromotionCommand command,
        CancellationToken cancellationToken = default);
}
