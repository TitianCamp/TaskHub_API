namespace TaskHub.Data.Repositories.Interfaces;

public interface IRepository<Entity> where Entity : class
{
    Task<IEnumerable<Entity>> GetAll();
    Task<Entity?> GetById(Guid id);
    Task Add(Entity entity);
    Task Update(Entity entity);
    Task Delete(Guid id);
}
