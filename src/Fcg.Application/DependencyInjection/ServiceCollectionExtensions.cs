using Fcg.Application.Authentication.AuthenticateUser;
using Fcg.Application.Games.CreateGame;
using Fcg.Application.Games.DeleteGame;
using Fcg.Application.Games.ListGames;
using Fcg.Application.Games.UpdateGame;
using Fcg.Application.Libraries.AcquireGameForUser;
using Fcg.Application.Libraries.GetUserLibrary;
using Fcg.Application.Promotions.CreatePromotion;
using Fcg.Application.Promotions.ListPromotions;
using Fcg.Application.Users.ChangeUserRole;
using Fcg.Application.Users.ListUsers;
using Fcg.Application.Users.RegisterUser;
using Microsoft.Extensions.DependencyInjection;

namespace Fcg.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticateUserUseCase, AuthenticateUserUseCase>();
        services.AddScoped<ICreateGameUseCase, CreateGameUseCase>();
        services.AddScoped<IDeleteGameUseCase, DeleteGameUseCase>();
        services.AddScoped<IAcquireGameForUserUseCase, AcquireGameForUserUseCase>();
        services.AddScoped<ICreatePromotionUseCase, CreatePromotionUseCase>();
        services.AddScoped<IGetUserLibraryUseCase, GetUserLibraryUseCase>();
        services.AddScoped<IListGamesUseCase, ListGamesUseCase>();
        services.AddScoped<IListPromotionsUseCase, ListPromotionsUseCase>();
        services.AddScoped<IListUsersUseCase, ListUsersUseCase>();
        services.AddScoped<IUpdateGameUseCase, UpdateGameUseCase>();
        services.AddScoped<IChangeUserRoleUseCase, ChangeUserRoleUseCase>();
        services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();

        return services;
    }
}
