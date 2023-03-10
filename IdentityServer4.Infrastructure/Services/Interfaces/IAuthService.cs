using IdentityServer4.Domain.Dtos;
using IdentityServer4.Domain.Entities;
using IdentityServer4.Domain.ViewModels;

namespace IdentityServer4.Infrastructure.Services.Interfaces;

public interface IAuthService
{
    public Task<AccessTokenDto> Login(LoginUserViewModel request);
    public Task<UserDto> RegisterUser(CreateUserViewModel request);
    public Task<AccessTokenDto> RefreshToken();
    string CreateToken(UserDto user);
    RefreshTokenDto CreateRefreshToken();
    public void SetRefreshToken(RefreshTokenDto refreshToken, UserDto user);
    public void CreateRoles(RoleEntity role);
    public void UpdateRoles(RoleEntity role);
    public Task<IList<string>> GetRolesOfUser(UserEntity user);
}