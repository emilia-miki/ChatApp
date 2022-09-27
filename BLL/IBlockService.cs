using ChatApp.BLL.Models;
using ChatApp.ViewModels;

namespace ChatApp.BLL;

public interface IBlockService
{ 
    IEnumerable<ChatView> GetBlocks(string username);
    IEnumerable<UserView> GetUsers(int skip, int batchSize, 
        string sortBy, bool sortDesc);
}