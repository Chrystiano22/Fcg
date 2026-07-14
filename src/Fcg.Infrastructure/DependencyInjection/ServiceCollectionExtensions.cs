using Fcg.Application.Security;
using Fcg.Domain.Games;
using Fcg.Domain.Libraries;
using Fcg.Domain.Promotions;
using Fcg.Domain.Users;
using Fcg.Infrastructure.Persistence;
using Fcg.Infrastructure.Persistence.Repositories;
using Fcg.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fcg.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var provider = configuration["Database:Provider"] ?? "Sqlite";
        var connectionString = configuration["Database:ConnectionString"];
        var jwtSettings = BuildJwtTokenSettings(configuration);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Database connection string is required.");
        }

        services.AddDbContext<FcgDbContext>(options =>
        {
            switch (provider.Trim().ToLowerInvariant())
            {
                case "sqlite":
                    options.UseSqlite(connectionString);
                    break;
                default:
                    throw new InvalidOperationException(
                        $"Database provider '{provider}' is not supported in this MVP.");
            }
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<ILibraryItemRepository, LibraryItemRepository>();
        services.AddScoped<IPromotionRepository, PromotionRepository>();
        services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton(jwtSettings);
        services.AddSingleton<IAccessTokenGenerator, JwtAccessTokenGenerator>();

        return services;
    }

    private static JwtTokenSettings BuildJwtTokenSettings(IConfiguration configuration)
    {
        var settings = new JwtTokenSettings
        {
            Issuer = configuration["Jwt:Issuer"] ?? string.Empty,
            Audience = configuration["Jwt:Audience"] ?? string.Empty,
            SecretKey = configuration["Jwt:SecretKey"] ?? string.Empty,
            ExpirationInMinutes = int.TryParse(configuration["Jwt:ExpirationInMinutes"], out var expirationInMinutes)
                ? expirationInMinutes
                : 60
        };

        if (string.IsNullOrWhiteSpace(settings.Issuer) ||
            string.IsNullOrWhiteSpace(settings.Audience) ||
            string.IsNullOrWhiteSpace(settings.SecretKey))
        {
            throw new InvalidOperationException("JWT settings are required.");
        }

        return settings;
    }
}
