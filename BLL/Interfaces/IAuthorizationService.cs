using ChatApp.DAL.Entities;

namespace ChatApp.BLL.Interfaces;

public interface IAuthorizationService
{
    Task<Message?> GetMessageIfAuthorizedAsync(string username, int messageId);
    Task<bool> IsUserInChat(string userName, string chatName);
}