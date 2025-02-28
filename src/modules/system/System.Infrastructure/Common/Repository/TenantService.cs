using Common.Domain.TransferObjects.System;
using Common.Infrastructure.System;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Application.Common.Interfaces;
using System.Domain.Features.Tenant;
using System.Infrastructure.Common.Database;
using System.Text.RegularExpressions;

namespace System.Infrastructure.Common.Repository;

internal sealed class TenantService(
    //IServiceProvider serviceProvider,
    CurrentTenant currentTenant,
    SystemDbContext systemDbContext) : ITenantService
{
    public async Task<TenantM> CreateTenant(CreateTenantDto request, CancellationToken cancellationToken = default)
    {
        string pattern = @"(?<=Database=)([^;]*)";
        string defaultConnectionString = currentTenant.GetSystemConnectionString;
        string newConnectionString = Regex.Replace(defaultConnectionString, pattern, request.DatabaseName);

        //try
        //{
        //    using IServiceScope scopeTenant = serviceProvider.CreateScope();
        //    ParentDbContext dbContext = scopeTenant.ServiceProvider.GetRequiredService<ParentDbContext>();
        //    dbContext.Database.SetConnectionString(newConnectionString);

        //    IEnumerable<string> pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken);

        //    if (pendingMigrations.Any())
        //    {
        //        Console.ForegroundColor = ConsoleColor.Blue;
        //        Console.WriteLine($"Applying ApplicationDB Migrations for New '{request.TenantName}' tenant.");
        //        Console.ResetColor();
        //        await dbContext.Database.MigrateAsync(cancellationToken);
        //    }
        //}
        //catch
        //{
        //    ArgumentNullException.ThrowIfNull(request);
        //}

        TenantM tenant = TenantM.Create
        (
            request.TenantTypeId,
            request.ParentTenantId,
            request.TenantName,
            request.DatabaseName,
            newConnectionString
        );

        await systemDbContext.AddAsync(tenant, cancellationToken);
        await systemDbContext.SaveChangesAsync(cancellationToken);

        return tenant;
    }

    public async Task<List<TenantM>> DeleteTenant(int tenantId, CancellationToken cancellationToken = default)
    {
        TenantM? tenant = await systemDbContext.Tenants.FindAsync([tenantId], cancellationToken);
        if (tenant != null)
        {
            using (SqlConnection connection = new(tenant.ConnectionString))
            {
                await connection.OpenAsync(cancellationToken);
                string sql =
                    $"""
						Use [master]
						alter database [{tenant.DatabaseName}] set single_user with rollback immediate
						DROP DATABASE [{tenant.DatabaseName}];
				""";

                await connection.ExecuteAsync(sql);
                Console.WriteLine($"Database '{sql}' dropped successfully.");
            }

            systemDbContext.Tenants.Remove(tenant);
            await systemDbContext.SaveChangesAsync(cancellationToken);

            return await systemDbContext.Tenants.ToListAsync(cancellationToken);
        }

        throw new KeyNotFoundException($"Tenant with ID {tenantId} not found.");
    }

    public async Task<List<TenantM>> GetStoreTenants(CancellationToken cancellationToken = default)
    {
        List<TenantM> tenants = await systemDbContext.Tenants.Where(_ => _.ParentTenantId != 0)
            .ToListAsync(cancellationToken);

        return tenants;
    }

    public async Task<List<TenantM>> GetParentTenants(CancellationToken cancellationToken = default)
    {
        List<TenantM> tenants = await systemDbContext.Tenants.Where(_ => _.ParentTenantId == 0)
            .ToListAsync(cancellationToken);

        return tenants;
    }

    public async Task<TenantM> GetTenant(int tenantId, CancellationToken cancellationToken = default)
    {
        TenantM? tenant = await systemDbContext.Tenants.FindAsync([tenantId], cancellationToken);
        return tenant ?? throw new KeyNotFoundException($"Tenant with ID {tenantId} not found.");
    }

    public Task<TenantM> UpdateTenant(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

