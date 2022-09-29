using ChatApp.DAL.Entities;
using ChatApp.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.DAL.Repositories;

public class UserRepository : GenericRepository<ApplicationUser>
{
    private readonly UserManager<ApplicationUser> _userManager;
    
    public UserRepository(ChatsContext chatsContext,
        UserManager<ApplicationUser> userManager) : base(chatsContext)
    {
        _userManager = userManager;
    }

    public IEnumerable<UserView> GetUserViews(string userId,
        int skip, int batchSize, string? sortBy, bool sortDesc)
    {
        return Entities
            .Where(u => u.Id != userId)
            .Select(u => new UserView
            {
                Login = u.UserName,
                Email = u.Email,
                Phone = u.PhoneNumber
            })
            .OrderBy(sortBy, sortDesc)
            .Skip(skip)
            .Take(batchSize)
            .ToList();
    }

    public async Task<ApplicationUser> GetByLoginAsync(string login)
    {
        return await _userManager.FindByNameAsync(login);
    }
}