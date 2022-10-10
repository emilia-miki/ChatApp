using System.Net.Http.Headers;
using System.Net.Http.Json;
using ChatApp.DAL;
using ChatApp.DAL.Entities;
using ChatApp.ViewModels;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace ChatAppTests.IntegrationTests;

[TestFixture]
public class ChatHubTests : IntegrationTest
{
    private HubConnection _connection;
    private object? result;
    private ChatsContext _context;
    private string currentUserId;
    private int _currentChat, _currentMessage, _currentUser;

    [SetUp]
    public async Task SetUp()
    {
        result = null;
        _currentChat = 0;
        _currentMessage = 0;
        _currentUser = 0;
        
        _context = new ChatsContext(new DbContextOptionsBuilder<ChatsContext>()
            .UseSqlServer(Factory.Configuration
                .GetSection("ConnectionStrings")["Default"])
            .Options, Factory.Configuration);
        await ResetDatabase();
        
        var response = await Client.PostAsJsonAsync("/auth/register",
            new RegisterViewModel
            {
                Login = "username",
                Password = "Password1!",
                Email = "email@mail.com",
                PasswordConfirmation = "Password1!",
                PhoneNumber = "380681117799"
            });
        var loginResult = await response.Content
            .ReadFromJsonAsync<LoginResult>();
        var user = _context.Users.Single(
            u => u.UserName == "username");
        currentUserId = user.Id;
        
        _connection = new HubConnectionBuilder()
            .WithUrl("http://localhost/hub", 
                options =>
                {
                    options.Headers.Add("Authorization", 
                        new AuthenticationHeaderValue(
                            "Bearer", loginResult!.AccessToken)
                            .ToString());
                    options.HttpMessageHandlerFactory =
                        _ => Factory.Server.CreateHandler();
                })
            .Build();
        
        // add event handlers
        _connection.On("GetUsersAsync",
            (List<UserView> list) => result = list);
        _connection.On("GetGroupsAsync",
            (List<ChatView> list) => result = list);
        _connection.On("GetMessagesAsync",
            (List<MessageView> list) => result = list);
        _connection.On("BroadcastMessageAsync",
            (string chatName, MessageView message) => 
                result = (chatName, message));
        _connection.On("BroadcastEditAsync",
            (string chatName, int messageId, string messageText) => 
                result = (chatName, messageId, messageText));
        _connection.On("BroadcastDeleteAsync",
            (string chatName, int messageId) =>
                result = (chatName, messageId));
        _connection.On("DeleteLocallyAsync",
            (string chatName, int messageId) =>
                result = (chatName, messageId));
        
        await _connection.StartAsync();
    }

    private async Task<string> RegisterUserAsync()
    {
        var response = await Client.PostAsJsonAsync("/auth/register",
            new RegisterViewModel
            {
                Login = $"username{_currentUser}",
                Password = $"Password{_currentUser}!",
                PasswordConfirmation = $"Password{_currentUser}!",
                Email = $"email{_currentUser}@mail.com",
                PhoneNumber = $"{_currentUser:D10}"
            });
        await response.Content.ReadFromJsonAsync<LoginResult>();
        var user = _context.Users.Single(
            u => u.UserName == $"username{_currentUser}");
        _currentUser++;
        return user.Id!;
    }

    private async Task<int> CreateChatAsync(bool isPersonal)
    {
        var chat = new Chat
        {
            IsPersonal = isPersonal,
            Name = $"Chat{_currentChat}"
        };
        _currentChat++;
        _context.Chats.Add(chat);
        await _context.SaveChangesAsync();
        return chat.Id;
    }

    private async Task AddMemberToChatAsync(string userId, int chatId)
    {
        _context.MembersChats.Add(new MemberChat
        {
            UserId = userId,
            ChatId = chatId
        });
        await _context.SaveChangesAsync();
    }

    private async Task<int> AddMessageToChatAsync(string userId, int chatId)
    {
        var message = new Message
        {
            Text = $"Message{_currentMessage}",
            ReplyTo = -1,
            ChatId = chatId,
            DateTime = DateTime.UtcNow,
            UserId = userId
        };
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        return message.Id;
    }

    [Test]
    public async Task GetUsersAsync_NoOtherUsers_ReturnEmptyList()
    {
        await _connection.InvokeAsync("GetUsersAsync",
            3, 5, "Login", true);

        while (result == null)
        {
            await Task.Delay(100);
        }
        
        var list = (List<UserView>) result;
        Assert.IsEmpty(list);
    }

    [Test]
    public async Task GetUsersAsync_SortDescending_ReturnUsersExceptCaller()
    {
        for (var i = 0; i < 10; i++)
        {
            await RegisterUserAsync();
        }

        await _connection.InvokeAsync("GetUsersAsync",
            1, 7, "Login", true);

        while (result == null)
        {
            await Task.Delay(100);
        }
        
        var list = (List<UserView>) result;
        
        // check count
        Assert.AreEqual(7, list.Count);
        
        // check skip and caller absence
        Assert.IsFalse(list.Any(u => u.Login == "username"));
        Assert.IsFalse(list.Any(u => u.Login == "username9"));
        
        // check sorting
        var sortedList = list.OrderByDescending(
            u => u.Login).ToList();
        for (var i = 0; i < list.Count; i++)
        {
            Assert.AreEqual(sortedList[i], list[i]);
        }
    }

    [Test]
    public async Task GetUsersAsync_SortAscending_ReturnUsersExceptCaller()
    {
        for (var i = 0; i < 10; i++)
        {
            await RegisterUserAsync();
        }
        
        await _connection.InvokeAsync("GetUsersAsync",
            2, 6, "Login", false);

        while (result == null)
        {
            await Task.Delay(100);
        }
        
        var list = (List<UserView>) result;
        
        // check count
        Assert.AreEqual(6, list.Count);
        
        // check skip and caller absence
        Assert.IsFalse(list.Any(u => u.Login == "username"));
        Assert.IsFalse(list.Any(u => u.Login == "username0"));
        Assert.IsFalse(list.Any(u => u.Login == "username1"));
        
        // check sorting
        var sortedList = list.OrderBy(u => u.Login).ToList();
        for (var i = 0; i < list.Count; i++)
        {
            Assert.AreEqual(sortedList[i], list[i]);
        }
    }
    
    [Test]
    public async Task GetGroupsAsync_NoGroups_ReturnEmptyList()
    {
        await _connection.InvokeAsync("GetGroupsAsync",
            3, 5, "Name", true);

        while (result == null)
        {
            await Task.Delay(100);
        }

        var list = (List<ChatView>) result;
        
        Assert.IsEmpty(list);
    }

    [Test]
    public async Task CreatePersonalChatIfNotExists_CreatesChatExceptDuplicate()
    {
        await RegisterUserAsync();
        
        await _connection.InvokeAsync(
            "CreatePersonalChatIfNotExistsAsync",
            "username", "username0");

        Assert.AreEqual(1, _context.Chats.Count());

        await _connection.InvokeAsync(
            "CreatePersonalChatIfNotExistsAsync",
            "username", "username0");
        
        Assert.AreEqual(1, _context.Chats.Count());
    }

    [Test]
    public async Task GetGroupsAsync_IsNotGroupMember_ReturnEmptyList()
    {
        await CreateChatAsync(false);
        await CreateChatAsync(true);

        await _connection.InvokeAsync("GetGroupsAsync",
            0, 10, "Name", false);

        while (result == null)
        {
            await Task.Delay(100);
        }

        var list = (List<ChatView>) result;
        Assert.IsEmpty(list);
    }
    
    [Test]
    // personal chats should NOT be returned!
    public async Task GetGroupsAsync_NoMessages_ReturnGroups()
    {
        await CreateChatAsync(false);
        var chatId1 = await CreateChatAsync(false);
        var chatId2 = await CreateChatAsync(false);
        await CreateChatAsync(true);

        await AddMemberToChatAsync(currentUserId, chatId1);
        await AddMemberToChatAsync(currentUserId, chatId2);

        await _connection.InvokeAsync("GetGroupsAsync", 
            1, 1, "Name", false);

        while (result == null)
        {
            await Task.Delay(100);
        }

        var list = (List<ChatView>) result;
        Assert.IsNotEmpty(list);
        Assert.AreEqual(1, list.Count);
        Assert.AreEqual("Chat2", list[0].Name);
    }
    
    [Test]
    // personal chats should NOT be returned!
    public async Task
        GetGroupsAsync_WithMessages_ReturnGroupsAndLatestMessageDateTime()
    {
        await RegisterUserAsync();
        var newUser = _context.Users.Single(
            u => u.UserName == "username0");
        var newUserId = newUser.Id!;

        var chatId = await CreateChatAsync(false);

        await AddMemberToChatAsync(currentUserId, chatId);
        await AddMemberToChatAsync(newUserId, chatId);

        var dateTime1 = DateTime.UtcNow;
        _context.Messages.Add(new Message
        {
            ChatId = chatId,
            UserId = currentUserId,
            DateTime = dateTime1,
            ReplyTo = -1,
            Text = ""
        });
        await _context.SaveChangesAsync();
        
        await _connection.InvokeAsync("GetGroupsAsync", 
            0, 10, "Name", false);

        while (result == null)
        {
            await Task.Delay(100);
        }

        var list = (List<ChatView>) result;
        Assert.IsNotEmpty(list);
        Assert.AreEqual(1, list.Count);
        Assert.AreEqual(dateTime1, list[0].LatestMessageDateTime);

        var dateTime2 = DateTime.UtcNow;
        _context.Messages.Add(new Message
        {
            ChatId = chatId,
            UserId = newUserId,
            DateTime = dateTime2,
            ReplyTo = -1,
            Text = ""
        });
        await _context.SaveChangesAsync();

        result = null;
        
        await _connection.InvokeAsync("GetGroupsAsync", 
            0, 10, "Name", false);

        while (result == null)
        {
            await Task.Delay(100);
        }

        list = (List<ChatView>) result;
        Assert.IsNotEmpty(list);
        Assert.AreEqual(1, list.Count);
        Assert.AreEqual(dateTime2, list[0].LatestMessageDateTime);
    }

    [Test]
    public async Task GetMessagesAsync_NoMessages_ReturnEmptyList()
    {
        var chatId = await CreateChatAsync(false);
        await AddMemberToChatAsync(currentUserId, chatId);

        await _connection.InvokeAsync("GetMessagesAsync",
            "Chat0", 0, 10);

        while (result == null)
        {
            await Task.Delay(100);
        }

        var list = (List<MessageView>) result;
        Assert.IsEmpty(list);
    }
    
    [Test]
    public async Task GetMessagesAsync_ReturnMessages()
    {
        var userId = await RegisterUserAsync();
        var chatId = await CreateChatAsync(false);
        var anotherChatId = await CreateChatAsync(true);
        await AddMemberToChatAsync(currentUserId, chatId);
        await AddMemberToChatAsync(userId, chatId);
        await AddMemberToChatAsync(currentUserId, anotherChatId);
        await AddMemberToChatAsync(userId, anotherChatId);
        for (var i = 0; i < 4; i++)
        {
            await AddMessageToChatAsync(userId, chatId);
            await AddMessageToChatAsync(currentUserId, chatId);
        }
        for (var i = 0; i < 5; i++)
        {
            await AddMessageToChatAsync(userId, anotherChatId);
            await AddMessageToChatAsync(currentUserId, anotherChatId);
        }

        await _connection.InvokeAsync("GetMessagesAsync",
            "Chat0", 0, 10);

        while (result == null)
        {
            await Task.Delay(100);
        }

        var list = (List<MessageView>) result;
        Assert.AreEqual(8, list.Count);
    }

    [Test]
    public async Task BroadcastMessageAsync_ReturnMessage()
    {
        var chatId = await CreateChatAsync(false);
        await AddMemberToChatAsync(currentUserId, chatId);

        await _connection.InvokeAsync("BroadcastMessageAsync",
            "Chat0", "text", -1);

        while (result == null)
        {
            await Task.Delay(100);
        }

        var (chatName, message) = ((string, MessageView)) result;
        Assert.AreEqual("Chat0", chatName);
        Assert.AreEqual("text", message.Text);
    }
    
    [Test]
    public async Task BroadcastEditAsync_ReturnIdAndNewText()
    {
        var chatId = await CreateChatAsync(false);
        await AddMemberToChatAsync(currentUserId, chatId);
        var messageId = await AddMessageToChatAsync(currentUserId, chatId);

        await _connection.InvokeAsync("BroadcastEditAsync",
            "Chat0", messageId, "text");

        while (result == null)
        {
            await Task.Delay(100);
        }

        var (chatName, id, text) = 
            ((string, int, string)) result;
        Assert.AreEqual("Chat0", chatName);
        Assert.AreEqual("text",  text);
        Assert.AreEqual(messageId, id);
    }
    
    [Test]
    public async Task BroadcastDeleteAsync_DeleteMessage()
    {
        var chatId = await CreateChatAsync(false);
        await AddMemberToChatAsync(currentUserId, chatId);
        var messageId = await AddMessageToChatAsync(currentUserId, chatId);

        await _connection.InvokeAsync("BroadcastDeleteAsync",
            "Chat0", messageId);

        while (result == null)
        {
            await Task.Delay(100);
        }

        var (chatName, id) = ((string, int)) result;
        Assert.AreEqual("Chat0", chatName);
        Assert.AreEqual(messageId, id);
        Assert.AreEqual(0, _context.Messages.Count());
    }
}