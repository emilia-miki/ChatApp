using System.Linq.Expressions;
using ChatApp.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.DAL.Repositories;

public class UserRepository : GenericRepository<IdentityUser>
{
    public UserRepository(ChatsContext chatsContext) : base(chatsContext)
    {
    }

    public IEnumerable<ExtendedUserView> GetUserViews(int skip, int batchSize,
        string? sortBy, bool sortDesc)
    {
        return Entities.Select(user => new ExtendedUserView 
            {
                Id = user.Id,
                Login = user.UserName,
                Email = user.Email,
                Phone = user.PhoneNumber
            })
            .OrderBy(sortBy, sortDesc)
            .Skip(skip)
            .Take(batchSize)
            .ToList();
    }
}