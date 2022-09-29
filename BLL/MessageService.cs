using ChatApp.DAL;
using ChatApp.DAL.Entities;
using ChatApp.DAL.Repositories;
using ChatApp.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.BLL;

public class MessageService : IMessageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public MessageService(IUnitOfWork unitOfWork, 
        UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    private async Task<string> GetUserIdAsync(string username)
    {
        var userRepository = _unitOfWork.GetRepository<UserRepository>();
        var user = await userRepository.GetByLoginAsync(username);
        var userId = user.Id!;
        return userId;
    }

    private int GetChatId(string chatName)
    {
        var chatRepository = _unitOfWork.GetRepository<ChatRepository>();
        var chat = chatRepository.GetByName(chatName);
        var chatId = chat!.Id;
        return chatId;
    }
    
    public IEnumerable<MessageView> GetMessageBatch( 
        string chatName, int skip, int batchSize)
    {
        var messageRepository = _unitOfWork.GetRepository<MessageRepository>();
        var chatRepository = _unitOfWork.GetRepository<ChatRepository>();
        var chat = chatRepository.GetByName(chatName);
        if (chat == null)
        {
            return new List<MessageView>();
        }
        var chatId = chat.Id;
        return messageRepository.GetMessages(chatId, skip, batchSize);
    }

    public async Task<Message> SaveMessage(string username, string chatName, 
        string messageText, int replyTo, bool replyIsPersonal)
    {
        var userId = await GetUserIdAsync(username);
        var chatId = GetChatId(chatName);

        var messageRepository = _unitOfWork.GetRepository<MessageRepository>();
        var message = new Message
        {
            ChatId = chatId,
            UserId = userId,
            DateTime = DateTime.UtcNow,
            Text = messageText,
            ReplyTo = replyTo,
            ReplyIsPersonal = replyIsPersonal
        };
        messageRepository.Insert(message);
        _unitOfWork.Save();
        return message;
    }

    public string? EditMessage(int messageId, string newText)
    {
        var messageRepository = _unitOfWork.GetRepository<MessageRepository>();
        var message = messageRepository.GetById(messageId);
        if (message == null)
        {
            return null;
        }

        var chatRepository = _unitOfWork.GetRepository<ChatRepository>();
        var chatName = chatRepository.GetById(message.ChatId)!.Name;
        
        message.Text = newText;
        messageRepository.Update(message);
        _unitOfWork.Save();
        return chatName;
    }

    public async Task<string?> DeleteMessageAsync(string username, int messageId)
    {
        var messageRepository = _unitOfWork.GetRepository<MessageRepository>();
        var message = messageRepository.GetById(messageId);
        var userId = await GetUserIdAsync(username);
        if (message?.UserId != userId)
        {
            return null;
        }

        var chatRepository = _unitOfWork.GetRepository<ChatRepository>();
        return chatRepository.GetById(message.ChatId)!.Name;
    }

    public string? DeleteMessageForUser(string username, int messageId)
    {
        throw new NotImplementedException();
    }

    public async Task<string?> GetMessageSenderAsync(int id)
    {
        var messageRepository = _unitOfWork.GetRepository<MessageRepository>();
        var message = messageRepository.GetById(id);
        if (message == null)
        {
            return null;
        }
        
        var userId = message.UserId;
        var user = await _userManager.FindByIdAsync(userId);
        return user.UserName;
    }
}