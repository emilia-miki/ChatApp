using System.Globalization;
using ChatApp.DAL.Entities;

namespace ChatApp.DAL.Repositories;

public class MessageRepository : GenericRepository<Message>
{
    public MessageRepository(ChatsContext chatsContext) : base(chatsContext)
    {
    }
    
    public DateTime? GetLatestMessageTime(string userId)
    {
        var result = Entities
            .Where(m => m.UserId == userId)
            .Select(m => (DateTime?) m.DateTime)
            .DefaultIfEmpty()
            .Max();

        return result;
    }
}