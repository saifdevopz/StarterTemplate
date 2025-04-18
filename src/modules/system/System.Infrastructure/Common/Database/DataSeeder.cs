using Microsoft.EntityFrameworkCore;
using System.Domain.Features.Tenant;
using System.Domain.Identity;

namespace System.Infrastructure.Common.Database;

public class DataSeeder(SystemDbContext context)
{
    private readonly SystemDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task SeedAsync()
    {
        await SeedTenantTypesAsync();
        await SeedTenantsAsync();
        await SeedUsersAsync();
        await SeedTenantUsersAsync();
        await SeedRolesAsync();
        await SeedUserRolesAsync();
        await SeedPermissionsAsync();
        await SeedRolePermissionsAsync();
    }

    private async Task SeedTenantTypesAsync()
    {
        if (await _context.TenantTypes.AnyAsync())
        {
            return;
        }

        TenantTypeM[] tenantTypes =
        [
            TenantTypeM.Create("SA", "Super Admin"),
            TenantTypeM.Create("P", "Parent"),
            TenantTypeM.Create("C", "Child1")
        ];

        await _context.TenantTypes.AddRangeAsync(tenantTypes);
        await _context.SaveChangesAsync();
    }

    private async Task SeedTenantsAsync()
    {
        if (await _context.Tenants.AnyAsync())
        {
            return;
        }

        TenantM[] tenants =
        [
            TenantM.Create(1, 0, "Web-Master", "Web-Master", GetSQLServerDatabaseConnectionString("Web-Master")),
            TenantM.Create(2, 0, "Customer 1", "Customer1-P", GetPostreSQLDatabaseConnectionString("Customer1-P")),
            TenantM.Create(2, 0, "Customer 2", "Customer2-P", GetPostreSQLDatabaseConnectionString("Customer2-P"))
        ];

        await _context.Tenants.AddRangeAsync(tenants);
        await _context.SaveChangesAsync();
    }

    private async Task SeedUsersAsync()
    {
        if (await _context.Users.AnyAsync())
        {
            return;
        }

        UserM[] users =
        [
            UserM.Create("admin@gmail.com", "12345678"),
            UserM.Create("customer1@gmail.com", "12345678"),
            UserM.Create("customer2@gmail.com", "12345678")
        ];

        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();
    }

    private async Task SeedTenantUsersAsync()
    {
        if (await _context.TenantUsers.AnyAsync())
        {
            return;
        }

        TenantUsersM[] tenantUsers =
        [
            new TenantUsersM { TenantId = 1, UserId = 1 },
            new TenantUsersM { TenantId = 2, UserId = 2 },
            new TenantUsersM { TenantId = 3, UserId = 3 }
        ];

        await _context.TenantUsers.AddRangeAsync(tenantUsers);
        await _context.SaveChangesAsync();
    }

    private async Task SeedRolesAsync()
    {
        if (await _context.Roles.AnyAsync())
        {
            return;
        }

        RoleM[] roles =
        [
            RoleM.Create("Admin"),
            RoleM.Create("Customer")
        ];

        await _context.Roles.AddRangeAsync(roles);
        await _context.SaveChangesAsync();
    }

    private async Task SeedUserRolesAsync()
    {
        if (await _context.UserRoles.AnyAsync())
        {
            return;
        }

        UserRoleM[] userRoles =
        [
            new UserRoleM { UserId = 1, RoleId = 1 },
            new UserRoleM { UserId = 2, RoleId = 2 },
            new UserRoleM { UserId = 3, RoleId = 2 }
        ];

        await _context.UserRoles.AddRangeAsync(userRoles);
        await _context.SaveChangesAsync();
    }

    private async Task SeedPermissionsAsync()
    {
        if (await _context.Permissions.AnyAsync())
        {
            return;
        }

        PermissionM[] permissions =
        [
            PermissionM.Create("system:global"),
            PermissionM.Create("parent:global"),
            PermissionM.Create("child1:global")
        ];

        await _context.Permissions.AddRangeAsync(permissions);
        await _context.SaveChangesAsync();
    }

    private async Task SeedRolePermissionsAsync()
    {
        if (await _context.RolePermissions.AnyAsync())
        {
            return;
        }

        RolePermissionM[] rolePermissions =
        [
            new RolePermissionM { RoleId = 1, PermissionId = 1 },
            new RolePermissionM { RoleId = 1, PermissionId = 2 },
            new RolePermissionM { RoleId = 1, PermissionId = 3 },
            new RolePermissionM { RoleId = 2, PermissionId = 2 },
            new RolePermissionM { RoleId = 2, PermissionId = 3 }
        ];

        await _context.RolePermissions.AddRangeAsync(rolePermissions);
        await _context.SaveChangesAsync();
    }

    private static string GetPostreSQLDatabaseConnectionString(string databaseName)
    {
        return $"Host=102.211.206.231;Port=5432;Database={databaseName};Username=sa;Password=25122000SK;Pooling=true;MinPoolSize=10;MaxPoolSize=100;Include Error Detail=true";
    }

    private static string GetSQLServerDatabaseConnectionString(string databaseName)
    {
        return $"Server=102.211.206.231,1433;Database={databaseName};User Id=sa;Password=25122000Saif;Encrypt=False;Trusted_Connection=True;TrustServerCertificate=True;Integrated Security=False";
    }
}