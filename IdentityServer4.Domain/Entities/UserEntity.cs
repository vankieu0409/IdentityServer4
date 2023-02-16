using EF.Support.Entities.Interfaces;

using Microsoft.AspNetCore.Identity;

namespace IdentityServer4.Domain.Entities
{
    public class UserEntity: IdentityUser<Guid>,IEntity
    {
        public string PasswordSalt { get; set; }
        public string RefreshToken { get; set; } 
        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpires { get; set; }
        public bool IsDeleted { get; set; }
        public ProfileEntities Profile { get; set; }
    }
}
