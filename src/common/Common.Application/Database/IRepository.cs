using System.Linq.Expressions;

namespace Common.Application.Database;

public interface IRepository<TEntity> where TEntity : class
{
    // Single Entity
    TEntity? GetById(int id);
    Task<TEntity?> GetByIdAsync(int id);
    TEntity FindOne(Expression<Func<TEntity, bool>> predicate, FindOptions? findOptions = null, CancellationToken cancellationToken = default);

    // Multiple Entities
    Task<List<TEntity>> GetAllAsync();
    IEnumerable<TEntity> GetAll();

    // Creating
    void Add(TEntity entity);
    Task AddAsync(TEntity entity);

    // Updating
    void Update(TEntity entity);

    // Deleting
    void Delete(TEntity entity);
    Task DeleteById(int id);

    // Saving
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

}

public class FindOptions
{
    public bool IsIgnoreAutoIncludes { get; set; }
    public bool IsAsNoTracking { get; set; }
}
