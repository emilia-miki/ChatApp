using ChatApp.BLL.Interfaces;
using ChatApp.DAL;
using ChatApp.DAL.Entities;
using ChatApp.DAL.Repositories;
using ChatApp.ViewModels;

namespace ChatApp.BLL.Services;

public class MessageService : IMessageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    public MessageService(IUnitOfWork unitOfWork,
        IAuthorizationService authorizationService)
    {
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
    }

    private async Task<string> GetUserIdAsync(string username)
    {
        var userRepository = _unitOfWork.GetRepository<UserRepository>();
        var user = await userRepository.GetByLoginAsync(username);
        var userId = user.Id!;
        return userId;
    }

    private async Task<int> GetChatIdAsync(string chatName)
    {
        var chatRepository = _unitOfWork.GetRepository<ChatRepository>();
        var chat = await chatRepository.GetByNameAsync(chatName);
        var chatId = chat!.Id;
        return chatId;
    }
    
    public async Task<IEnumerable<MessageView>> GetMessageBatchAsync( 
        string username, string chatName, int skip, int batchSize)
    {
        if (!await _authorizationService.IsUserInChat(username, chatName))
        {
            return new List<MessageView>();
        }
        
        var chatRepository = _unitOfWork.GetRepository<ChatRepository>();
        var chat = await chatRepository.GetByNameAsync(chatName);
        if (chat == null)
        {
            return new List<MessageView>();
        }
        var chatId = chat.Id;

        var messageRepository = _unitOfWork.GetRepository<MessageRepository>();
        var messageViews = 
            await messageRepository.GetMessagesAsync(chatId, skip, batchSize);
        return messageViews;
    }

    public async Task<Message?> SaveMessageAsync(string username, string chatName, 
        string messageText, int replyTo)
    {
        if (!await _authorizationService.IsUserInChat(username, chatName))
        {
            return null;
        }
        
        var userId = await GetUserIdAsync(username);
        var chatId = await GetChatIdAsync(chatName);

        var messageRepository = _unitOfWork.GetRepository<MessageRepository>();
        var message = new Message
        {
            ChatId = chatId,
            UserId = userId,
            DateTime = DateTime.UtcNow,
            Text = messageText,
            ReplyTo = replyTo
        };
        
        await messageRepository.InsertAsync(message);
        await _unitOfWork.SaveAsync();
        
        return message;
    }

    public async Task CreateIfNotExistsAsync(string chatName)
    {
        var chatRepository = _unitOfWork.GetRepository<ChatRepository>();
        var chat = await chatRepository.GetByNameAsync(chatName);
        if (chat != null)
        {
            return;
        }

        chat = new Chat
        {
            Name = chatName,
            IsPersonal = true
        };
        await chatRepository.InsertAsync(chat);
        await _unitOfWork.SaveAsync();

        var memberChatRepository = _unitOfWork
            .GetRepository<MemberChatRepository>();
        
        var splits = chatName.Split(' ');
        var username1 = splits[0];
        var userId1 = await GetUserIdAsync(username1);
        var username2 = splits[2];
        var userId2 = await GetUserIdAsync(username2);
        
        await memberChatRepository.InsertAsync(new MemberChat
        {
            UserId = userId1,
            ChatId = chat.Id
        });
        
        await memberChatRepository.InsertAsync(new MemberChat
        {
            UserId = userId2,
            ChatId = chat.Id
        });
        
        await _unitOfWork.SaveAsync();
    }

    public async Task<string?> EditMessageAsync(string username,
        int messageId, string newText)
    {
        var message = await _authorizationService
            .GetMessageIfAuthorizedAsync(username, messageId);
        if (message == null)
        {
            return null;
        }

        var chatRepository = _unitOfWork.GetRepository<ChatRepository>();
        var chat = await chatRepository.GetByIdAsync(message.ChatId);
        var chatName = chat!.Name;
        
        message.Text = newText;
        var messageRepository = _unitOfWork.GetRepository<MessageRepository>();
        messageRepository.Update(message);
        await _unitOfWork.SaveAsync();
        
        return chatName;
    }

    public async Task<string?> DeleteMessageAsync(string username, int messageId)
    {
        var message = await _authorizationService
            .GetMessageIfAuthorizedAsync(username, messageId);
        if (message == null)
        {
            return null;
        }

        var chatRepository = _unitOfWork.GetRepository<ChatRepository>();
        var chat = await chatRepository.GetByIdAsync(message.ChatId);
        var chatName = chat!.Name;

        var messageRepository = _unitOfWork.GetRepository<MessageRepository>();
        messageRepository.Delete(message);
        await _unitOfWork.SaveAsync();
        return chatName;
    }

    public async Task<string?> DeleteMessageForUserAsync(
        string username, int messageId)
    {
        var message = await _authorizationService
            .GetMessageIfAuthorizedAsync(username, messageId);
        if (message == null)
        {
            return null;
        }
        
        var chatId = message.ChatId;
        var chatRepository = _unitOfWork.GetRepository<ChatRepository>();
        var chat = await chatRepository.GetByIdAsync(chatId);
        if (chat == null)
        {
            return null;
        }
        
        var repository = _unitOfWork
            .GetRepository<GenericRepository<MessageDeletedForUser>>();
        await repository.InsertAsync(new MessageDeletedForUser
        {
            UserId = await GetUserIdAsync(username),
            MessageId = messageId
        });
        await _unitOfWork.SaveAsync();

        return chat.Name;
    }
}