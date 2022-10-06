using ChatApp.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.DAL.Repositories;

public class MemberChatRepository : GenericRepository<MemberChat>
{
    public MemberChatRepository() {}
    
    public MemberChatRepository(ChatsContext chatsContext) : base(chatsContext)
    {
    }

    public virtual async Task<bool> ContainsAsync(string userId, int chatId)
    {
        return await Entities.AnyAsync(mc => 
            mc.UserId == userId && mc.ChatId == chatId);
    }
}