using Common.Application.Database;
using Common.Infrastructure.System;
using Dapper;
using Microsoft.Data.SqlClient;
using Npgsql;
using System.Data.Common;

namespace Common.Infrastructure.Database;

internal sealed class DbConnectionFactory(CurrentTenant ct) : IDbConnectionFactory
{
    public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object parameters = null!)
    {
        using DbConnection connection = GetConnection(true);
        await connection.OpenAsync();

        Task<T> result = connection.QueryFirstOrDefaultAsync<T>(sql, parameters)!;
        return await result!;
    }

    public async Task<List<T>> QueryAsync<T>(string sql, object parameters = null!, bool systemDb = false)
    {
        using DbConnection connection = GetConnection(systemDb);
        await connection.OpenAsync();

        IEnumerable<T> result = await connection.QueryAsync<T>(sql, parameters);
        return result.ToList();
    }

    public async Task<int> ExecuteAsync(string sql, object parameters = null!)
    {
        using DbConnection connection = GetConnection();
        await connection.OpenAsync();

        return await connection.ExecuteAsync(sql, parameters);
    }

    public async Task<T> QuerySingleOrDefaultAsync<T>(string sql, object parameters = null!)
    {
        using DbConnection connection = GetConnection();
        await connection.OpenAsync();

        Task<T> result = connection.QuerySingleOrDefaultAsync<T>(sql, parameters)!;
        return await result;
    }

    public DbConnection GetConnection(bool systemDb = false)
    {
        return systemDb
            ? new SqlConnection(ct.GetSystemConnectionString)
            : new SqlConnection(ct.GetStoreConnectionString());
    }

    public async ValueTask<DbConnection> OpenSystemConnection()
    {
        SqlConnection connection = new(ct.GetSystemConnectionString);
        await connection.OpenAsync();

        return connection;
    }

    public async ValueTask<DbConnection> OpenPostgreSQLConnection(string? connectionString = null)
    {
        NpgsqlConnection connection = !string.IsNullOrEmpty(connectionString) ?
            new NpgsqlConnection(connectionString) :
            new NpgsqlConnection(ct.GetParentConnectionString());

        await connection.OpenAsync();

        return connection;
    }

    public async ValueTask<DbConnection> OpenSQLServerConnection(string? connectionString = null)
    {
        SqlConnection connection = !string.IsNullOrEmpty(connectionString) ?
            new SqlConnection(connectionString) :
            new SqlConnection(ct.GetStoreConnectionString());

        await connection.OpenAsync();
        return connection;
    }
}
