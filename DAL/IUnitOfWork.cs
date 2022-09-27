using ChatApp.DAL.Repositories;

namespace ChatApp.DAL;

public interface IUnitOfWork
{
    ChatsContext Context { get; }
    void CreateTransaction();
    void Commit();
    void Rollback();
    void Save();
    T GetRepository<T>() where T : IRepository;
}