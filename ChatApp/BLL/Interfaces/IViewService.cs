using ChatApp.ViewModels;

namespace ChatApp.BLL.Interfaces;

public interface IViewService
{ 
    Task<IEnumerable<MessageView>> GetMessageBatchAsync(string username, 
        string chatName, int skip, int batchSize);
    Task<IEnumerable<ChatView>> GetGroupsAsync(string username, 
        int skip, int batchSize, string sortBy, bool sortDesc);
    Task<IEnumerable<UserView>> GetUsersAsync(string username, 
        int skip, int batchSize, string sortBy, bool sortDesc);
}