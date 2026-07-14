using Fcg.Domain.Games;
using Fcg.Domain.Libraries;
using Fcg.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fcg.Infrastructure.Persistence.Configurations;

public sealed class LibraryItemConfiguration : IEntityTypeConfiguration<LibraryItem>
{
    public void Configure(EntityTypeBuilder<LibraryItem> builder)
    {
        builder.ToTable("LibraryItems");

        builder.HasKey(libraryItem => libraryItem.Id);

        builder.Property(libraryItem => libraryItem.UserId)
            .IsRequired();

        builder.Property(libraryItem => libraryItem.GameId)
            .IsRequired();

        builder.Property(libraryItem => libraryItem.AcquiredAt)
            .IsRequired();

        builder.HasIndex(libraryItem => new { libraryItem.UserId, libraryItem.GameId })
            .IsUnique();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(libraryItem => libraryItem.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Game>()
            .WithMany()
            .HasForeignKey(libraryItem => libraryItem.GameId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
