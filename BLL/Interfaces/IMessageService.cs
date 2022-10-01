using ChatApp.DAL.Entities;

namespace ChatApp.BLL.Interfaces;

public interface IMessageService
{
    Task<Message?> SaveMessageAsync(string username, 
        string chatName, string messageText, int replyTo);
    Task EditMessageAsync(string username, int messageId, string newText);
    Task DeleteMessageAsync(string username, int messageId);
    Task DeleteMessageForUserAsync(string username, int messageId);
    Task CreateIfNotExistsAsync(string chatName);
}