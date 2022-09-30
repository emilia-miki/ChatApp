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
    
    public async Task<Message?> GetMessageIfAuthorizedAsync(
        string username, int messageId)
    {
        var messageRepository = _unitOfWork.GetRepository<MessageRepository>();
        var message = await messageRepository.GetByIdAsync(messageId);
        
        var userRepository = _unitOfWork.GetRepository<UserRepository>();
        var user = await userRepository.GetByLoginAsync(username);
        var userId = user.Id!;
        
        return message?.UserId == userId ? message : null;
    }

    public async Task<bool> IsUserInChat(string userName, string chatName)
    {
        var userRepository = _unitOfWork.GetRepository<UserRepository>();
        var user = await userRepository.GetByLoginAsync(userName);
        var userId = user.Id!;
        
        var chatRepository = _unitOfWork.GetRepository<ChatRepository>();
        var chat = await chatRepository.GetByNameAsync(chatName);
        var chatId = chat!.Id;

        var memberChatRepository = _unitOfWork
            .GetRepository<MemberChatRepository>();
        return await memberChatRepository.ContainsAsync(userId, chatId);
    }
}