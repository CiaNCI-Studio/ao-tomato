namespace AoTomato.API.DependencyInjection;

using AoTomato.API.Helpers;
using AoTomato.domain.Login.Models;
using AoTomato.Domain.Common.Models;
using AoTomato.Domain.FunctionLogs.Abstractions.Repositories;
using AoTomato.Domain.FunctionLogs.Abstractions.Services;
using AoTomato.Domain.Functions.Abstractions.Repositories;
using AoTomato.Domain.Functions.Abstractions.Services;
using AoTomato.Domain.Login.Abstractions.Services;
using AoTomato.Domain.Settings.Abstractions.Repositories;
using AoTomato.Domain.Settings.Abstractions.Services;
using AoTomato.Domain.Users.Abstractions.Repositories;
using AoTomato.Domain.Users.Abstractions.Services;
using AoTomato.Domain.Variables.Abstractions.Repositories;
using AoTomato.Domain.Variables.Abstractions.Services;
using AoTomato.Repositories.FunctionLogs;
using AoTomato.Repositories.Functions;
using AoTomato.Repositories.Settings;
using AoTomato.Repositories.Users;
using AoTomato.Repositories.Variables;
using AoTomato.Services.FunctionLogs;
using AoTomato.Services.Functions;
using AoTomato.Services.Login;
using AoTomato.Services.Settings;
using AoTomato.Services.Users;
using AoTomato.Services.Variables;

public static class ServicesExtensions
{
     public static IServiceCollection SetServiceProviderHelper(this IServiceCollection services)
    {
        ServiceProviderFactory.SetServiceProvider(services.BuildServiceProvider());
        return services;
    }
    
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IFunctionLogsService, FunctionLogsService>();
        services.AddScoped<IFunctionsService, FunctionsService>();
        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<ISettingsService, SettingsService>();
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IVariablesService, VariableService>();
    }
    
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IFunctionLogssRepository, FunctionLogsRepository>();
        services.AddScoped<IFunctionsRepository, FunctionsRepository>();
        services.AddScoped<ISettingsRepository, SettingsRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IVariablessRepository, VariableRepository>();
    }

    public static void AddSettings(this IServiceCollection services)
    {
        services.AddScoped(s =>
        {
            var settingsService = s.GetRequiredService<ISettingsService>();
            return settingsService.GetByModelAsync<AuthSettings>().GetAwaiter().GetResult() ?? new AuthSettings();
        });
    }

    public static IServiceCollection AddMappers(this IServiceCollection services)
        {
            return services
                .AddAutoMapper(typeof(Domain.Users.Mapping.Mapper))
                .AddAutoMapper(typeof(Domain.Functions.Mapping.Mapper))
                .AddAutoMapper(typeof(Domain.FunctionLogs.Mapping.Mapper))
                .AddAutoMapper(typeof(Domain.Settings.Mapping.Mapper))
                .AddAutoMapper(typeof(Domain.Variables.Mapping.Mapper));
        }

    public static WebApplicationBuilder AddConfiguration<T>(this WebApplicationBuilder builder)
        where T : class, new()
    {
        if (builder.Configuration.GetSection(typeof(T).Name).Value == null || builder.Configuration.GetSection(typeof(T).Name).GetChildren().Count() == 0)
        {
            builder.Services.AddSingleton(s =>new DbSettings());
        }
        else
        {
            var settings = new DbSettings();
            builder.Configuration.Bind(typeof(DbSettings).Name, settings);
            builder.Services.AddSingleton(settings);
        }
        return builder;
    }

}