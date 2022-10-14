using ChatApp.BLL.Interfaces;
using ChatApp.BLL.Services;
using ChatApp.DAL;
using ChatApp.DAL.Repositories;
using Moq;
using NUnit.Framework;

namespace ChatAppTests.UnitTests.ServiceTests;

[TestFixture]
public class ViewServiceTests
{
    private Mock<UserRepository> _userRepositoryMock = null!;
    private Mock<MessageRepository> _messageRepositoryMock = null!;
    private Mock<ChatRepository> _chatRepositoryMock = null!;
    private Mock<IUnitOfWork> _unitOfWorkMock = null!;
    private Mock<IAuthorizationService> _authorizationServiceMock = null!;
    private IViewService _viewService = null!;

    [SetUp]
    public void SetUp()
    {
        _userRepositoryMock = new Mock<UserRepository>();
        _messageRepositoryMock = new Mock<MessageRepository>();
        _chatRepositoryMock = new Mock<ChatRepository>();
        
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        
        _unitOfWorkMock.Setup(uow =>
                uow.GetRepository<ChatRepository>())
            .Returns(_chatRepositoryMock.Object);
        _unitOfWorkMock.Setup(uow =>
                uow.GetRepository<MessageRepository>())
            .Returns(_messageRepositoryMock.Object);
        _unitOfWorkMock.Setup(uow =>
                uow.GetRepository<UserRepository>())
            .Returns(_userRepositoryMock.Object);
        
        _viewService = new ViewService(_unitOfWorkMock.Object,
            _authorizationServiceMock.Object);
    }
    
    [Test]
    public async Task GetMessageBatchAsync_ChatDoesNotExist_ReturnEmptyList()
    {
        TestHelper.SetAuthorizationReturns(_authorizationServiceMock, true);
        TestHelper.SetChatExists(_chatRepositoryMock, false);

        var result = await _viewService
            .GetMessageBatchAsync("", "", 0, 10);
        
        _messageRepositoryMock.Verify(repo => 
            repo.GetMessagesAsync(It.IsAny<string>(), It.IsAny<int>(),
                It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        Assert.IsEmpty(result);
    }

    [Test]
    public async Task GetMessageBatchAsync_AuthorizationFailed_ReturnEmptyList()
    {
        TestHelper.SetAuthorizationReturns(
            _authorizationServiceMock, false);
        TestHelper.SetChatExists(_chatRepositoryMock, true);

        var result = await _viewService.GetMessageBatchAsync(
            "", "", 0, 10);
        
        _messageRepositoryMock.Verify(repo => 
            repo.GetMessagesAsync(It.IsAny<string>(), It.IsAny<int>(), 
                It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        Assert.IsEmpty(result);
    }
    
    [Test]
    public async Task GetMessageBatchAsync_Success_ReturnMessageViews()
    {
        TestHelper.SetAuthorizationReturns(
            _authorizationServiceMock, true);
        TestHelper.SetUserExists(_userRepositoryMock, true);
        TestHelper.SetChatExists(_chatRepositoryMock, true);

        await _viewService.GetMessageBatchAsync(
            "", "", 0, 10);
        
        _messageRepositoryMock.Verify(repo => 
            repo.GetMessagesAsync(It.IsAny<string>(), It.IsAny<int>(), 
                It.IsAny<int>(), It.IsAny<int>()), Times.Once);
    }
    
    [Test]
    public async Task GetUsersAsync_UserDoesNotExist_ReturnEmptyList()
    {
        TestHelper.SetUserExists(_userRepositoryMock, false);

        var result = await _viewService.GetUsersAsync(
            "", 0, 10, "Login", false);
        
        _userRepositoryMock.Verify(repo => 
            repo.GetUserViewsAsync(It.IsAny<string>(), It.IsAny<int>(), 
                It.IsAny<int>(), It.IsAny<string>(), 
                It.IsAny<bool>()), Times.Never);
        Assert.IsEmpty(result);
    }
    
    [Test]
    public async Task GetUsersAsync_Success_ReturnUserViews()
    {
        TestHelper.SetUserExists(_userRepositoryMock, true);

        await _viewService.GetUsersAsync(
            "", 0, 10, "Login", false);
        
        _userRepositoryMock.Verify(repo => 
            repo.GetUserViewsAsync(It.IsAny<string>(), It.IsAny<int>(), 
                It.IsAny<int>(), It.IsAny<string>(), 
                It.IsAny<bool>()), Times.Once);
    }

    [Test]
    public async Task GetGroupsAsync_UserDoesNotExist_ReturnEmptyList()
    {
        TestHelper.SetUserExists(_userRepositoryMock, false);

        await _viewService.GetGroupsAsync(
            "", 0, 10, "Name", false);
        
        _chatRepositoryMock.Verify(repo => 
            repo.GetGroupsAsync(It.IsAny<string>(), It.IsAny<int>(), 
                It.IsAny<int>(), It.IsAny<string>(), 
                It.IsAny<bool>()), Times.Never);
    }
    
    [Test]
    public async Task GetGroupsAsync_Success_ReturnChatViews()
    {
        TestHelper.SetUserExists(_userRepositoryMock, true);

        await _viewService.GetGroupsAsync(
            "", 0, 10, "Name", false);
        
        _chatRepositoryMock.Verify(repo => 
            repo.GetGroupsAsync(It.IsAny<string>(), It.IsAny<int>(), 
                It.IsAny<int>(), It.IsAny<string>(), 
                It.IsAny<bool>()), Times.Once);
    }
}