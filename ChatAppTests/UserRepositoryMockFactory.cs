using ChatApp.DAL.Entities;
using ChatApp.DAL.Repositories;
using ChatApp.ViewModels;
using Moq;

namespace ChatAppTests;

public static class UserRepositoryMockFactory
{
    public static Mock<UserRepository> GetMock()
    {
        var mock = new Mock<UserRepository>();
        
        mock.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(DataProvider.Users);
        
        mock.Setup(repo => 
                repo.InsertAsync(It.IsAny<ApplicationUser>()))
            .Callback((ApplicationUser u) =>
            {
                DataProvider.Users.Add(u);
            });
        
        mock.Setup(repo => 
                repo.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((string id) => DataProvider.Users
                .SingleOrDefault(u => u.Id == id));

        mock.Setup(repo =>
                repo.GetByLoginAsync(It.IsAny<string>()))
            .ReturnsAsync((string login) => DataProvider.Users
                .SingleOrDefault(u => u.UserName == login));

        mock.Setup(repo =>
                repo.Delete(It.IsAny<ApplicationUser>()))
            .Callback((ApplicationUser u) => DataProvider.Users.Remove(u));

        mock.Setup(repo => repo.GetUserViewsAsync(
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync((string userId, int skip, int batchSize,
                string? sortBy, bool sortDesc) =>
            {
                var queryable = DataProvider.Users
                    .Where(u => u.Id != userId)
                    .Select(u => new UserView
                    {
                        Login = u.UserName,
                        Email = u.Email,
                        Phone = u.PhoneNumber
                    })
                    .Skip(skip)
                    .Take(batchSize);

                object Action(UserView u) =>
                    sortBy == null
                        ? u.Login
                        : u.GetType().GetProperty(sortBy)!.GetValue(u)!;

                return sortDesc
                    ? queryable.OrderByDescending(Action)
                    : queryable.OrderBy(Action);
            });

        return mock;
    }
}