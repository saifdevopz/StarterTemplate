using System.Data.Common;

namespace Common.Application.Database;

public interface IDbConnectionFactory
{
    Task<T> QueryFirstOrDefaultAsync<T>(string sql, object parameters = null!);
    Task<T> QuerySingleOrDefaultAsync<T>(string sql, object parameters = null!);
    Task<List<T>> QueryAsync<T>(string sql, object parameters = null!, bool systemDb = false);
    Task<int> ExecuteAsync(string sql, object parameters = null!);
    DbConnection GetConnection(bool systemDb = false);
    ValueTask<DbConnection> OpenSystemConnection();
    ValueTask<DbConnection> OpenPostgreSQLConnection(string? connectionString = null);
    ValueTask<DbConnection> OpenSQLServerConnection(string? connectionString = null);
}
