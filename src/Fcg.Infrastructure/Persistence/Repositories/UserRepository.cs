using Fcg.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly FcgDbContext _dbContext;

    public UserRepository(FcgDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users.AnyAsync(user => user.Email == email, cancellationToken);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users.FirstOrDefaultAsync(user => user.Email == email, cancellationToken);
    }

    public Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users.FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<User>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .OrderBy(user => user.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
