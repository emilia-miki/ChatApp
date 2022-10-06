using ChatApp.BLL.Interfaces;
using ChatApp.BLL.Services;
using ChatApp.DAL;
using ChatApp.DAL.Entities;
using ChatApp.DAL.Repositories;
using Moq;
using NUnit.Framework;

namespace ChatAppTests.UnitTests.ServiceTests;

[TestFixture]
public class ChatServiceTests
{
    private Mock<UserRepository> _userRepositoryMock = null!;
    private Mock<ChatRepository> _chatRepositoryMock = null!;
    private Mock<MemberChatRepository> _memberChatRepositoryMock = null!;
    private Mock<IUnitOfWork> _unitOfWorkMock = null!;
    private IChatService _chatService = null!;

    [SetUp]
    public void SetUp()
    {
        _userRepositoryMock = new Mock<UserRepository>();
        _chatRepositoryMock = new Mock<ChatRepository>();
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

        _chatService = new ChatService(_unitOfWorkMock.Object);
    }

    [Test]
    public void CreateIfNotExistsAsync_ChatAlreadyExists_DoNotCreateNewChat()
    {
        TestHelper.SetChatExists(_chatRepositoryMock, true);
        TestHelper.SetUserExists(_userRepositoryMock, true);

        _chatService.CreatePersonalChatIfNotExistsAsync("james", "john");
        
        _chatRepositoryMock.Verify(repo => 
            repo.GetByNameAsync(It.IsAny<string>()), Times.Once);
        _chatRepositoryMock.Verify(repo =>
            repo.InsertAsync(It.IsAny<Chat>()), Times.Never);
        _userRepositoryMock.Verify(repo =>
            repo.GetByLoginAsync(It.IsAny<string>()), Times.Never);
        _memberChatRepositoryMock.Verify(repo =>
            repo.InsertAsync(It.IsAny<MemberChat>()), Times.Never);
        _unitOfWorkMock.Verify(uow =>
            uow.SaveAsync(), Times.Never);
    }
    
    [Test]
    public void CreateIfNotExistsAsync_UserDoesNotExist_DoNotCreateNewChat()
    {
        TestHelper.SetChatExists(_chatRepositoryMock, false);
        TestHelper.SetUserExists(_userRepositoryMock, false);

        _chatService.CreatePersonalChatIfNotExistsAsync("james", "john");
        
        _chatRepositoryMock.Verify(repo => 
            repo.GetByNameAsync(It.IsAny<string>()), Times.Once);
        _chatRepositoryMock.Verify(repo =>
            repo.InsertAsync(It.IsAny<Chat>()), Times.Never);
        _userRepositoryMock.Verify(repo =>
            repo.GetByLoginAsync(It.IsAny<string>()), Times.Once);
        _memberChatRepositoryMock.Verify(repo =>
            repo.InsertAsync(It.IsAny<MemberChat>()), Times.Never);
        _unitOfWorkMock.Verify(uow =>
            uow.SaveAsync(), Times.Never);
    }

    [Test]
    public void CreateIfNotExistsAsync_ValidParameters_CreateNewChat()
    {
        TestHelper.SetChatExists(_chatRepositoryMock, false);
        TestHelper.SetUserExists(_userRepositoryMock, true);

        _chatService.CreatePersonalChatIfNotExistsAsync("james", "john");
        
        _chatRepositoryMock.Verify(repo => 
            repo.GetByNameAsync(It.IsAny<string>()), Times.Once);
        _userRepositoryMock.Verify(repo =>
            repo.GetByLoginAsync(It.IsAny<string>()), Times.Exactly(2));
        _chatRepositoryMock.Verify(repo =>
            repo.InsertAsync(It.IsAny<Chat>()), Times.Once);
        _memberChatRepositoryMock.Verify(repo =>
            repo.InsertAsync(It.IsAny<MemberChat>()), Times.Exactly(2));
        _unitOfWorkMock.Verify(uow =>
            uow.SaveAsync(), Times.Exactly(2));
    }
}