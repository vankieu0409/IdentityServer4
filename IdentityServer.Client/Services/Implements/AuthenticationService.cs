using System.Net.Http.Json;

using Blazored.LocalStorage;

using IdentityServer.Client.Services.Interfaces;

using IdentityServer4.Domain.ViewModels;

using Microsoft.AspNetCore.Components.Authorization;

namespace IdentityServer.Client.Services.Implements;

public class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly ILocalStorageService _localStorage;

    public AuthenticationService(HttpClient httpClient, AuthenticationStateProvider authStateProvider, ILocalStorageService localStorage)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _authStateProvider = authStateProvider ?? throw new ArgumentNullException(nameof(authStateProvider));
        _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
    }

    public async Task<bool> LoginService(LoginUserViewModel viewModel)
    {
        var result = await _httpClient.PostAsJsonAsync("api/Auth/login", viewModel);
        var token = await result.Content.ReadAsStringAsync();
        Console.WriteLine(token);
        await _localStorage.SetItemAsync("bearer", token);
        await _authStateProvider.GetAuthenticationStateAsync();
        return await Task.FromResult(result.IsSuccessStatusCode);
    }
}