using System.Security.Principal;
using ChatApp.BLL.Interfaces;
using ChatApp.Controllers;
using ChatApp.DAL.Entities;
using ChatApp.DAL.Repositories;
using ChatApp.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using IAuthorizationService = ChatApp.BLL.Interfaces.IAuthorizationService;

namespace ChatAppTests;

public static class TestHelper
{
    public static void SetUserExists(Mock<UserRepository> mock, 
        bool returnUser, string id = "")
    {
        mock.Setup(repo =>
                repo.GetByLoginAsync(It.IsAny<string>()))
            .ReturnsAsync((string login) =>
            {
                if (!returnUser)
                {
                    return null;
                }
                
                return new ApplicationUser
                {
                    Id = id,
                    UserName = login
                };
            });
    }
    
    public static void SetUserExists(Mock<UserManager<ApplicationUser>> mock, 
        bool returnUser, string id = "")
    {
        mock.Setup(manager =>
                manager.FindByNameAsync(It.IsAny<string>()))!
            .ReturnsAsync((string login) =>
            {
                if (!returnUser)
                {
                    return null;
                }
                
                return new ApplicationUser
                {
                    Id = id,
                    UserName = login
                };
            });
    }
    
    public static void SetChatExists(Mock<ChatRepository> mock, 
        bool returnChat, int id = 0)
    {
        mock.Setup(repo =>
                repo.GetByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((string name) =>
            {
                if (!returnChat)
                {
                    return null;
                }

                return new Chat
                {
                    Id = id,
                    Name = name
                };
            });
    }
    
    public static void SetAuthorizationReturns(
        Mock<IAuthorizationService> mock, bool result)
    {
        mock.Setup(auth =>
                auth.IsUserInChatAsync(It.IsAny<string>(),
                    It.IsAny<string>()))
            .ReturnsAsync((string _, string _) => result);
        mock.Setup(auth =>
                auth.IsMessageByUserAsync(It.IsAny<int>(),
                    It.IsAny<string>()))
            .ReturnsAsync((int _, string _) => result);
    }

    public static void SetMemberChat(Mock<MemberChatRepository> mock,
        string userId, int chatId, bool result)
    {
        mock.Setup(repo =>
                repo.ContainsAsync(userId, chatId))
            .ReturnsAsync((string _, int _) => result);
    }

    public static void SetMessageExists(Mock<MessageRepository> mock,
        bool result, string userId = "")
    {
        mock.Setup(repo =>
                repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) =>
            {
                if (!result)
                {
                    return null;
                }

                return new Message
                {
                    Id = id,
                    UserId = userId
                };
            });
    }

    public static void SetRegisterResult(
        Mock<IAccountService> mock, bool result)
    {
        mock.Setup(service =>
                service.RegisterAsync(It.IsAny<RegisterViewModel>()))
            .ReturnsAsync((RegisterViewModel _) => result
                ? IdentityResult.Success
                : IdentityResult.Failed());
    }

    public static void SetControllerContext(AuthController controller)
    {
        controller.ControllerContext.HttpContext =
            new DefaultHttpContext();
        controller.ControllerContext.HttpContext.User = 
            new GenericPrincipal(
                new GenericIdentity(""), Array.Empty<string>());
    }

    public static void SetChangeResult(Mock<IAccountService> mock, bool result)
    {
        mock.Setup(service =>
                service.ChangePasswordAsync(It.IsAny<string>(),
                    It.IsAny<string>()))
            .ReturnsAsync((string _, string _) => result
                ? IdentityResult.Success
                : IdentityResult.Failed());
    }
}