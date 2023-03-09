using IdentityServer.Client.Services.Implements;
using IdentityServer.Client.Services.Interfaces;

using System.Reflection;

namespace IdentityServer.Client.Services.Extensions.DependencyInjection;

public static class ServiceCollection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        var entryAssembly = Assembly.GetEntryAssembly();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        //services.AddSingleton<IJSRuntime>(provider => provider.GetRequiredService<IJSRuntime>());


        return services;
    }
}