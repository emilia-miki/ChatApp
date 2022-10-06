using ChatApp.DAL;
using ChatApp.DAL.Repositories;
using Moq;

namespace ChatAppTests;

public static class UnitOfWorkMockFactory
{
    public static Mock<IUnitOfWork> GetMock(ChatRepository chatRepository,
        UserRepository userRepository, MessageRepository messageRepository)
    {
        var mock = new Mock<IUnitOfWork>();

        mock.Setup(uow => 
                uow.GetRepository<ChatRepository>())
            .Returns(chatRepository);
        
        mock.Setup(uow => 
                uow.GetRepository<UserRepository>())
            .Returns(userRepository);
        
        mock.Setup(uow => 
                uow.GetRepository<MessageRepository>())
            .Returns(messageRepository);

        return mock;
    }
}