using Microsoft.EntityFrameworkCore.Storage;

namespace ChatApp.DAL;

public class UnitOfWork : IUnitOfWork
{
    private IDbContextTransaction _objTran = null!;
    private Dictionary<string, object>? _repositories;
    private readonly IServiceProvider _serviceProvider;

    public ChatsContext Context { get; }

    public UnitOfWork(
        IServiceProvider serviceProvider, 
        ChatsContext chatsContext)
    {
        _serviceProvider = serviceProvider;
        Context = chatsContext;
    }

    public void CreateTransaction()
    {
        _objTran = Context.Database.BeginTransaction();
    }

    public void Commit()
    {
        _objTran.Commit();
    }

    public void Rollback()
    {
        _objTran.Rollback();
        _objTran.Dispose();
    }

    public void Save()
    {
        Context.SaveChanges();
    }

    public T GetRepository<T>() where T : IRepository
    {
        _repositories ??= new Dictionary<string, object>();

        var type = typeof(T).Name;

        if (_repositories.ContainsKey(type))
        {
            return (T) _repositories[type];
        }
        
        var instance = (T) _serviceProvider.GetService(typeof(T))!;
        instance.Context = Context;
        _repositories.Add(type, instance);
        return (T) _repositories[type];
    }
}