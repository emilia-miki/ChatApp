using ChatApp.BLL.Interfaces;
using ChatApp.DAL;
using ChatApp.DAL.Entities;
using ChatApp.DAL.Repositories;

namespace ChatApp.BLL.Services;

public class ChatService : IChatService
{
    private readonly IUnitOfWork _unitOfWork;

    public ChatService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task CreatePersonalChatIfNotExistsAsync(
        string userName1, string userName2)
    {
        if (string.CompareOrdinal(userName1, userName2) > 0)
        {
            (userName1, userName2) = (userName2, userName1);
        }

        var chatName = $"{userName1} and {userName2} Personal Chat";
        
        var chatRepository = _unitOfWork.GetRepository<ChatRepository>();
        var memberChatRepository = _unitOfWork
            .GetRepository<MemberChatRepository>();
        var userRepository = _unitOfWork.GetRepository<UserRepository>();
        
        var chat = await chatRepository.GetByNameAsync(chatName);
        if (chat != null)
        {
            return;
        }
        
        var user1 = await userRepository.GetByLoginAsync(userName1);
        if (user1 == null)
        {
            return;
        }
        var userId1 = user1.Id!;
        
        var user2 = await userRepository.GetByLoginAsync(userName2);
        if (user2 == null)
        {
            return;
        }
        var userId2 = user2.Id!;

        chat = new Chat
        {
            Name = chatName,
            IsPersonal = true
        };
        await _unitOfWork.CreateTransactionAsync();
        await chatRepository.InsertAsync(chat);
        await _unitOfWork.CommitAsync();
        await _unitOfWork.SaveAsync();

        await _unitOfWork.CreateTransactionAsync();
        await memberChatRepository.InsertAsync(new MemberChat
        {
            UserId = userId1,
            ChatId = chat.Id
        });
        
        await memberChatRepository.InsertAsync(new MemberChat
        {
            UserId = userId2,
            ChatId = chat.Id
        });

        await _unitOfWork.CommitAsync();
        await _unitOfWork.SaveAsync();
    }
}