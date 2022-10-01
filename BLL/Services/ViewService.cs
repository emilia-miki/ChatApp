using ChatApp.BLL.Interfaces;
using ChatApp.DAL;
using ChatApp.DAL.Entities;
using ChatApp.ViewModels;
using ChatApp.DAL.Repositories;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.BLL.Services;

public class ViewService : IViewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuthorizationService _authorizationService;

    public ViewService(IUnitOfWork unitOfWork, 
        UserManager<ApplicationUser> userManager,
        IAuthorizationService authorizationService)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _authorizationService = authorizationService;
    }
    
    public async Task<IEnumerable<ChatView>> GetGroupsAsync(string username, 
        int skip, int batchSize, string sortBy, bool sortDesc)
    {
        var chatRepository = _unitOfWork.GetRepository<ChatRepository>();
        var user = await _userManager.FindByNameAsync(username);
        var userId = user.Id!;
        var chatViews = await chatRepository.GetGroupsAsync(
            userId, skip, batchSize, sortBy, sortDesc);
        return chatViews;
    }

    public async Task<IEnumerable<UserView>> GetUsersAsync(string username,
        int skip, int batchSize, string sortBy, bool sortDesc)
    {
        var userRepository = _unitOfWork.GetRepository<UserRepository>();
        var user = await _userManager.FindByNameAsync(username);
        var userId = user.Id!;
        var userViews = 
            await userRepository.GetUserViewsAsync(userId, 
                skip, batchSize, sortBy, sortDesc);
        return userViews;
    }
    
    public async Task<IEnumerable<MessageView>> GetMessageBatchAsync( 
        string username, string chatName, int skip, int batchSize)
    {
        if (!await _authorizationService.IsUserInChat(username, chatName))
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