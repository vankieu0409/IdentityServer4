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
            Profile = new ProfileDto();
        }
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Descreption { get; set; }
        public string Image { get; set; }
        public string RefreshToken { get; set; } = String.Empty;
        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpires { get; set; }
        public string Role { get; set; }
        public ProfileDto Profile { get; set; }
    }
}
