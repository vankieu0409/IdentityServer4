
using System.Reflection;
using System.Text;
using IdentityServer.Client.Services.Implements;
using IdentityServer.Client.Services.Interfaces;
using IdentityServer4.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Client.Services.Extensions.DependencyInjection
{
    public static class ServiceCollection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var entryAssembly = Assembly.GetEntryAssembly();
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            return services;
        }
    }
}
