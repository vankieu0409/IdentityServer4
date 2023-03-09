using IdentityServer4.Domain.ViewModels;
using System.Net.Http;

namespace IdentityServer.Client.Services.Interfaces;

public interface IAuthenticationService
{
    public Task<HttpResponseMessage> LoginService(LoginUserViewModel viewModel);
    public Task<bool> RegiterService(CreateUserViewModel viewModel);
    public Task<bool> LogoutService();
}