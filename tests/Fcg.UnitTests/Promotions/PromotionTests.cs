using Fcg.Domain.Common;
using Fcg.Domain.Promotions;

namespace Fcg.UnitTests.Promotions;

public sealed class PromotionTests
{
    [Fact]
    public void Create_WithValidData_CreatesPromotion()
    {
        var startsAt = DateTime.UtcNow.AddDays(-1);
        var endsAt = DateTime.UtcNow.AddDays(5);

        var promotion = Promotion.Create(
            Guid.NewGuid(),
            "Semana Tech",
            "Descontos especiais para jogos educacionais.",
            20,
            startsAt,
            endsAt);

        Assert.NotEqual(Guid.Empty, promotion.Id);
        Assert.Equal("Semana Tech", promotion.Name);
        Assert.Equal(20, promotion.DiscountPercentage);
        Assert.True(promotion.Active);
    }

    [Fact]
    public void Create_WithInvalidDiscount_ThrowsException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => Promotion.Create(
            Guid.NewGuid(),
            "Semana Tech",
            "Descontos especiais para jogos educacionais.",
            0,
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddDays(5)));

        Assert.Equal("Discount percentage must be greater than 0 and less than or equal to 100.", exception.Message);
    }

    [Fact]
    public void Create_WithInvalidDateRange_ThrowsException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => Promotion.Create(
            Guid.NewGuid(),
            "Semana Tech",
            "Descontos especiais para jogos educacionais.",
            20,
            DateTime.UtcNow.AddDays(2),
            DateTime.UtcNow.AddDays(1)));

        Assert.Equal("Promotion end date must be greater than the start date.", exception.Message);
    }
}
