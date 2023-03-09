using EF.Support.RepositoryAsync;
using IdentityServer4.Data.Repositories.Interfaces;
using IdentityServer4.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Data.Repositories.Implements;

public class UserRepository : RepositoryAsync<UserEntity>, IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context) : base(context, context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
}