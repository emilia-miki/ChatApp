using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ChatApp.BLL;

namespace ChatApp.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ChatHub : Hub
{
     private readonly IBlockService _blockService;
     private readonly IMessageService _messageService;

     public ChatHub(IBlockService blockService, IMessageService messageService)
     {
          _blockService = blockService;
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
          var users = _blockService.GetUsers(skip, batchSize, 
               sortBy, sortDesc);
          await Clients.Caller.SendAsync("GetUsers", users);
     }

     public async Task GetChats()
     {
          var blocks = 
               _blockService.GetBlocks(Context.User!.Identity!.Name!);
          await Clients.Client(Context.ConnectionId).SendAsync(
               "GetChats", blocks);
     }

     public async Task GetMessages(int skip, string chatName)
     {
          var messages = _messageService.GetMessageBatch(
               Context.User!.Identity!.Name!, skip, chatName);
          await Clients.Client(Context.ConnectionId).SendAsync(
               "GetMessages", chatName, messages);
     }

     public async Task BroadcastMessage(string chatName, string messageText, 
          int replyTo, bool replyIsPersonal)
     {
          var message = _messageService.SaveMessage(
               Context.User!.Identity!.Name!, chatName, 
               messageText, replyTo, replyIsPersonal);
          if (message == null)
          {
               return;
          }

          if (replyTo != -1 && replyIsPersonal)
          {
               await Clients.Client(Context.ConnectionId).SendAsync(
                    "BroadcastMessage", chatName, message);
               
               var username = _messageService.GetMessageSender(replyTo);
               if (username == null)
               {
                    return;
               }
               
               if (username != Context.User!.Identity!.Name)
               {
                    await Clients.Group(username).SendAsync(
                         "BroadcastMessage", chatName, message);
               }
          }
          else
          {
               await Clients.All.SendAsync(
                    "BroadcastMessage", chatName, message);
          }
     }

     public async Task BroadcastEdit(int messageId, string messageText)
     {
          var chatName = _messageService.EditMessage(
               Context.User!.Identity!.Name!, messageId, messageText);
          if (chatName != null)
          {
               await Clients.All.SendAsync("BroadcastEdit", 
                    chatName, messageId, messageText);
          }
     }

     public async Task BroadcastDelete(int messageId)
     {
          var chatName = _messageService.DeleteMessage(
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
          var chatName = _messageService.DeleteMessageForUser(
               Context.User!.Identity!.Name!, messageId);
          if (chatName == null)
          {
               return;
          }
          
          await Clients.Client(Context.ConnectionId).SendAsync(
               "BroadcastDelete", chatName, messageId);
     }
}