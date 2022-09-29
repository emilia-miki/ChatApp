using ChatApp.ViewModels;

namespace ChatApp.BLL;

public interface IViewService
{ 
    Task<IEnumerable<ChatView>> GetGroupsAsync(string username, int skip, int 
    batchSize,
        string sortBy, bool sortDesc);
    Task<IEnumerable<UserView>> GetUsersAsync(string username, int skip, int 
    batchSize, 
        string sortBy, bool sortDesc);
}