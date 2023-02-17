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
using IdentityServer4.Infrastructure.Services.Implements;
using IdentityServer4.Infrastructure.Services.Interfaces;

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
            services.AddAuthorization();
            services.AddAuthentication(options => //được sử dụng để cấu hình xác thực trong ứng dụng và thiết lập chế độ xác thực và thách thức mặc định cho JWT bearer authentication.
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
                {
                    options.SaveToken = true; //Một giá trị boolean xác định liệu có nên lưu token nhận được trong vé xác thực vào AuthenticationProperties sau khi xác thực thành công hay không.
                    //options.RequireHttpsMetadata = false; // Một giá trị boolean xác định liệu middleware có yêu cầu HTTPS để truy cập điểm cuối xác thực hay không.
                    options.TokenValidationParameters = new TokenValidationParameters() //xác định các tham số được sử dụng để xác thực token JWT
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = false,// Một giá trị boolean xác định liệu nên xác thực nhà cung cấp token (issuer) hay không.
                        ValidateAudience = false,// Một giá trị boolean xác định liệu nên xác thực khán giả (audience) của token hay không.
                        ValidAudience = configuration["Jwt:ValidAudience"],//Một chuỗi giá trị xác định khán giả hợp lệ cho token.
                        ValidIssuer = configuration["Jwt:ValidIssuer"],// Một chuỗi giá trị xác định nhà cung cấp token hợp lệ cho token.
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"])),//Một đối tượng SymmetricSecurityKey chứa khóa bí mật được sử dụng để ký token.
                        RequireExpirationTime = false// Một giá trị boolean xác định liệu nên yêu cầu token có thời gian hết hạn hay không.
                    };
                });
            services.AddDbContext<ApplicationDbContext>(c => c.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddHttpContextAccessor();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddScoped<UserManager<UserEntity>>();

            services.AddTransient<IAuthService,AuthService>();
            return services;
        }
    }
}
