using Microsoft.EntityFrameworkCore;
using System.API.Models;

namespace System.API.Infrastructure.Database;

internal sealed class SystemDbContext(DbContextOptions<SystemDbContext> options) : DbContext(options)
{
    internal DbSet<TenantM> Tenants => Set<TenantM>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder);
    }
}

