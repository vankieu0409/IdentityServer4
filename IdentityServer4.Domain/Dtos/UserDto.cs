using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using static System.Net.Mime.MediaTypeNames;

namespace IdentityServer4.Domain.Dtos
{
    public class UserDto
    {
        public UserDto()
        {
            Profile= new ProfileDto();
        }
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset AccessTokenExpireTime { get; set; }
        public DateTimeOffset RefreshTokenExpireTime { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public ProfileDto Profile { get; set; }
    }
}
