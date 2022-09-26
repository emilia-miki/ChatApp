using Microsoft.EntityFrameworkCore;

namespace ChatApp.DAL;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private DbSet<T> _entities;
    private ChatsContext _context;

    public ChatsContext Context
    {
        get => _context;
        set
        {
            _context = value;
            _entities = _context.Set<T>();
        }
    }

    public GenericRepository(ChatsContext chatsContext)
    {
        _context = chatsContext;
        _entities = _context.Set<T>();
    }

    public IEnumerable<T> GetAll()
    {
        return _entities.ToList();
    }

    public T? GetById(int id)
    {
        return _entities.Find(id);
    }

    public void Insert(T obj)
    {
        _entities.Add(obj);
    }

    public void Update(T obj)
    {
        _entities.Attach(obj);
        Context.Entry(obj).State = EntityState.Modified;
    }

    public void Delete(T obj)
    {
        _entities.Remove(obj);
    }
}