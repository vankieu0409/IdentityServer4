using EF.Support.Entities.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Domain.Entities;

public class RoleEntity : IdentityRole<Guid>, IEntity
{
}