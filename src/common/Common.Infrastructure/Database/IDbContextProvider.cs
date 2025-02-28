using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure.Database;

public interface IDbContextProvider
{
    DbContext GetContext();
}

public class DbContextProvider(IServiceProvider serviceProvider, string assemblyName) : IDbContextProvider
{
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    private readonly string _assemblyName = assemblyName ?? throw new ArgumentNullException(nameof(assemblyName));

    public DbContext GetContext()
    {
        // Dynamically split the assembly name and remove any part after the first `.`
        string rootAssemblyName = _assemblyName.Split('.')[0];

        string dbContextTypeName = $"{rootAssemblyName}DbContext";

        Type? dbContextType = AppDomain.CurrentDomain
            .GetAssemblies() // Get all loaded assemblies
            .SelectMany(a => a.GetTypes()) // Get all types in each assembly
            .FirstOrDefault(t => t.Name == dbContextTypeName && typeof(DbContext).IsAssignableFrom(t));

        return dbContextType == null
            ? throw new InvalidOperationException("Unable to determine the calling assembly.")
            : (DbContext)_serviceProvider.GetRequiredService(dbContextType);
    }

}