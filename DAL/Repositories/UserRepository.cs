using ChatApp.DAL.Entities;
using ChatApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.DAL.Repositories;

public class UserRepository : GenericRepository<ApplicationUser>
{
    private readonly UserManager<ApplicationUser> _userManager;
    
    public UserRepository(ChatsContext chatsContext,
        UserManager<ApplicationUser> userManager) : base(chatsContext)
    {
        _userManager = userManager;
    }

    public async Task<IEnumerable<UserView>> GetUserViewsAsync(string userId,
        int skip, int batchSize, string? sortBy, bool sortDesc)
    {
        return await Entities
            .Include(u => u.Messages)
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
            .ToListAsync();
    }

    public async Task<ApplicationUser> GetByLoginAsync(string login)
    {
        return await _userManager.FindByNameAsync(login);
    }
}