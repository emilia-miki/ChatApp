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

    public IEnumerable<T> GetAll()
    {
        return Entities.ToList();
    }

    public T? GetById(int id)
    {
        return Entities.Find(id);
    }

    public void Insert(T obj)
    {
        Entities.Add(obj);
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