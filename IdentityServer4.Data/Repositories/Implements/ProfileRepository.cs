using EF.Support.RepositoryAsync;

using IdentityServer4.Data.Repositories.Interfaces;
using IdentityServer4.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Data.Repositories.Implements
{
    public class ProfileRepository:RepositoryBaseAsync<ProfileEntities>,IProfileRepository
    {
        private readonly ApplicationDbContext _context;
        public ProfileRepository(ApplicationDbContext context) : base(context) 
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }
}
