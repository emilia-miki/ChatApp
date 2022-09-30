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

    public GenericRepository(ChatsContext chatsContext)
    {
        _context = chatsContext;
        Entities = _context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await Entities.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await Entities.FindAsync(id);
    }

    public async Task InsertAsync(T obj)
    {
        await Entities.AddAsync(obj);
    }

    public void Update(T obj)
    {
        Entities.Attach(obj);
        Context.Entry(obj).State = EntityState.Modified;
    }

    public void Delete(T obj)
    {
        Entities.Remove(obj);
    }
}