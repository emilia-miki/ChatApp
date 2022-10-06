using ChatApp.BLL.Interfaces;
using ChatApp.BLL.Services;
using ChatApp.DAL;
using ChatApp.DAL.Repositories;
using Moq;
using NUnit.Framework;

namespace ChatAppTests.UnitTests.ServiceTests;

[TestFixture]
public class AuthorizationServiceTests
{
    private Mock<UserRepository> _userRepositoryMock = null!;
    private Mock<ChatRepository> _chatRepositoryMock = null!;
    private Mock<MessageRepository> _messageRepositoryMock = null!;
    private Mock<MemberChatRepository> _memberChatRepositoryMock = null!;
    private Mock<IUnitOfWork> _unitOfWorkMock = null!;
    private IAuthorizationService _authorizationService = null!;

    [SetUp]
    public void SetUp()
    {
        _userRepositoryMock = new Mock<UserRepository>();
        _chatRepositoryMock = new Mock<ChatRepository>();
        _messageRepositoryMock = new Mock<MessageRepository>();
        _memberChatRepositoryMock = new Mock<MemberChatRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _unitOfWorkMock.Setup(uow =>
                uow.GetRepository<ChatRepository>())
            .Returns(_chatRepositoryMock.Object);
        _unitOfWorkMock.Setup(uow =>
                uow.GetRepository<UserRepository>())
            .Returns(_userRepositoryMock.Object);
        _unitOfWorkMock.Setup(uow =>
                uow.GetRepository<MemberChatRepository>())
            .Returns(_memberChatRepositoryMock.Object);
        _unitOfWorkMock.Setup(uow =>
                uow.GetRepository<MessageRepository>())
            .Returns(_messageRepositoryMock.Object);
        
        _authorizationService = 
            new AuthorizationService(_unitOfWorkMock.Object);
    }
    
    [Test]
    public async Task IsUserInChatAsync_UserDoesNotExist_ReturnFalse()
    {
        TestHelper.SetUserExists(_userRepositoryMock, false, "uId");
        TestHelper.SetChatExists(_chatRepositoryMock, true, 42);
        TestHelper.SetMemberChat(_memberChatRepositoryMock, 
            "uId", 42, true);

        var result = await _authorizationService
            .IsUserInChatAsync("", "");
        
        
        Assert.IsFalse(result);
    }
    
    [Test]
    public async Task IsUserInChatAsync_ChatDoesNotExist_ReturnFalse()
    {
        TestHelper.SetUserExists(_userRepositoryMock, true, "uId");
        TestHelper.SetChatExists(_chatRepositoryMock, false, 42);
        TestHelper.SetMemberChat(_memberChatRepositoryMock, 
            "uId", 42, true);

        var result = await _authorizationService
            .IsUserInChatAsync("", "");
        
        Assert.IsFalse(result);
    }
    
    [Test]
    public async Task IsUserInChatAsync_UserIsNotMember_ReturnFalse()
    {
        TestHelper.SetUserExists(_userRepositoryMock, true, "uId");
        TestHelper.SetChatExists(_chatRepositoryMock, true, 42);
        TestHelper.SetMemberChat(_memberChatRepositoryMock, 
            "uId", 42, false);

        var result = await _authorizationService
            .IsUserInChatAsync("", "");
        
        Assert.IsFalse(result);
    }
    
    [Test]
    public async Task IsUserInChatAsync_UserAuthorized_ReturnTrue()
    {
        TestHelper.SetUserExists(_userRepositoryMock, true, "uId");
        TestHelper.SetChatExists(_chatRepositoryMock, true, 42);
        TestHelper.SetMemberChat(_memberChatRepositoryMock, 
            "uId", 42, true);

        var result = await _authorizationService
            .IsUserInChatAsync("", "");
        
        Assert.IsTrue(result);
    }

    [Test]
    public async Task IsMessageByUser_UserDoesNotExist_ReturnFalse()
    {
        TestHelper.SetUserExists(_userRepositoryMock, false);
        TestHelper.SetMessageExists(_messageRepositoryMock, true);

        var result = await _authorizationService
            .IsMessageByUserAsync(0, "");
        
        Assert.IsFalse(result);
    }
    
    [Test]
    public async Task IsMessageByUser_MessageDoesNotExist_ReturnFalse()
    {
        TestHelper.SetUserExists(_userRepositoryMock, true);
        TestHelper.SetMessageExists(_messageRepositoryMock, false);

        var result = await _authorizationService
            .IsMessageByUserAsync(0, "");
        
        Assert.IsFalse(result);
    }
    
    [Test]
    public async Task IsMessageByUser_IsAuthorized_ReturnTrue()
    {
        TestHelper.SetUserExists(_userRepositoryMock, true, "uId");
        TestHelper.SetMessageExists(_messageRepositoryMock, true, "uId");

        var result = await _authorizationService
            .IsMessageByUserAsync(0, "");
        
        Assert.IsTrue(result);
    }
}