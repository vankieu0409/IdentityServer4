using IdentityServer4.Data;
using IdentityServer4.Data.Repositories.Interfaces;
using IdentityServer4.Domain.Entities;
using IdentityServer4.Domain.ViewModels;
using IdentityServer4.Infrastructure.Extensions;
using IdentityServer4.Infrastructure.Services.Implements;
using IdentityServer4.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer4.Api;

public static class Seeder
{
    public static async Task SeedAsync(this WebApplication app)
    {
        var _service = new SeederService();
        using (var servicescope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            _service.SeederData();
        }
    }
}