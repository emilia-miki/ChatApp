using Microsoft.EntityFrameworkCore;

namespace ChatApp.DAL.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected DbSet<T> Entities;
    private ChatsContext _context;

    public ChatsContext Context
    {
        get => _context;
        set
        {
            _context = value;
            Entities = _context.Set<T>();
        }
    }
    
    public GenericRepository() {}

    public GenericRepository(ChatsContext chatsContext)
    {
        _context = chatsContext;
        Entities = _context.Set<T>();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await Entities.ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await Entities.FindAsync(id);
    }

    public virtual async Task InsertAsync(T obj)
    {
        await Entities.AddAsync(obj);
    }

    public virtual void Update(T obj)
    {
        Entities.Attach(obj);
        Context.Entry(obj).State = EntityState.Modified;
    }

    public virtual void Delete(T obj)
    {
        Entities.Remove(obj);
    }
}