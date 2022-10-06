using ChatApp.BLL.Interfaces;
using ChatApp.DAL;
using ChatApp.DAL.Entities;
using ChatApp.DAL.Repositories;

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

    public async Task<Message?> SaveMessageAsync(
        string username, string chatName, string messageText, int replyTo)
    {
        if (string.IsNullOrWhiteSpace(messageText))
        {
            return null;
        }
        
        if (!await _authorizationService.IsUserInChatAsync(username, chatName))
        {
            return null;
        }
        
        var userRepository = _unitOfWork.GetRepository<UserRepository>();
        var user = await userRepository.GetByLoginAsync(username);
        var userId = user!.Id!;
        
        var chatRepository = _unitOfWork.GetRepository<ChatRepository>();
        var chat = await chatRepository.GetByNameAsync(chatName);
        var chatId = chat!.Id;
        
        var messageRepository = _unitOfWork.GetRepository<MessageRepository>();
        var message = new Message
        {
            ChatId = chatId,
            UserId = userId,
            DateTime = DateTime.UtcNow,
            Text = messageText,
            ReplyTo = replyTo
        };

        await _unitOfWork.CreateTransactionAsync();
        await messageRepository.InsertAsync(message);
        await _unitOfWork.CommitAsync();
        await _unitOfWork.SaveAsync();
        
        return message;
    }

    public async Task EditMessageAsync(string username, 
        int messageId, string newText)
    {
        if (string.IsNullOrWhiteSpace(newText))
        {
            return;
        }
        
        if (!await _authorizationService.IsMessageByUserAsync(
                messageId, username))
        {
            return;
        }
        
        var messageRepository = _unitOfWork.GetRepository<MessageRepository>();
        var message = await messageRepository.GetByIdAsync(messageId);
        message!.Text = newText;
        await _unitOfWork.CreateTransactionAsync();
        messageRepository.Update(message);
        await _unitOfWork.CommitAsync();
        await _unitOfWork.SaveAsync();
    }

    public async Task DeleteMessageAsync(string username, int messageId)
    {
        if (!await _authorizationService.IsMessageByUserAsync(
                messageId, username))
        {
            return;
        }
        
        var messageRepository = _unitOfWork.GetRepository<MessageRepository>();
        var message = await messageRepository.GetByIdAsync(messageId);
        await _unitOfWork.CreateTransactionAsync();
        messageRepository.Delete(message!);
        await _unitOfWork.CommitAsync();
        await _unitOfWork.SaveAsync();
    }

    public async Task DeleteMessageForUserAsync(
        string username, int messageId)
    {
        if (!await _authorizationService.IsMessageByUserAsync(
                messageId, username))
        {
            return;
        }
        
        var userRepository = _unitOfWork.GetRepository<UserRepository>();
        var user = await userRepository.GetByLoginAsync(username);
        var userId = user!.Id!;
        
        var repository = _unitOfWork
            .GetRepository<GenericRepository<MessageDeletedForUser>>();
        await _unitOfWork.CreateTransactionAsync();
        await repository.InsertAsync(new MessageDeletedForUser
        {
            UserId = userId,
            MessageId = messageId
        });
        await _unitOfWork.CommitAsync();
        await _unitOfWork.SaveAsync();
    }
}