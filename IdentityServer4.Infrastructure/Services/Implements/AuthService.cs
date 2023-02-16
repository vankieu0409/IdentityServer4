using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using IdentityServer4.Data.Repositories.Interfaces;
using IdentityServer4.Domain.Dtos;
using IdentityServer4.Domain.Entities;
using IdentityServer4.Domain.ViewModels;
using IdentityServer4.Infrastructure.Services.Interfaces;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer4.Infrastructure.Services.Implements;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IProfileRepository _profileRepository;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(IUserRepository userRepository, IRoleRepository roleRepository, IProfileRepository profileRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _profileRepository = profileRepository ?? throw new ArgumentNullException(nameof(profileRepository));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }
    public async Task<AccessTokenDto> Login(UserViewModel request)
    {
        var userEntity = await _userRepository.AsQueryable().FirstOrDefaultAsync(u => u.UserName == request.UserName);

       
        if (userEntity == null)
        {

            return new AccessTokenDto { Message = "User not found." };
        }
        var user = new UserEntity()
        {
            Id = userEntity.Id,
            UserName = userEntity.UserName,

        };
        if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            return new AccessTokenDto { Message = "Wrong Password." };
        }

        string token = CreateToken(user);
        var refreshToken = CreateRefreshToken();
        SetRefreshToken(refreshToken, user);

        return new AccessTokenDto
        {
            Success = true,
            Token = token,
            RefreshToken = refreshToken.Token,
            TokenExpires = refreshToken.Expires
        };
    }

    public async Task<UserDto> RegisterUser(UserViewModel request)
    {
        CreatePasswordHash(request.Password, out string passwordHash, out string passwordSalt);

        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            UserName = request.UserName,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };
        var profileCreate = new ProfileEntities()
        {
            Id = Guid.NewGuid(),
            Name= request.Name,
            Description= request.Description,
            Image= request.Image,
        };

        await _userRepository.AddAsync(user);
        await _profileRepository.AddAsync(profileCreate);
        var userDto = new UserDto();

        return userDto;
    }

    public async Task<AccessTokenDto> RefreshToken()
    {
        var refreshToken = _httpContextAccessor?.HttpContext?.Request.Cookies["refreshToken"];
        var user = await _userRepository.AsQueryable().FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        if (user == null)
        {
            return new AccessTokenDto { Message = "Invalid Refresh Token" };
        }
        else if (user.TokenExpires < DateTime.Now)
        {
            return new AccessTokenDto { Message = "Token expired." };
        }

        string token = CreateToken(user);
        var newRefreshToken = CreateRefreshToken();
        SetRefreshToken(newRefreshToken, user);

        return new AccessTokenDto
        {
            Success = true,
            Token = token,
            RefreshToken = newRefreshToken.Token,
            TokenExpires = newRefreshToken.Expires
        };
    }
    public bool VerifyPasswordHash(string password, string passwordHash, string passwordSalt)
    {
        var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(passwordSalt));
        var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computeHash.SequenceEqual(Encoding.UTF8.GetBytes(passwordHash));
    }

    public void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
    {
        var hmac = new HMACSHA512();
        passwordSalt = Convert.ToBase64String(hmac.Key);
        passwordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
    }

    public string CreateToken(UserEntity user)
    {
        var userRoles = _roleRepository.AsQueryable().ToList().Wher
        List<Claim> claims = new List<Claim>(){
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role,roles. )
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Secret").Value));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:ValidIssuer"],
            audience: _configuration["Jwt:ValidAudience"],
            //claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }

    public RefreshTokenDto CreateRefreshToken()
    {
        var refreshToken = new RefreshTokenDto
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.Now.AddDays(7),
            Created = DateTime.Now
        };

        return refreshToken;
    }

    public async void SetRefreshToken(RefreshTokenDto refreshToken, UserEntity user)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = refreshToken.Expires,
        };
        _httpContextAccessor?.HttpContext?.Response
            .Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);

        user.RefreshToken = refreshToken.Token;
        user.TokenCreated = refreshToken.Created;
        user.TokenExpires = refreshToken.Expires;

        await _userRepository.SaveChangesAsync();
    }
}