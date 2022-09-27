using ChatApp.ViewModels;

namespace ChatApp.BLL;

public class MessageService : IMessageService
{
    public IEnumerable<MessageView> GetMessageBatch(string username, int skip, string chatName)
    {
        throw new NotImplementedException();
    }

    public MessageView? SaveMessage(string username, string chatName, string messageText,
        int replyTo, bool replyIsPersonal)
    {
        throw new NotImplementedException();
    }

    public string? EditMessage(string username, int messageId, string newText)
    {
        throw new NotImplementedException();
    }

    public string? DeleteMessage(string username, int messageId)
    {
        throw new NotImplementedException();
    }

    public string? DeleteMessageForUser(string username, int messageId)
    {
        throw new NotImplementedException();
    }

    public string? GetMessageSender(int id)
    {
        throw new NotImplementedException();
    }
}