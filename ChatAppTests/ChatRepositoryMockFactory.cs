using ChatApp.DAL.Entities;
using ChatApp.DAL.Repositories;
using ChatApp.ViewModels;
using Moq;

namespace ChatAppTests;

public static class ChatRepositoryMockFactory
{
    public static Mock<ChatRepository> GetMock()
    {
        var mock = new Mock<ChatRepository>();

        mock.Setup(repo => repo.Delete(It.IsAny<Chat>()))
            .Callback((Chat c) => DataProvider.Chats.Remove(c));

        mock.Setup(repo => repo.InsertAsync(
                It.IsAny<Chat>()))
            .Callback((Chat c) => DataProvider.Chats.Add(c));

        mock.Setup(repo => repo.GetByIdAsync(
                It.IsAny<int>()))
            .ReturnsAsync((int id) => DataProvider.Chats.SingleOrDefault(
                c => c.Id == id));

        mock.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(DataProvider.Chats);

        mock.Setup(repo => repo.GetByNameAsync(
                It.IsAny<string>()))
            .ReturnsAsync((string name) =>
                DataProvider.Chats.SingleOrDefault(c => c.Name == name));

        mock.Setup(repo => repo.GetGroupsAsync(
                It.IsAny<string>(), It.IsAny<int>(),
                It.IsAny<int>(), It.IsAny<string>(),
                It.IsAny<bool>()))
            .ReturnsAsync((string userId, int skip, int batchSize,
                string sortBy, bool sortDesc) =>
            {
                var queryable = DataProvider.Chats
                    .Where(c => DataProvider.MembersChats
                        .Where(mc => mc.UserId == userId)
                        .Select(mc => mc.ChatId)
                        .Contains(c.Id));

                var action = (Chat chat) => chat.GetType()
                    .GetProperty(sortBy)!.GetValue(chat)!;

                queryable = sortDesc
                    ? queryable.OrderByDescending(action)
                    : queryable.OrderBy(action);

                return queryable
                    .Skip(skip)
                    .Take(batchSize)
                    .Select(c => new ChatView
                    {
                        Name = c.Name,
                        LatestMessageDateTime = DataProvider.Messages
                            .Where(m => m.ChatId == c.Id)
                            .Select(m => m.DateTime)
                            .Max()
                    });
            });

        return mock;
    }
}