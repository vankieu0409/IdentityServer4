using IdentityServer4.Data.Repositories.Interfaces;
using IdentityServer4.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer4.Infrastructure.Extensions;

public class SeederService
{
    private readonly RoleManager<RoleEntity> _role;
    private readonly UserManager<UserEntity> _user;
    private readonly IProfileRepository _profile;

    public SeederService()
    {
    }

    public SeederService(RoleManager<RoleEntity> role, UserManager<UserEntity> user, IProfileRepository profile)
    {
        _role = role ?? throw new ArgumentNullException(nameof(role));
        _user = user ?? throw new ArgumentNullException(nameof(user));
        _profile = profile ?? throw new ArgumentNullException(nameof(profile));
    }

    public async void SeederData()
    {
        var roleCollection = new List<RoleEntity>()
        {
            new()
            {
                Id = Guid.Parse("ab251560-a455-40fd-adfd-54f9e150f874"), Name = "Administrator",
                ConcurrencyStamp = Guid.NewGuid().ToString(), NormalizedName = "Administrator"
            },
            new()
            {
                Id = Guid.Parse("8d4b836e-d9fa-4fa9-88c0-9a875d2b7d5c"), Name = "Owner",
                ConcurrencyStamp = Guid.NewGuid().ToString(), NormalizedName = "Owner"
            },
            new()
            {
                Id = Guid.Parse("f91ec0e5-d768-42e2-8926-de7d3162430f"), Name = "User",
                ConcurrencyStamp = Guid.NewGuid().ToString(), NormalizedName = "User"
            }
        };


        var newUser = new UserEntity()
        {
            Id = Guid.Parse("fb1eab16-920e-4480-b3ee-01f6e9c15ab5"),
            UserName = "vankieu0409@gmail.com",
            NormalizedUserName = "vankieiu0409@gmail.com",
            SecurityStamp = Guid.NewGuid().ToString(),
            Email = "vankieu0409@gmail.com",
            EmailConfirmed = true,
            NormalizedEmail = "vankieu0409@gmail.com",
            PhoneNumber = "",
            PhoneNumberConfirmed = false,
            LockoutEnabled = false,
            LockoutEnd = DateTimeOffset.MinValue,
            AccessFailedCount = 0,
            IsDeleted = false,
            PasswordSalt = "",
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            TwoFactorEnabled = false
        };
        var profile1 = new ProfileEntities(Guid.NewGuid(), "Kiều", "No Comment",
            "https://media-cdn-v2.laodong.vn/Storage/NewsPortal/2022/8/18/1082204/Leesuk.jpg",
            Guid.Parse("fb1eab16-920e-4480-b3ee-01f6e9c15ab5"));
        var newUser2 = new UserEntity()
        {
            Id = Guid.Parse("6aa93c41-f21f-44e3-8f46-7d76b03574c5"),
            UserName = "kieunvph14806@fpt.edu.vn",
            NormalizedUserName = "kieunvph14806@fpt.edu.vn",
            SecurityStamp = Guid.NewGuid().ToString(),
            Email = "kieunvph14806@fpt.edu.vn",
            EmailConfirmed = true,
            NormalizedEmail = "kieunvph14806@fpt.edu.vn",
            PhoneNumber = "",
            PhoneNumberConfirmed = false,
            LockoutEnabled = false,
            LockoutEnd = DateTimeOffset.MinValue,
            AccessFailedCount = 0,
            IsDeleted = false,
            PasswordSalt = "",
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            TwoFactorEnabled = false
        };
        var profile2 = new ProfileEntities(Guid.NewGuid(), "Kiều", "No Comment",
            "https://media-cdn-v2.laodong.vn/Storage/NewsPortal/2022/8/18/1082204/Leesuk.jpg",
            Guid.Parse("fb1eab16-920e-4480-b3ee-01f6e9c15ab5"));
        await _user.CreateAsync(newUser, "phithitrang");
        await _profile.AddAsync(profile1);

        await _user.CreateAsync(newUser2, "phithitrang");
        await _profile.AddAsync(profile2);

        foreach (var role in roleCollection) await _role.CreateAsync(role);
        await _user.AddToRolesAsync(newUser, roleCollection.Select(c => c.Name));
        await _user.AddToRoleAsync(newUser2, "User");
    }
}