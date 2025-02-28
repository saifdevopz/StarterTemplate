using Microsoft.EntityFrameworkCore;
using System.Infrastructure.Common.Database;

namespace Gateway.API.Extensions;

internal static class MigrationExtensions
{
    public static async Task ApplyAllMigrations(this IApplicationBuilder app)
    {
        await app.ApplySystemMigrations();
        await app.ApplySystemSeeder();
    }

    /// Applies system database migrations.
    public static async Task ApplySystemMigrations(this IApplicationBuilder app)
    {
        app.ApplyCustomMigration<SystemDbContext>(null);
        await Task.CompletedTask;
    }

    /// Applies seed data for the system database.
    public static async Task ApplySystemSeeder(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        DataSeeder seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
        await seeder.SeedAsync();
    }

    /// Applies migrations for a given DbContext.
    private static void ApplyCustomMigration<TDbContext>(this IApplicationBuilder app, string? connectionString)
        where TDbContext : DbContext
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        using TDbContext context = scope.ServiceProvider.GetRequiredService<TDbContext>();

        if (connectionString != null)
        {
            context.Database.SetConnectionString(connectionString);
        }

        if (context.Database.GetPendingMigrations().Any())
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Applying Migrations for '{connectionString ?? "System Database"}'.");
            Console.ResetColor();
            context.Database.Migrate();
        }
    }
}
