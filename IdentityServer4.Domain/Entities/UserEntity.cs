using EF.Support.Entities.Interfaces;

using Microsoft.AspNetCore.Identity;

namespace IdentityServer4.Domain.Entities
{
    public class UserEntity: IdentityUser<Guid>,IEntity
    {
        public string PasswordSalt { get; set; }
        public bool IsDeleted { get; set; }
        public ProfileEntities Profile { get; set; }
    }
}
