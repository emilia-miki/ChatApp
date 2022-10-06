namespace ChatApp.BLL.Interfaces;

public interface IAuthorizationService
{
    Task<bool> IsUserInChatAsync(string userName, string chatName);
    Task<bool> IsMessageByUserAsync(int messageId, string userName);
}