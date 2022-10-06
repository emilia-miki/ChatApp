using ChatApp.BLL.Interfaces;
using ChatApp.DAL;
using ChatApp.ViewModels;
using ChatApp.DAL.Repositories;

namespace ChatApp.BLL.Services;

public class ViewService : IViewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    public ViewService(IUnitOfWork unitOfWork, 
        IAuthorizationService authorizationService)
    {
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
    }
    
    public async Task<IEnumerable<ChatView>> GetGroupsAsync(string username, 
        int skip, int batchSize, string sortBy, bool sortDesc)
    {
        var chatRepository = _unitOfWork.GetRepository<ChatRepository>();
        var userRepository = _unitOfWork.GetRepository<UserRepository>();
        var user = await userRepository.GetByLoginAsync(username);
        if (user == null)
        {
            return new List<ChatView>();
        }
        var userId = user.Id!;
        var chatViews = await chatRepository.GetGroupsAsync(
            userId, skip, batchSize, sortBy, sortDesc);
        return chatViews;
    }

    public async Task<IEnumerable<UserView>> GetUsersAsync(string username,
        int skip, int batchSize, string sortBy, bool sortDesc)
    {
        var userRepository = _unitOfWork.GetRepository<UserRepository>();
        var user = await userRepository.GetByLoginAsync(username);
        if (user == null)
        {
            return new List<UserView>();
        }
        
        var userId = user.Id!;
        var userViews = 
            await userRepository.GetUserViewsAsync(userId, 
                skip, batchSize, sortBy, sortDesc);
        return userViews;
    }
    
    public async Task<IEnumerable<MessageView>> GetMessageBatchAsync( 
        string username, string chatName, int skip, int batchSize)
    {
        if (!await _authorizationService.IsUserInChatAsync(username, chatName))
        {
            return new List<MessageView>();
        }
        
        var chatRepository = _unitOfWork.GetRepository<ChatRepository>();
        var chat = await chatRepository.GetByNameAsync(chatName);
        if (chat == null)
        {
            return new List<MessageView>();
        }
        var chatId = chat.Id;

        var messageRepository = _unitOfWork.GetRepository<MessageRepository>();
        var messageViews = 
            await messageRepository.GetMessagesAsync(chatId, skip, batchSize);
        return messageViews;
    }
}