namespace TaskHub.Data.Repositories.Interfaces;

public interface IRepository<TEntity> where TEntity : class
{
    Task<IReadOnlyList<TEntity>> GetAll();
    Task<TEntity?> GetById(Guid id);
    Task Add(TEntity entity);
    Task Update(TEntity entity);
    Task Delete(Guid id);
}
