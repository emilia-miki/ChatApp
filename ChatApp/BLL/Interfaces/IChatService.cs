namespace ChatApp.BLL.Interfaces;

public interface IChatService
{
    Task CreatePersonalChatIfNotExistsAsync(string userName1, string userName2);
}