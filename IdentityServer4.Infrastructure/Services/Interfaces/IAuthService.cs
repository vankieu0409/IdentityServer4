using IdentityServer4.Domain.Dtos;
using IdentityServer4.Domain.Entities;
using IdentityServer4.Domain.ViewModels;

namespace IdentityServer4.Infrastructure.Services.Interfaces;

public interface IAuthService
{
    public Task<AccessTokenDto> Login(LoginUserViewModel request);
    public Task<UserDto> RegisterUser(CreateUserViewModel request);
    public Task<AccessTokenDto> RefreshToken();
    public bool VerifyPasswordHash(string password, string passwordHash, string passwordSalt);
    void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt);
    string CreateToken(UserDto user);
    RefreshTokenDto CreateRefreshToken();
    public void SetRefreshToken(RefreshTokenDto refreshToken, UserDto user);
}