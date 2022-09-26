using ChatApp.ViewModels;

namespace ChatApp.BLL;

public interface IBlockService
{ 
    IEnumerable<ChatView> GetBlocks(string username);
}