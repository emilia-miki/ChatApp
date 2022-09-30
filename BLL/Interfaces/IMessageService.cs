using ChatApp.DAL.Entities;
using ChatApp.ViewModels;

namespace ChatApp.BLL.Interfaces;

public interface IMessageService
{
    Task<IEnumerable<MessageView>> GetMessageBatchAsync(string username, 
        string chatName, int skip, int batchSize);
    Task<Message?> SaveMessageAsync(string username, 
        string chatName, string messageText, int replyTo);
    Task<string?> EditMessageAsync(string username, 
        int messageId, string newText);
    Task<string?> DeleteMessageAsync(string username, int messageId);
    Task<string?> DeleteMessageForUserAsync(string username, int messageId);
    Task CreateIfNotExistsAsync(string chatName);
}