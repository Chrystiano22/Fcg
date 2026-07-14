using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Fcg.Infrastructure.Persistence;

public sealed class FcgDbContextFactory : IDesignTimeDbContextFactory<FcgDbContext>
{
    public FcgDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FcgDbContext>();
        optionsBuilder.UseSqlite("Data Source=fcg.db");

        return new FcgDbContext(optionsBuilder.Options);
    }
}
