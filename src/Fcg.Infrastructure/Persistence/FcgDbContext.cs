using Fcg.Domain.Games;
using Fcg.Domain.Libraries;
using Fcg.Domain.Promotions;
using Fcg.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Infrastructure.Persistence;

public sealed class FcgDbContext : DbContext
{
    public FcgDbContext(DbContextOptions<FcgDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Game> Games => Set<Game>();

    public DbSet<LibraryItem> LibraryItems => Set<LibraryItem>();

    public DbSet<Promotion> Promotions => Set<Promotion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FcgDbContext).Assembly);
    }
}
