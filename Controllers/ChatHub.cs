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

     public ChatHub(IViewService viewService, IMessageService messageService)
     {
          _viewService = viewService;
          _messageService = messageService;
     }
     
     public override async Task OnConnectedAsync()
     {
          await Groups.AddToGroupAsync(
               Context.ConnectionId, Context.User!.Identity!.Name!);
          await base.OnConnectedAsync();
     }

     public async Task GetUsers(int skip, int batchSize, 
          string sortBy, bool sortDesc)
     {
          var users = 
               await _viewService.GetUsersAsync(Context.User!.Identity!.Name!,
               skip, batchSize, sortBy, sortDesc);
          await Clients.Caller.SendAsync("GetUsers", users);
     }

     public async Task GetGroups(int skip, int batchSize, 
          string sortBy, bool sortDesc)
     {
          var views = 
               await _viewService.GetGroupsAsync(
                    Context.User!.Identity!.Name!,
                    skip, batchSize, sortBy, sortDesc);
          await Clients.Client(Context.ConnectionId).SendAsync(
               "GetGroups", views);
     }

     public async Task GetMessages(string chatName, int skip, int batchSize)
     {
          var messages = 
               await _messageService.GetMessageBatchAsync(
                    Context.User!.Identity!.Name!, 
                    chatName, skip, batchSize);
          await Clients.Client(Context.ConnectionId).SendAsync(
               "GetMessages", messages);
     }

     public async Task CreateChatIfNotExists(string chatName)
     {
          await _messageService.CreateIfNotExistsAsync(chatName);
     }

     public async Task BroadcastMessage(string chatName, 
          string messageText, int replyTo)
     {
          var username = Context.User!.Identity!.Name!;
          var message = await _messageService.SaveMessageAsync(
               username, chatName, 
               messageText, replyTo);
          
          await Clients.All.SendAsync(
               "BroadcastMessage", chatName, 
               new MessageView 
               {
                    Id = message.Id,
                    UserName = username,
                    DateTime = message.DateTime,
                    ReplyTo = message.ReplyTo,
                    Text = message.Text
               });
     }

     public async Task BroadcastEdit(int messageId, string messageText)
     {
          var chatName = await _messageService.EditMessageAsync(
               Context.User!.Identity!.Name!, messageId, messageText);
          if (chatName != null)
          {
               await Clients.All.SendAsync("BroadcastEdit", 
                    chatName, messageId, messageText);
          }
     }

     public async Task BroadcastDelete(int messageId)
     {
          var chatName = await _messageService.DeleteMessageAsync(
               Context.User!.Identity!.Name!, messageId);
          if (chatName == null)
          {
               return;
          }
          
          await Clients.All.SendAsync(
               "BroadcastDelete", chatName, messageId);
     }

     public async Task DeleteLocally(int messageId)
     {
          var chatName = await _messageService.DeleteMessageForUserAsync(
               Context.User!.Identity!.Name!, messageId);
          if (chatName == null)
          {
               return;
          }
          
          await Clients.Client(Context.ConnectionId).SendAsync(
               "BroadcastDelete", chatName, messageId);
     }
}