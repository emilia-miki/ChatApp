using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ChatApp.BLL.Interfaces;
using ChatApp.ViewModels;

namespace ChatApp.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ChatHub : Hub
{
     private readonly IViewService _viewService;
     private readonly IMessageService _messageService;
     private readonly IChatService _chatService;

     public ChatHub(IViewService viewService, IMessageService messageService,
          IChatService chatService)
     {
          _viewService = viewService;
          _messageService = messageService;
          _chatService = chatService;
     }
     
     public override async Task OnConnectedAsync()
     {
          await Groups.AddToGroupAsync(
               Context.ConnectionId, Context.User!.Identity!.Name!);
          await base.OnConnectedAsync();
     }

     public async Task GetUsersAsync(int skip, int batchSize, 
          string sortBy, bool sortDesc)
     {
          var users = 
               await _viewService.GetUsersAsync(
                    Context.User!.Identity!.Name!,
               skip, batchSize, sortBy, sortDesc);
          await Clients.Caller.SendAsync("GetUsersAsync", users);
     }

     public async Task GetGroupsAsync(int skip, int batchSize, 
          string sortBy, bool sortDesc)
     {
          var views = 
               await _viewService.GetGroupsAsync(
                    Context.User!.Identity!.Name!,
                    skip, batchSize, sortBy, sortDesc);
          await Clients.Client(Context.ConnectionId).SendAsync(
               "GetGroupsAsync", views);
     }

     public async Task GetMessagesAsync(string chatName, int skip, int batchSize)
     {
          var messages =
               await _viewService.GetMessageBatchAsync(
                    Context.User!.Identity!.Name!,
                    chatName, skip, batchSize);
          await Clients.Client(Context.ConnectionId).SendAsync(
               "GetMessagesAsync", messages);
     }

     public async Task CreatePersonalChatIfNotExistsAsync(
          string userName1, string userName2)
     {
          await _chatService.CreatePersonalChatIfNotExistsAsync(
               userName1, userName2);
     }

     public async Task BroadcastMessageAsync(string chatName, 
          string messageText, int replyTo)
     {
          var username = Context.User!.Identity!.Name!;
          var message = await _messageService.SaveMessageAsync(
               username, chatName,
               messageText, replyTo);
          if (message == null)
          {
               return;
          }
          
          await Clients.All.SendAsync(
               "BroadcastMessageAsync", chatName, 
               new MessageView 
               {
                    Id = message.Id,
                    UserName = username,
                    DateTime = message.DateTime,
                    ReplyTo = message.ReplyTo,
                    Text = message.Text
               });
     }

     public async Task BroadcastEditAsync(string chatName, 
          int messageId, string messageText)
     {
          if (string.IsNullOrWhiteSpace(messageText))
          {
               return;
          }
          
          await _messageService.EditMessageAsync(
               Context.User!.Identity!.Name!, messageId, messageText);
          await Clients.All.SendAsync("BroadcastEditAsync", 
               chatName, messageId, messageText);
     }

     public async Task BroadcastDeleteAsync(string chatName, int messageId)
     {
          await _messageService.DeleteMessageAsync(
               Context.User!.Identity!.Name!, messageId);
          await Clients.All.SendAsync(
               "BroadcastDeleteAsync", chatName, messageId);
     }

     public async Task DeleteLocallyAsync(string chatName, int messageId)
     {
          await _messageService.DeleteMessageForUserAsync(
               Context.User!.Identity!.Name!, messageId);
          await Clients.Client(Context.ConnectionId).SendAsync(
               "BroadcastDeleteAsync", chatName, messageId);
     }
}