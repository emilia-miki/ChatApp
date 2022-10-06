using ChatApp.DAL.Entities;
using ChatApp.DAL.Repositories;
using ChatApp.ViewModels;
using Moq;

namespace ChatAppTests;

public static class MessageRepositoryMockFactory
{
    public static Mock<MessageRepository> GetMock()
    {
        var mock = new Mock<MessageRepository>();

        mock.Setup(repo => repo.GetMessagesAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((int chatId, int skip, int batchSize) => 
                DataProvider.Messages
                    .Where(m => m.ChatId == chatId)
                    .OrderByDescending(m => m.DateTime)
                    .Skip(skip)
                    .Take(batchSize)
                    .Select(m => new MessageView
                    {
                        Id = m.Id,
                        Text = m.Text,
                        DateTime = m.DateTime,
                        ReplyTo = m.ReplyTo,
                        UserName = DataProvider.Users.Single(u =>
                            u.Id == m.UserId).UserName
                    })
            );

        mock.Setup(repo => repo.InsertAsync(
                It.IsAny<Message>()))
            .Callback((Message m) => DataProvider.Messages.Add(m));

        mock.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(DataProvider.Messages);

        mock.Setup(repo => repo.GetByIdAsync(
                It.IsAny<int>()))
            .ReturnsAsync((int id) => DataProvider.Messages.SingleOrDefault(
                m => m.Id == id));

        mock.Setup(repo => repo.Delete(
                It.IsAny<Message>()))
            .Callback((Message m) => DataProvider.Messages.Remove(m));

        return mock;
    }
} 