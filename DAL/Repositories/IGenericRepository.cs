namespace ChatApp.DAL.Repositories;

public interface IGenericRepository<T> : IRepository where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task InsertAsync(T obj);
    void Update(T obj);
    void Delete(T obj);
}