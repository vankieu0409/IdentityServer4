using IdentityServer4.Api;
using IdentityServer4.Infrastructure.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddHealthChecks();
builder.Services.AddCors(options => options.AddPolicy("Cors",
    policy => { policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }
));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHealthChecks("/health");

app.UseCors("Cors");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();
//app.SeedAsync().ConfigureAwait(false).GetAwaiter();
//app.MapControllerRoute(
//name: "account",
//pattern: "{controller=Account}/{action=SignIn}/{id?}"
//);

app.MapControllers();

app.Run();