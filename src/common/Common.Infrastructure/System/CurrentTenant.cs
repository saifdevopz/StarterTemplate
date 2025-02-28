using Common.Infrastructure.Authentication;
using Common.Infrastructure.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace Common.Infrastructure.System;

public sealed class CurrentTenant(IHttpContextAccessor httpContextAccessor, IConfiguration config)
{
    private readonly string _parentConnectionString = config.GetValueOrThrow<string>("PostgreSQL:DefaultConnection");
    private readonly string _storeConnectionString = config.GetValueOrThrow<string>("PostgreSQL:DefaultConnection");
    public string GetSystemConnectionString { get; } = config.GetValueOrThrow<string>("SQLServer:DefaultConnection");
    public string? TenantId => httpContextAccessor.HttpContext?.Request.Headers["Tenant"];
    public string? TenantDbName => httpContextAccessor.HttpContext?.User.GetTenantDbName();

    public string GetParentConnectionString()
    {
        if (string.IsNullOrWhiteSpace(TenantDbName))
        {
            return _parentConnectionString;
        }

        string pattern = @"(?<=Database=)([^;]*)";
        string newConnectionString = Regex.Replace(_parentConnectionString, pattern, TenantDbName);

        return newConnectionString;
    }

    public string GetStoreConnectionString()
    {
        if (string.IsNullOrWhiteSpace(TenantDbName))
        {
            return _storeConnectionString;
        }

        string pattern = @"(?<=Database=)([^;]*)";
        string newConnectionString = Regex.Replace(_storeConnectionString, pattern, TenantDbName);

        return newConnectionString;
    }

}


