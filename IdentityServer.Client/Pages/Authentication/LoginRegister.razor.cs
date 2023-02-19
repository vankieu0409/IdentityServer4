using IdentityServer.Client.Services.Interfaces;
using IdentityServer4.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IdentityServer.Client.Pages.Authentication;
[AllowAnonymous]
public partial class LoginRegister:ComponentBase
{
    [Inject] private IAuthenticationService _authenticationService { get; set; }
    [Inject] private NavigationManager _navigation { get; set; }

    protected LoginUserViewModel userLogin { get; set; } = new LoginUserViewModel();
    protected CreateUserViewModel user { get; set; } = new CreateUserViewModel();
    protected override void OnInitialized()
    {
        base.OnInitialized();
    }

    private void SetUserLoginName(ChangeEventArgs e)
    {
        userLogin.UserName = e.Value.ToString();
    }
    private void SetUserLoginPassword(ChangeEventArgs e)
    {
        userLogin.Password= e.Value.ToString();
    }

    private async void HandleLogin()
    {
        var isAccessed=await _authenticationService.LoginService(userLogin);
        if (isAccessed) _navigation.NavigateTo("/loading");
    }
}