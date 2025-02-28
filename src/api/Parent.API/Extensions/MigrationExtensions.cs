using Common.Infrastructure.System;
using Microsoft.EntityFrameworkCore;
using Parent.Infrastructure.Common.Database;

namespace Parent.API.Extensions;

internal static class MigrationExtensions
{
    public static async Task ApplyAllMigrations(this IApplicationBuilder app)
    {
        await ApplyParentMigrations(app);
    }

    public static async Task ApplyParentMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        ISystemService systemService = scope.ServiceProvider.GetRequiredService<ISystemService>();

        List<TenantDto> tenants = await systemService.GetParentTenants();

        await ApplyCustomTenantMigrations<ParentDbContext>(app, tenants);
    }

    private static async Task ApplyCustomTenantMigrations<TDbContext>(
        this IApplicationBuilder app,
        List<TenantDto> tenants)
        where TDbContext : DbContext
    {
        foreach (TenantDto tenant in tenants)
        {
            ApplyCustomMigration<TDbContext>(app, tenant.ConnectionString!);
        }

        await Task.CompletedTask;
    }

    private static void ApplyCustomMigration<TDbContext>(this IApplicationBuilder app, string? connectionString)
        where TDbContext : DbContext
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        using TDbContext context = scope.ServiceProvider.GetRequiredService<TDbContext>();

        try
        {
            if (connectionString != null)
            {
                context.Database.SetConnectionString(connectionString);
            }

            List<string> pendingMigrations = context.Database.GetPendingMigrations().ToList();
            if (pendingMigrations.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"Applying Migrations for '{connectionString ?? "System Database"}'.");
                Console.ResetColor();
                context.Database.Migrate();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error applying migrations for '{connectionString ?? "System Database"}': {ex.Message}");
            Console.ResetColor();
        }
    }

}

