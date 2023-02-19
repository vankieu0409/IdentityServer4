using IdentityServer4.Domain.ViewModels;
using System.Net.Http;

namespace IdentityServer.Client.Services.Interfaces;

public interface IAuthenticationService
{
    public Task<bool> LoginService(LoginUserViewModel viewModel);

}