using EF.Support.RepositoryAsync;

using IdentityServer4.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Data.Repositories.Interfaces
{
    public interface IRoleRepository:IRepositoryAsync<RoleEntity>
    {
    }
}
