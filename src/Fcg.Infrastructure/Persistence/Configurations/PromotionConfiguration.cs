using Fcg.Domain.Games;
using Fcg.Domain.Promotions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fcg.Infrastructure.Persistence.Configurations;

public sealed class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
{
    public void Configure(EntityTypeBuilder<Promotion> builder)
    {
        builder.ToTable("Promotions");

        builder.HasKey(promotion => promotion.Id);

        builder.Property(promotion => promotion.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(promotion => promotion.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(promotion => promotion.DiscountPercentage)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(promotion => promotion.StartsAt)
            .IsRequired();

        builder.Property(promotion => promotion.EndsAt)
            .IsRequired();

        builder.Property(promotion => promotion.Active)
            .IsRequired();

        builder.Property(promotion => promotion.CreatedAt)
            .IsRequired();

        builder.Property(promotion => promotion.UpdatedAt)
            .IsRequired();

        builder.HasIndex(promotion => promotion.GameId);

        builder.HasOne<Game>()
            .WithMany()
            .HasForeignKey(promotion => promotion.GameId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
