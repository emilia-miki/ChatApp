using ChatApp.DAL.Entities;
using ChatApp.ViewModels;

namespace ChatApp.BLL;

public interface IMessageService
{
    IEnumerable<MessageView> GetMessageBatch(
        string chatName, int skip, int batchSize);
    Task<Message> SaveMessage(string username, string chatName, 
        string messageText, int replyTo, bool replyIsPersonal);
    string? EditMessage(int messageId, string newText);
    Task<string?> DeleteMessageAsync(string username, int messageId);
    string? DeleteMessageForUser(string username, int messageId);
    Task<string?> GetMessageSenderAsync(int id);
}