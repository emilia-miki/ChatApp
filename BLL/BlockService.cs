using ChatApp.DAL;
using ChatApp.ViewModels;
using ChatApp.DAL.Repositories;

namespace ChatApp.BLL;

public class BlockService : IBlockService
{
    private readonly IUnitOfWork _unitOfWork;

    public BlockService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public IEnumerable<ChatView> GetBlocks(string username)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<UserView> GetUsers(int skip, int batchSize, 
        string sortBy, bool sortDesc)
    {
        var userRepository = _unitOfWork.GetRepository<UserRepository>();
        var messageRepository = _unitOfWork.GetRepository<MessageRepository>();
        var sort = sortBy == "LastMessageTime" ? null : sortBy;
        var extendedUserViews = 
            userRepository.GetUserViews(skip, batchSize, sort, sortDesc);
        
        var userViews = extendedUserViews.Select(euw =>
            new UserView 
            {
                Login = euw.Login,
                Email = euw.Email,
                LastMessageTime = messageRepository
                    .GetLatestMessageTime(euw.Id),
                Phone = euw.Phone
            });
        
        if (sort == null)
        {
            return sortDesc
                ? userViews.OrderByDescending(v => v.LastMessageTime)
                : userViews.OrderBy(v => v.LastMessageTime);
        }
        
        return userViews;
    }
}