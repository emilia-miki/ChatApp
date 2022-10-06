using ChatApp.DAL.Repositories;

namespace ChatApp.DAL;

public interface IUnitOfWork
{
    ChatsContext Context { get; }
    Task CreateTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
    Task SaveAsync();
    T GetRepository<T>() where T : IRepository;
}