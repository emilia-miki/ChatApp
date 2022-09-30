using Microsoft.EntityFrameworkCore.Storage;
using ChatApp.DAL.Repositories;

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

    public async Task CreateTransactionAsync()
    {
        _objTran = await Context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        await _objTran.CommitAsync();
    }

    public async Task RollbackAsync()
    {
        await _objTran.RollbackAsync();
        await _objTran.DisposeAsync();
    }

    public async Task SaveAsync()
    {
        await Context.SaveChangesAsync();
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