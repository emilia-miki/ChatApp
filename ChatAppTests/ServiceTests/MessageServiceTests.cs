using ChatApp.BLL.Interfaces;
using ChatApp.BLL.Services;
using ChatApp.DAL;
using ChatApp.DAL.Entities;
using ChatApp.DAL.Repositories;
using Moq;
using NUnit.Framework;

namespace ChatAppTests.ServiceTests;

[TestFixture]
public class MessageServiceTests
{
    private Mock<UserRepository> _userRepositoryMock = null!;
    private Mock<ChatRepository> _chatRepositoryMock = null!;
    private Mock<MessageRepository> _messageRepositoryMock = null!;
    private Mock<MemberChatRepository> _memberChatRepositoryMock = null!;
    private Mock<GenericRepository<MessageDeletedForUser>>
        _messageDeletedForUserRepositoryMock = null!;
    private Mock<IUnitOfWork> _unitOfWorkMock = null!;
    private Mock<IAuthorizationService> _authorizationServiceMock = null!;
    private IMessageService _messageService = null!;

    [SetUp]
    public void SetUp()
    {
        _userRepositoryMock = new Mock<UserRepository>();
        _chatRepositoryMock = new Mock<ChatRepository>();
        _messageRepositoryMock = new Mock<MessageRepository>();
        _memberChatRepositoryMock = new Mock<MemberChatRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _messageDeletedForUserRepositoryMock =
            new Mock<GenericRepository<MessageDeletedForUser>>();

        _unitOfWorkMock.Setup(uow =>
                uow.GetRepository<ChatRepository>())
            .Returns(_chatRepositoryMock.Object);
        _unitOfWorkMock.Setup(uow =>
                uow.GetRepository<UserRepository>())
            .Returns(_userRepositoryMock.Object);
        _unitOfWorkMock.Setup(uow =>
                uow.GetRepository<MessageRepository>())
            .Returns(_messageRepositoryMock.Object);
        _unitOfWorkMock.Setup(uow =>
                uow.GetRepository<MemberChatRepository>())
            .Returns(_memberChatRepositoryMock.Object);
        _unitOfWorkMock.Setup(uow =>
                uow.GetRepository<GenericRepository<MessageDeletedForUser>>())
            .Returns(_messageDeletedForUserRepositoryMock.Object);

        _messageService = new MessageService(_unitOfWorkMock.Object,
            _authorizationServiceMock.Object);
    }
    
    [Test]
    public async Task SaveMessageAsync_AuthorizationFailed_ReturnNull()
    {
        // This also checks for chat or user not existing
        TestHelper.SetAuthorizationReturns(
            _authorizationServiceMock, false);
        TestHelper.SetChatExists(_chatRepositoryMock, true, 42);
        TestHelper.SetUserExists(_userRepositoryMock, true, "uId");

        var result = await _messageService.SaveMessageAsync(
            "", "", "test", -1);

        _authorizationServiceMock.Verify(auth => 
            auth.IsUserInChatAsync(It.IsAny<string>(), 
                It.IsAny<string>()), Times.Once);
        _messageRepositoryMock.Verify(repo =>
            repo.InsertAsync(It.IsAny<Message>()), Times.Never);
        Assert.IsNull(result);
    }
    
    [Test]
    public async Task SaveMessageAsync_TextIsNullOrWhiteSpace_ReturnNull()
    {
        // This also checks for chat or user not existing
        TestHelper.SetAuthorizationReturns(
            _authorizationServiceMock, true);
        TestHelper.SetChatExists(_chatRepositoryMock, true, 42);
        TestHelper.SetUserExists(_userRepositoryMock, true, "uId");

        var result1 = await _messageService.SaveMessageAsync(
            "", "", "", -1);
        var result2 = await _messageService.SaveMessageAsync(
            "", "", "   ", -1);
        var result3 = await _messageService.SaveMessageAsync(
            "", "", null, -1);
        
        _messageRepositoryMock.Verify(repo =>
            repo.InsertAsync(It.IsAny<Message>()), Times.Never);
        Assert.IsNull(result1);
        Assert.IsNull(result2);
        Assert.IsNull(result3);
    }
    
    [Test]
    public async Task SaveMessageAsync_Success_ReturnMessage()
    {
        TestHelper.SetAuthorizationReturns(
            _authorizationServiceMock, true);
        TestHelper.SetChatExists(_chatRepositoryMock, true, 42);
        TestHelper.SetUserExists(_userRepositoryMock, true, "uId");

        var result = await _messageService.SaveMessageAsync(
            "", "", "test", -1);

        _authorizationServiceMock.Verify(auth => 
            auth.IsUserInChatAsync(It.IsAny<string>(), 
                It.IsAny<string>()), Times.Once);
        _messageRepositoryMock.Verify(repo =>
            repo.InsertAsync(It.IsAny<Message>()), Times.Once);
        Assert.IsNotNull(result);
    }

    [Test]
    public async Task EditMessageAsync_AuthorizationFailed_DoNotEdit()
    {
        TestHelper.SetAuthorizationReturns(
            _authorizationServiceMock, false);

        await _messageService.EditMessageAsync(
            "", 0, "test");
        
        _authorizationServiceMock.Verify(auth =>
            auth.IsMessageByUserAsync(It.IsAny<int>(), 
                It.IsAny<string>()), Times.Once);
        _messageRepositoryMock.Verify(repo =>
            repo.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _messageRepositoryMock.Verify(repo =>
            repo.Update(It.IsAny<Message>()), Times.Never);
    }
    
    [Test]
    public async Task EditMessageAsync_TextIsNullOrWhiteSpace_DoNotEdit()
    {
        TestHelper.SetAuthorizationReturns(
            _authorizationServiceMock, true);

        await _messageService.EditMessageAsync(
            "", 0, "");
        await _messageService.EditMessageAsync(
            "", 0, "   ");
        await _messageService.EditMessageAsync(
            "", 0, null);
        
        _messageRepositoryMock.Verify(repo =>
            repo.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _messageRepositoryMock.Verify(repo =>
            repo.Update(It.IsAny<Message>()), Times.Never);
    }
    
    [Test]
    public async Task EditMessageAsync_AuthorizationSuccessful_EditMessage()
    {
        TestHelper.SetAuthorizationReturns(
            _authorizationServiceMock, true);
        TestHelper.SetMessageExists(_messageRepositoryMock, true);

        await _messageService.EditMessageAsync(
            "", 0, "test");
        
        _authorizationServiceMock.Verify(auth =>
            auth.IsMessageByUserAsync(It.IsAny<int>(), 
                It.IsAny<string>()), Times.Once);
        _messageRepositoryMock.Verify(repo =>
            repo.GetByIdAsync(It.IsAny<int>()), Times.Once);
        _messageRepositoryMock.Verify(repo =>
            repo.Update(It.IsAny<Message>()), Times.Once);
    }

    [Test]
    public async Task DeleteMessageAsync_AuthorizationFailed_DoNotDelete()
    {
        TestHelper.SetAuthorizationReturns(
            _authorizationServiceMock, false);

        await _messageService.DeleteMessageAsync("", 0);
        
        _authorizationServiceMock.Verify(auth =>
            auth.IsMessageByUserAsync(It.IsAny<int>(), 
                It.IsAny<string>()), Times.Once);
        _messageRepositoryMock.Verify(repo =>
            repo.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _messageRepositoryMock.Verify(repo =>
            repo.Delete(It.IsAny<Message>()), Times.Never);
    }
    
    [Test]
    public async Task DeleteMessageAsync_AuthorizationSuccessful_DeleteMessage()
    {
        TestHelper.SetAuthorizationReturns(
            _authorizationServiceMock, true);
        TestHelper.SetMessageExists(_messageRepositoryMock, true);

        await _messageService.DeleteMessageAsync("", 0);
        
        _authorizationServiceMock.Verify(auth =>
            auth.IsMessageByUserAsync(It.IsAny<int>(), 
                It.IsAny<string>()), Times.Once);
        _messageRepositoryMock.Verify(repo =>
            repo.GetByIdAsync(It.IsAny<int>()), Times.Once);
        _messageRepositoryMock.Verify(repo =>
            repo.Delete(It.IsAny<Message>()), Times.Once);
    }

    [Test]
    public async Task
        DeleteMessageForUserAsync_AuthorizationFailed_DoNotDelete()
    {
        TestHelper.SetAuthorizationReturns(
            _authorizationServiceMock, false);

        await _messageService.DeleteMessageForUserAsync("", 0);
        
        _authorizationServiceMock.Verify(auth =>
            auth.IsMessageByUserAsync(It.IsAny<int>(), 
                It.IsAny<string>()), Times.Once);
        _userRepositoryMock.Verify(repo =>
            repo.GetByLoginAsync(It.IsAny<string>()), Times.Never);
        _messageDeletedForUserRepositoryMock.Verify(
            repo => 
                repo.InsertAsync(
                    It.IsAny<MessageDeletedForUser>()), Times.Never);
    }
    
    [Test]
    public async Task
        DeleteMessageForUserAsync_AuthorizationSuccessful_AddEntry()
    {
        TestHelper.SetAuthorizationReturns(
            _authorizationServiceMock, true);
        TestHelper.SetUserExists(_userRepositoryMock, true);

        await _messageService.DeleteMessageForUserAsync("", 0);
        
        _authorizationServiceMock.Verify(auth =>
            auth.IsMessageByUserAsync(It.IsAny<int>(), 
                It.IsAny<string>()), Times.Once);
        _userRepositoryMock.Verify(repo =>
            repo.GetByLoginAsync(It.IsAny<string>()), Times.Once);
        _messageDeletedForUserRepositoryMock.Verify(
            repo => 
                repo.InsertAsync(
                    It.IsAny<MessageDeletedForUser>()), Times.Once);
    }
}