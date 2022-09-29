using ChatApp.DAL;
using ChatApp.DAL.Entities;
using ChatApp.ViewModels;
using ChatApp.DAL.Repositories;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.BLL;

public class ViewService : IViewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public ViewService(IUnitOfWork unitOfWork, 
        UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }
    
    public async Task<IEnumerable<ChatView>> GetGroupsAsync(string username, 
        int skip, int batchSize, string sortBy, bool sortDesc)
    {
        var chatRepository = _unitOfWork.GetRepository<ChatRepository>();
        var user = await _userManager.FindByNameAsync(username);
        var userId = user.Id!;
        var chatViews = chatRepository.GetGroups(
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
            userRepository.GetUserViews(userId, 
                skip, batchSize, sortBy, sortDesc);
        return userViews;
    }
}