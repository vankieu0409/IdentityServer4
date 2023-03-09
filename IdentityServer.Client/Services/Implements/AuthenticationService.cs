using Blazored.LocalStorage;

using IdentityServer.Client.Services.Interfaces;

using IdentityServer4.Domain.ViewModels;

using Microsoft.AspNetCore.Components.Authorization;

using System.Net.Http.Json;

namespace IdentityServer.Client.Services.Implements;

public class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly ILocalStorageService _localStorage;

    public AuthenticationService(HttpClient httpClient, AuthenticationStateProvider authStateProvider,
        ILocalStorageService localStorage)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _authStateProvider = authStateProvider ?? throw new ArgumentNullException(nameof(authStateProvider));
        _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
    }

    public async Task<HttpResponseMessage> LoginService(LoginUserViewModel viewModel)
    {
        var result = await _httpClient.PostAsJsonAsync("api/Auth/login", viewModel);
        var a = result.Content.Headers;
        var token = await result.Content.ReadAsStringAsync();
        Console.WriteLine(token);
        if (result.IsSuccessStatusCode)
        {
            await _localStorage.SetItemAsync("bearer", token);
            await _authStateProvider.GetAuthenticationStateAsync();
        }
        //else
        //{
        //    JObject obj = JObject.Parse(token);
        //    JArray passwordErrors = (JArray)obj["errors"]["Password"];
        //    JArray userNameErrors = (JArray)obj["errors"]["UserName"];

        //    foreach (string error in userNameErrors)
        //    {
        //        messErorrCollection.Add(error);
        //    }
        //    foreach (string error in passwordErrors)
        //    {
        //        messErorrCollection.Add(error);
        //    }
        //}

        return result;
    }

    public async Task<bool> RegiterService(CreateUserViewModel viewModel)
    {
        var result = await _httpClient.PostAsJsonAsync("api/Auth/register", viewModel);
        return await Task.FromResult(result.IsSuccessStatusCode);
    }

    public async Task<bool> LogoutService()
    {
        await _localStorage.ClearAsync();
        var checkTokenLogout = string.IsNullOrEmpty(await _localStorage.GetItemAsStringAsync("bearer"));
        //_authStateProvider.NotifyAuthenticationStateChangedForLogout();
        return await Task.FromResult(checkTokenLogout);
    }
}