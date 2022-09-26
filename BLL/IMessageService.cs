using ChatApp.ViewModels;

namespace ChatApp.BLL;

public interface IMessageService
{
    IEnumerable<MessageView> GetMessageBatch(
        string username, int skip, string chatName);
    MessageView? SaveMessage(string username, string chatName, 
        string messageText, int replyTo, bool replyIsPersonal);
    string? EditMessage(string username, int messageId, string newText);
    string? DeleteMessage(string username, int messageId);
    string? DeleteMessageForUser(string username, int messageId);
    string? GetMessageSender(int id);
}