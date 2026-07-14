using Fcg.Domain.Games;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fcg.Infrastructure.Persistence.Configurations;

public sealed class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.ToTable("Games");

        builder.HasKey(game => game.Id);

        builder.Property(game => game.Title)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(game => game.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(game => game.Category)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(game => game.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(game => game.Active)
            .IsRequired();

        builder.Property(game => game.CreatedAt)
            .IsRequired();

        builder.Property(game => game.UpdatedAt)
            .IsRequired();
    }
}
