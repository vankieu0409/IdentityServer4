using IdentityServer.Client.Services.Interfaces;

using IdentityServer4.Domain.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

using Newtonsoft.Json.Linq;

namespace IdentityServer.Client.Pages.Authentication;

[AllowAnonymous]
public partial class LoginRegister : ComponentBase
{
    [Inject]
    private IAuthenticationService _authenticationService { get; set; }
    [Inject]
    private NavigationManager _navigation { get; set; }
    public List<string> _messErorrCollection = new();

    /// <summary>
    [Inject]
    private IJSRuntime _js { get; set; } //Phải dùng IJSRuntime
    /// </summary>

    private LoginUserViewModel userLogin { get; set; } = new();

    private CreateUserViewModel userRegister { get; set; } = new();



    #region userLogin

    private void SetUserLoginName(ChangeEventArgs e)
    {
        userLogin.UserName = e.Value.ToString();
    }

    private void SetUserLoginPassword(ChangeEventArgs e)
    {
        userLogin.Password = e.Value.ToString();
    }

    private async Task ShowAlert(string mess)
    {
        await _js.InvokeVoidAsync("alert", mess);
    }
    private async void HandleLogin()
    {
        // ValidateFormLogin();
        var result = await _authenticationService.LoginService(userLogin);
        if (result.IsSuccessStatusCode)
        {
            //await _js.InvokeVoidAsync("alert", "Đăng nhập thành công!");
            _navigation.NavigateTo("/");
        }
        else
        {
            var obj = JObject.Parse(await result.Content.ReadAsStringAsync());
            var passwordErrors = (JArray)obj["errors"]["Password"];
            var userNameErrors = (JArray)obj["errors"]["UserName"];
            if (userNameErrors != null)
                foreach (string error in userNameErrors)
                    _messErorrCollection.Add(error);

            if (passwordErrors != null)

                foreach (string error in passwordErrors)
                    _messErorrCollection.Add(error);

            await ShowAlert(_messErorrCollection.FirstOrDefault());
            _messErorrCollection.Clear();
        }
    }

    //private void ValidateFormLogin()
    //{
    //    if (userLogin.UserName == "" || string.IsNullOrWhiteSpace(userLogin.UserName))
    //    {
    //        _js.InvokeVoidAsync("alert", "Email không được để trống");
    //    }
    //    if (!_regexEmai.IsMatch(userLogin.UserName))
    //    {
    //        _js.InvokeVoidAsync("alert", "Email không đúng định dạng!");
    //    }


    //    if (string.IsNullOrWhiteSpace(userLogin.Password) || userLogin.Password == "")
    //    {
    //        _js.InvokeVoidAsync("alert", "Password không được để trống");
    //    }
    //    if (!_regexNumber.IsMatch(userLogin.Password))
    //    {
    //        _js.InvokeVoidAsync("alert", "Password phải có ít nhất 1 chữ số");
    //    }
    //    if (userLogin.Password.Length < 8)
    //    {
    //        _js.InvokeVoidAsync("alert", "PPassword phải tù tám ký tự trở lên!");
    //    }
    //}

    #endregion

    #region UserRegister

    private void SetUserRegisterName(ChangeEventArgs e)
    {
        userRegister.FullName = e.Value.ToString();
    }

    private void SetUserRegisterEmail(ChangeEventArgs e)
    {
        userRegister.Email = e.Value.ToString();
    }

    private void SetUserRegisterPassword(ChangeEventArgs e)
    {
        userRegister.Password = e.Value.ToString();
    }
    //private void ValidateFormRegister()
    //{
    //    if (userRegister.FullName == "" || string.IsNullOrWhiteSpace(userRegister.FullName))
    //    {
    //        _js.InvokeVoidAsync("alert", "Tên không được để trống");


    //    }
    //    if (userRegister.Email == "" || string.IsNullOrWhiteSpace(userRegister.Email))
    //    {
    //        _js.InvokeVoidAsync("alert", "Email không được để trống");
    //    }
    //    if (!_regexEmai.IsMatch(userRegister.Email))
    //    {
    //        _js.InvokeVoidAsync("alert", "Email không đúng định dạng!");
    //    }

    //    if (string.IsNullOrWhiteSpace(userRegister.Password) || userRegister.Password == "")
    //    {
    //        _js.InvokeVoidAsync("alert", "Password không được để trống");
    //    }
    //    if (!_regexNumber.IsMatch(userRegister.Password))
    //    {
    //        _js.InvokeVoidAsync("alert", "Password phải có ít nhất 1 chữ số");
    //    }
    //    if (userRegister.Password.Length < 8)
    //    {
    //        _js.InvokeVoidAsync("alert", "Password phải tù tám ký tự trở lên!");
    //    }
    //}

    private async void RegisterHandle()
    {
        var isAccessed = await _authenticationService.RegiterService(userRegister);
        //await _js.InvokeVoidAsync("alert", "Đăng ký thành công");
        if (isAccessed) _navigation.NavigateTo("/login");
    }

    #endregion
}