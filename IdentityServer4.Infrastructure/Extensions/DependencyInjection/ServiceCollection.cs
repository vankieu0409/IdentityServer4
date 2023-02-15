using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using IdentityServer4.Data;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using IdentityServer4.Data.Repositories.Implements;
using IdentityServer4.Data.Repositories.Interfaces;
using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace IdentityServer4.Infrastructure.Extensions.DependencyInjection
{
    public static class ServiceCollection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, ConfigurationManager configuration)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var entryAssembly = Assembly.GetEntryAssembly();
            services.AddAutoMapper(configuration =>
            {
                configuration.AddExpressionMapping();
            }, executingAssembly, entryAssembly);
            services.AddIdentity<UserEntity, RoleEntity>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = configuration["Jwt:ValidAudience"],
                        ValidIssuer = configuration["Jwt:ValidIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"])),
                        RequireExpirationTime = false
                    };
                });
            services.AddDbContext<ApplicationDbContext>(c => c.UseSqlServer(configuration.GetConnectionString("Default")));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IProfileRepository, ProfileRepository>();
            return services;
        }
    }
}
