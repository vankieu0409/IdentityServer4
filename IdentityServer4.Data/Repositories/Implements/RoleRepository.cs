using EF.Support.RepositoryAsync;
using IdentityServer4.Data.Repositories.Interfaces;
using IdentityServer4.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Data.Repositories.Implements
{
    public class RoleRepository : RepositoryBaseAsync<RoleEntity>, IRoleRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public RoleRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
    }
}
