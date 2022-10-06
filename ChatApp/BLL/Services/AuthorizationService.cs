using ChatApp.BLL.Interfaces;
using ChatApp.DAL;
using ChatApp.DAL.Entities;
using ChatApp.DAL.Repositories;

namespace ChatApp.BLL.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly IUnitOfWork _unitOfWork;

    public AuthorizationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<bool> IsUserInChatAsync(string userName, string chatName)
    {
        var userRepository = _unitOfWork.GetRepository<UserRepository>();
        var user = await userRepository.GetByLoginAsync(userName);
        if (user == null)
        {
            return false;
        }
        var userId = user.Id!;
        
        var chatRepository = _unitOfWork.GetRepository<ChatRepository>();
        var chat = await chatRepository.GetByNameAsync(chatName);
        if (chat == null)
        {
            return false;
        }
        var chatId = chat!.Id;

        var memberChatRepository = _unitOfWork
            .GetRepository<MemberChatRepository>();
        return await memberChatRepository.ContainsAsync(userId, chatId);
    }

    public async Task<bool> IsMessageByUserAsync(int messageId, string userName)
    {
        var messageRepository = _unitOfWork.GetRepository<MessageRepository>();
        var userRepository = _unitOfWork.GetRepository<UserRepository>();
            
        var message = await messageRepository.GetByIdAsync(messageId);
        if (message == null)
        {
            return false;
        }
        
        var user = await userRepository.GetByLoginAsync(userName);
        if (user == null)
        {
            return false;
        }

        return message.UserId == user.Id;
    }
}