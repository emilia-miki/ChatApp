using ChatApp.DAL.Entities;
using ChatApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.DAL.Repositories;

public class MessageRepository : GenericRepository<Message>
{
    public MessageRepository() {}
    
    public MessageRepository(ChatsContext chatsContext) : base(chatsContext)
    {
    }
    
    public virtual async Task<IEnumerable<MessageView>> GetMessagesAsync(
        string userId, int chatId, int skip, int batchSize)
    {
        return await Entities
            .Include(m => m.User)
            .Include(m => m.MessageDeletedForUsers)
            .Where(m => m.ChatId == chatId 
                        && !m.MessageDeletedForUsers.Any(
                            mdfu => mdfu.UserId == userId))
            .OrderByDescending(m => m.DateTime)
            .Skip(skip)
            .Take(batchSize)
            .Select(m => new MessageView
            {
                Id = m.Id,
                UserName = m.User.UserName,
                Text = m.Text,
                ReplyTo = m.ReplyTo,
                DateTime = m.DateTime
            }).ToListAsync();
    }
}