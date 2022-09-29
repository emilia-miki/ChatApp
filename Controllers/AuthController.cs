using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChatApp.BLL;
using ChatApp.BLL.Models;
using ChatApp.DAL;
using ChatApp.DAL.Entities;
using ChatApp.DAL.Repositories;
using ChatApp.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Controllers;

public class AuthController : Controller
{
    private readonly IAccountService _accountService;
    private readonly UserManager<ApplicationUser> _manager;
    private readonly IUnitOfWork _unitOfWork;

    public AuthController(
        IAccountService accountService,
        UserManager<ApplicationUser> manager,
        IUnitOfWork unitOfWork)
    {
        _accountService = accountService;
        _manager = manager;
        _unitOfWork = unitOfWork;
    }
    
    [AllowAnonymous]
    [HttpPost]
    [Route("/auth/register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var registerResult = await _accountService.Register(model);

        if (registerResult.Succeeded)
        {
            return Ok(await _accountService.Login(
                new LoginUser(model.Login, model.Password)));
        }

        foreach (var error in registerResult.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return BadRequest(ModelState);
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("/auth/login")]
    public async Task<IActionResult> Login([FromBody] LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await _accountService.IsValidUserCredentials(
                new LoginUser(model.Login, model.Password)))
        {
            return Unauthorized();
        }

        return Ok(await _accountService.Login(
            new LoginUser(model.Login, model.Password)));
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    [Route("/auth/change")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] PasswordChangeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var login = HttpContext.User.Identity!.Name!;
        if (!await _accountService.IsValidUserCredentials(
                new LoginUser(login, model.OldPassword)))
        {
            return Unauthorized();
        }

        var changeResult = await _accountService.ChangePassword(
            HttpContext, model.NewPassword);

        if (changeResult.Succeeded)
        {
            return Ok(await _accountService.Login(
                new LoginUser(login, model.NewPassword)));
        }

        foreach (var error in changeResult.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return BadRequest(ModelState);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    [Route("/auth/delete")]
    public async Task<IActionResult> Delete()
    {
        await _accountService.Delete(HttpContext);
        return Ok();
    }
    
    [HttpPost]
    [Route("/generate/users")]
    public async Task<IActionResult> GenerateUsers()
    {
        const int count = 67;
        
        for (var i = 0; i < count; i++)
        {
            await Register(new RegisterViewModel
            {
                Login = $"login{i}",
                Email = $"email{i}@my.website",
                PhoneNumber = $"{i:D10}",
                Password = $"Password!{i}",
                PasswordConfirmation = $"Password!{i}"
            });
        }
        
        Console.WriteLine("Users registered");
        return Ok();
    }
    
    [HttpPost]
    [Route("/generate/personal-chats")]
    public async Task<IActionResult> GeneratePersonalChats()
    {
        var random = new Random();
        var users = _manager.Users.ToList();
        var messageRepository = _unitOfWork.GetRepository<MessageRepository>();
        var memberChatRepository = _unitOfWork
            .GetRepository<GenericRepository<MemberChat>>();
        var chatRepository = _unitOfWork.GetRepository<ChatRepository>();
        for (var i = 0; i < users.Count; i++)
        {
            for (var j = 1; j < users.Count; j++)
            {
                if (random.NextDouble() < 0.7)
                {
                    continue;
                }

                var username1 = users[i].UserName;
                var username2 = users[j].UserName;
                if (string.CompareOrdinal(username1, username2) > 0)
                {
                    (username1, username2) = (username2, username1);
                }
                var chat = new Chat
                {
                    Name = $"{username1} and {username2} Personal Chat"
                };
                chatRepository.Insert(chat);
                _unitOfWork.Save();
                
                memberChatRepository.Insert(new MemberChat
                {
                    ChatId = chat.Id,
                    UserId = users[i].Id
                });
                memberChatRepository.Insert(new MemberChat
                {
                    ChatId = chat.Id,
                    UserId = users[j].Id
                });

                for (var k = 0; k < 5; k++)
                {
                    messageRepository.Insert(new Message
                    {
                        ChatId = chat.Id,
                        DateTime = DateTime.UtcNow,
                        ReplyIsPersonal = true,
                        ReplyTo = -1,
                        UserId = k % 2 == 0 ? users[i].Id : users[j].Id,
                        Text = "message " + k
                    });
                }
                _unitOfWork.Save();
            }
        }

        Console.WriteLine("Personal chats registered");
        return Ok();
    }
    
    [HttpPost]
    [Route("/generate/group-chats")]
    public Task<IActionResult> GenerateGroupChats()
    {
        const int chatsCount = 12;
        var random = new Random();
        var users = _manager.Users.ToList();
        var messageRepository = _unitOfWork.GetRepository<MessageRepository>();
        var memberChatRepository = _unitOfWork
            .GetRepository<GenericRepository<MemberChat>>();
        var chatRepository = _unitOfWork.GetRepository<ChatRepository>();
        for (var i = 0; i < chatsCount; i++)
        {
            var chat = new Chat
            {
                IsPersonal = false,
                Name = $"Group chat {i}"
            };
            chatRepository.Insert(chat);
            _unitOfWork.Save();
            
            var usersCount = random.Next(users.Count);
            for (var j = 0; j < usersCount; j++)
            {
                var userIndex = random.Next(users.Count);
                memberChatRepository.Insert(new MemberChat
                {
                    ChatId = chat.Id,
                    UserId = users[userIndex].Id
                });
                for (var k = 0; k < random.Next(20); k++)
                {
                    messageRepository.Insert(new Message
                    {
                        ChatId = chat.Id,
                        UserId = users[userIndex].Id,
                        DateTime = DateTime.UtcNow - TimeSpan.FromMinutes(
                            random.Next(1000)),
                        ReplyTo = -1,
                        ReplyIsPersonal = false,
                        Text = $"{users[userIndex].UserName} message {k} " +
                               $"in chat {chat.Name}"
                    });
                }
            }
        }
        
        _unitOfWork.Save();
        Console.WriteLine("Group chats registered");
        return Task.FromResult<IActionResult>(Ok());
    }
}