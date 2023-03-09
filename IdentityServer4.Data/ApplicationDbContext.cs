using IdentityServer4.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.Data;

public class ApplicationDbContext : IdentityDbContext<UserEntity, RoleEntity, Guid>
{
    public ApplicationDbContext([NotNullAttribute] DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        ChangeTracker.LazyLoadingEnabled = false;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.LogTo(Console.WriteLine);
    }

    public DbSet<ProfileEntities> Profiles { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<ProfileEntities>(entity =>
        {
            entity.ToTable("Profiles");
            entity.HasKey(e => e.Id);
            entity.HasOne(entity => entity.User).WithOne(entity => entity.Profile)
                .HasForeignKey<ProfileEntities>(entity => entity.UserId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}