using ChatApp.DAL.Entities;
using ChatApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.DAL.Repositories;

public class ChatRepository : GenericRepository<Chat>
{
    public ChatRepository(ChatsContext chatsContext) : base(chatsContext)
    {
    }

    public Chat? GetByName(string name)
    {
        return Entities.SingleOrDefault(c => c.Name == name);
    }
    
    public IEnumerable<ChatView> GetGroups(string userId, 
        int skip, int batchSize, string sortBy, bool sortDesc)
    {
        return Entities
            .Include(c => c.MembersChats)
            .Where(c => !c.IsPersonal && c.MembersChats.Any(
                mc => mc.UserId == userId))
            .Select(c => new ChatView
            {
                Name = c.Name
            })
            .OrderBy(sortBy, sortDesc)
            .Skip(skip)
            .Take(batchSize)
            .ToList();
    }
}