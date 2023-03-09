﻿using IdentityServer4.Domain.Dtos;
using IdentityServer4.Domain.ViewModels;
using IdentityServer4.Infrastructure.Services.Interfaces;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<UserDto>> RegisterAsync(CreateUserViewModel request)
    {
        var response = await _authService.RegisterUser(request);
        return Ok(response);
    }

    [HttpGet("/signin-google")]
    public async Task<IActionResult> SignInGoogle()
    {
        var result = await HttpContext.AuthenticateAsync("External");
        var claims = result.Principal.Claims.ToList();
        // Access the user's information from the claims
        // ...
        return Ok();
    }
    // }
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<UserDto>> LoginAsync(LoginUserViewModel request)
    {
        var response = await _authService.Login(request);
        if (response.Success)
            return Ok(response);

        return BadRequest(response.Message);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<string>> RefreshToken()
    {
        var response = await _authService.RefreshToken();
        if (response.Success)
            return Ok(response);

        return BadRequest(response.Message);
    }

    [HttpGet("huhu")]
    [Authorize(Roles = "owner")]
    public ActionResult<string> NguyenActionResult()
    {
        return Ok("Aloha! You're authorized!");
    }
}