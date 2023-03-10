using IdentityServer4.Data;
using IdentityServer4.Data.Repositories.Interfaces;
using IdentityServer4.Domain.Dtos;
using IdentityServer4.Domain.Entities;
using IdentityServer4.Domain.ViewModels;
using IdentityServer4.Infrastructure.Services.Interfaces;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace IdentityServer4.Infrastructure.Services.Implements;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<UserEntity> _userManager;
    private readonly RoleManager<RoleEntity> _roleManager;
    private readonly SignInManager<UserEntity> _signInManager;
    private readonly IRoleRepository _roleRepository;
    private readonly IProfileRepository _profileRepository;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(SignInManager<UserEntity> signInManager, UserManager<UserEntity> userManager,
        ApplicationDbContext context, IUserRepository userRepository, IRoleRepository roleRepository,
        IProfileRepository profileRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _profileRepository = profileRepository ?? throw new ArgumentNullException(nameof(profileRepository));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
    }

    public async Task<AccessTokenDto> Login(LoginUserViewModel viewModel)
    {
        var userEntity = await _userManager.FindByNameAsync(viewModel.UserName);

        if (userEntity == null) return new AccessTokenDto { Message = "User not found." };
        if (!await _userManager.CheckPasswordAsync(userEntity, viewModel.Password))
            return new AccessTokenDto { Message = "Wrong Password." };
        var userDto = new UserDto()
        {
            Id = userEntity.Id,
            UserName = userEntity.UserName
        };
        var token = CreateToken(userDto);
        var refreshToken = CreateRefreshToken();
        SetRefreshToken(refreshToken, userDto);

        return new AccessTokenDto
        {
            Success = true,
            Token = token,
            RefreshToken = refreshToken.Token,
            TokenExpires = refreshToken.Expires
        };
    }

    public async Task<UserDto> RegisterUser(CreateUserViewModel request)
    {
        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            UserName = request.Email,
            NormalizedUserName = request.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            Email = request.Email,
            EmailConfirmed = true,
            NormalizedEmail = request.Email,
            PhoneNumber = "",
            PhoneNumberConfirmed = false,
            LockoutEnabled = false,
            LockoutEnd = DateTimeOffset.MinValue,
            AccessFailedCount = 0,
            IsDeleted = false,
            PasswordSalt = CreatePasswordSalt(request.Password),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            TwoFactorEnabled = false
        };
        await _userManager.CreateAsync(user, request.Password);
        await _userManager.AddToRoleAsync(user, "User");


        var profileCreate = new ProfileEntities()
        {
            Id = Guid.NewGuid(),
            Name = request.FullName,
            UserId = user.Id,
            Description = "",
            Image = ""
        };

        await _profileRepository.AddAsync(profileCreate);
        await _context.SaveChangesAsync();
        var userDto = new UserDto()
        {
            Id = user.Id,
            UserName = user.UserName,
            Profile = new ProfileDto()
            {
                Id = profileCreate.Id,
                Name = profileCreate.Name,
                Description = profileCreate.Description,
                Image = profileCreate.Image
            }
        };

        return userDto;
    }

    public Task<AccessTokenDto> RefreshToken()
    {
        var userDtoCollection = new List<UserDto>();
        foreach (var userEntity in _userRepository.AsQueryable())
        {
            var userDto = new UserDto()
            {
                Id = userEntity.Id,
                UserName = userEntity.UserName
            };

            var profileEntity = _profileRepository.AsQueryable().FirstOrDefault(c => c.UserId == userEntity.Id);
            if (profileEntity != null)
            {
                var profileDto = new ProfileDto()
                {
                    Id = profileEntity.Id,
                    Name = profileEntity.Name,
                    Description = profileEntity.Description,
                    Image = profileEntity.Image
                };
                userDto.Profile = profileDto;
            }

            userDtoCollection.Add(userDto);
        }

        var refreshToken = _httpContextAccessor?.HttpContext?.Request.Cookies["refreshToken"];
        var user = userDtoCollection.FirstOrDefault(u => u.RefreshToken == refreshToken);
        if (user == null)
            return Task.FromResult(new AccessTokenDto { Message = "Invalid Refresh Token" });
        else if (user.TokenExpires < DateTime.Now)
            return Task.FromResult(new AccessTokenDto { Message = "Token expired." });

        var token = CreateToken(user);
        var newRefreshToken = CreateRefreshToken();
        SetRefreshToken(newRefreshToken, user);

        return Task.FromResult(new AccessTokenDto
        {
            Success = true,
            Token = token,
            RefreshToken = newRefreshToken.Token,
            TokenExpires = newRefreshToken.Expires
        });
    }

    public bool VerifyPasswordHash(string password, string passwordHash, string passwordSalt)
    {
        byte[] test;
        test = Convert.FromBase64String(passwordSalt);
        var hmac = new HMACSHA512(test);
        var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computeHash.SequenceEqual(Encoding.UTF8.GetBytes(passwordHash));
    }

    public string CreatePasswordSalt(string password)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(passwordBytes);
        return Convert.ToBase64String(hashBytes);
    }

    public string CreateToken(UserDto user)
    {
        //List<Claim> claims = new List<Claim>
        //{
        //    new Claim(ClaimTypes.Name, user.UserName),
        //    new Claim(ClaimTypes.Role, "Admin")
        //};

        //var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
        //    _configuration.GetSection("Jwt:Secret").Value));

        //var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        //var token = new JwtSecurityToken(
        //    claims: claims,
        //    expires: DateTime.Now.AddDays(1),
        //    signingCredentials: creds);
        var userRoles = _roleRepository.AsQueryable().FirstOrDefault(p =>
            p.Id == _context.UserRoles.Where(c => c.UserId == user.Id).Select(c => c.RoleId).FirstOrDefault());
        List<Claim> claims = new()
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Role, userRoles.NormalizedName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Secret").Value));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var token = new JwtSecurityToken(
            _configuration["Jwt:ValidIssuer"],
            _configuration["Jwt:ValidAudience"],
            claims,
            expires: DateTime.Now.AddMinutes(5),
            signingCredentials: creds);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }

    public RefreshTokenDto CreateRefreshToken()
    {
        var refreshToken = new RefreshTokenDto
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.Now.AddMinutes(5),
            Created = DateTime.Now
        };

        return refreshToken;
    }

    public async void SetRefreshToken(RefreshTokenDto refreshToken, UserDto user)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = refreshToken.Expires
        };
        _httpContextAccessor?.HttpContext?.Response
            .Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);

        user.RefreshToken = refreshToken.Token;
        user.TokenCreated = refreshToken.Created;
        user.TokenExpires = refreshToken.Expires;

        await _userRepository.SaveChangesAsync();
    }

    public void CreateRoles(RoleEntity role)
    {
        _roleManager.CreateAsync(role);
    }

    public void UpdateRoles(RoleEntity role)
    {
        _roleManager.UpdateAsync(role);
    }

    public async Task<IList<string>> GetRolesOfUser(UserEntity user)
    {
        return await _userManager.GetRolesAsync(user);
    }
}