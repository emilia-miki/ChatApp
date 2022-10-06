using ChatApp.DAL.Entities;
using ChatApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.DAL.Repositories;

public class ChatRepository : GenericRepository<Chat>
{
    public ChatRepository() {}
    
    public ChatRepository(ChatsContext chatsContext) : base(chatsContext)
    {
    }

    public virtual async Task<Chat?> GetByNameAsync(string name)
    {
        return await Entities.SingleOrDefaultAsync(c => c.Name == name);
    }
    
    public virtual async Task<IEnumerable<ChatView>> GetGroupsAsync(string userId, 
        int skip, int batchSize, string sortBy, bool sortDesc)
    {
        return await Entities
            .Include(c => c.MembersChats)
            .Include(c => c.Messages)
            .Where(c => !c.IsPersonal && c.MembersChats.Any(
                mc => mc.UserId == userId))
            .Select(c => new ChatView 
                {
                    Name = c.Name,
                    LatestMessageDateTime = c.Messages
                        .Select(m => m.DateTime)
                        .Max()
                }
            )
            .OrderBy(sortBy, sortDesc)
            .Skip(skip)
            .Take(batchSize)
            .ToListAsync();
    }
}