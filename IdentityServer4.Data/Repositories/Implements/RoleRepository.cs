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
    public class RoleRepository : RepositoryAsync<RoleEntity>, IRoleRepository
    {
        private readonly ApplicationDbContext _context;
        public RoleRepository(ApplicationDbContext context) : base(context,context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }
}
