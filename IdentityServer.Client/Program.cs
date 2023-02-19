using Blazored.LocalStorage;
using IdentityServer.Client;
using IdentityServer.Client.Pages.Authentication;
using IdentityServer.Client.Services.Authentication;
using IdentityServer.Client.Services.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddApplication();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:5000")});
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();
await builder.Build().RunAsync();
