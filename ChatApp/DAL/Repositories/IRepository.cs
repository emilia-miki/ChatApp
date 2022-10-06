namespace ChatApp.DAL.Repositories;

public interface IRepository
{
    public ChatsContext Context { get; set; }
}