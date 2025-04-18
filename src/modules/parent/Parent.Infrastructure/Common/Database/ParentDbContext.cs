using Common.Infrastructure.Inbox;
using Common.Infrastructure.Interceptors;
using Common.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Parent.Application.Common;
using Parent.Domain.Inventory.CategoryGroup;
using Parent.Domain.TestEntities;
using System.Reflection;

namespace Parent.Infrastructure.Common.Database;

public sealed class ParentDbContext(DbContextOptions<ParentDbContext> options) : DbContext(options), IUnitOfWork
{
    internal DbSet<CategoryGroupM> CategoryGroups => Set<CategoryGroupM>();
    public DbSet<Habit> Habits { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<HabitTag> HabitTags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.HasDefaultSchema(Constants.Schema);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder);

        optionsBuilder.AddInterceptors(new AuditableEntityInterceptor());
    }
}