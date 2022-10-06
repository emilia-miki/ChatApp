using System.Text;
using ChatApp.BLL.Interfaces;
using ChatApp.BLL.Models;
using ChatApp.BLL.Services;
using ChatApp.DAL.Entities;
using ChatApp.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NUnit.Framework;

namespace ChatAppTests.UnitTests.ServiceTests;

[TestFixture]
public class AccountServiceTests
{
    private Mock<UserManager<ApplicationUser>> _userManagerMock = null!;
    private Mock<SignInManager<ApplicationUser>> _signInManagerMock = null!;
    private Mock<IAuthOptions> _authOptionsMock = null!;
    private IAccountService _accountService = null!;

    [SetUp]
    public void SetUp()
    {
        var storeMock = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            storeMock.Object, null, null, null, null, 
            null, null, null, null);
        _userManagerMock.Setup(manager =>
                manager.CreateAsync(It.IsAny<ApplicationUser>(),
                    It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser _, string _) =>
                new IdentityResult());
        var contextAccessorMock = new Mock<IHttpContextAccessor>();
        var claimsFactoryMock =
            new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
        _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
            _userManagerMock.Object, contextAccessorMock.Object, 
            claimsFactoryMock.Object, null, null, null, null);
        _signInManagerMock.Setup(manager =>
                manager.CheckPasswordSignInAsync(It.IsAny<ApplicationUser>(),
                    It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync((ApplicationUser _, string _, bool _) =>
                new SignInResult());
        _authOptionsMock = new Mock<IAuthOptions>();
        _accountService = new AccountService(_userManagerMock.Object,
            _signInManagerMock.Object, _authOptionsMock.Object);
    }

    [Test]
    public async Task IsValidUserCredentialsAsync_UserDoesNotExist_ReturnFalse()
    {
        TestHelper.SetUserExists(_userManagerMock, false);

        var result = await _accountService.IsValidUserCredentialsAsync(
            new LoginUser("", ""));
        
        _userManagerMock.Verify(manager =>
            manager.FindByNameAsync(It.IsAny<string>()), Times.Once);
        _signInManagerMock.Verify(manager =>
            manager.CheckPasswordSignInAsync(It.IsAny<ApplicationUser>(), 
                It.IsAny<string>(), It.IsAny<bool>()),
            Times.Never);
        Assert.IsFalse(result);
    } 
    
    [Test]
    public async Task IsValidUserCredentialsAsync_UserExists_CheckPassword()
    {
        TestHelper.SetUserExists(_userManagerMock, true);

        await _accountService.IsValidUserCredentialsAsync(
            new LoginUser("", ""));
        
        _userManagerMock.Verify(manager =>
            manager.FindByNameAsync(It.IsAny<string>()), Times.Once);
        _signInManagerMock.Verify(manager =>
                manager.CheckPasswordSignInAsync(It.IsAny<ApplicationUser>(), 
                    It.IsAny<string>(), It.IsAny<bool>()),
            Times.Once);
    }

    [Test]
    public async Task RegisterAsync_RegistersUser()
    {
        await _accountService.RegisterAsync(new RegisterViewModel());
        
        _userManagerMock.Verify(manager =>
            manager.CreateAsync(It.IsAny<ApplicationUser>(), 
                It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task LoginAsync_ReturnsToken()
    {
        TestHelper.SetUserExists(_userManagerMock, true);
        _userManagerMock.Setup(manager =>
                manager.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync((ApplicationUser _) => null);
        _authOptionsMock.Setup(options => options.Key)
            .Returns(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("jfaskdjasdnlfkansdlknasldf")));
        _authOptionsMock.Setup(options => options.Issuer)
            .Returns("issuer");
        _authOptionsMock.Setup(options => options.Audience)
            .Returns("audience");
        _authOptionsMock.Setup(options => options.Lifetime)
            .Returns(1);
        
        const string myUserName = "myUserName";
        
        var result = await _accountService.LoginAsync(
            new LoginUser(myUserName, ""));
        
        _userManagerMock.Verify(manager =>
            manager.FindByNameAsync(It.IsAny<string>()), Times.Once);
        Assert.IsNotNull(result.AccessToken);
        Assert.IsNotEmpty(result.AccessToken);
        Assert.AreEqual(myUserName,result.UserName);
    }

    [Test]
    public async Task ChangePasswordAsync_ChangesPassword()
    {
        TestHelper.SetUserExists(_userManagerMock, true);
        _userManagerMock.Setup(manager =>
                manager.GeneratePasswordResetTokenAsync(
                    It.IsAny<ApplicationUser>()))
            .ReturnsAsync((ApplicationUser _) => "");
        
        await _accountService.ChangePasswordAsync("", "");
        
        _userManagerMock.Verify(manager =>
            manager.FindByNameAsync(It.IsAny<string>()), Times.Once);
        _userManagerMock.Verify(manager =>
            manager.GeneratePasswordResetTokenAsync(
                It.IsAny<ApplicationUser>()), Times.Once);
        _userManagerMock.Verify(manager =>
            manager.ResetPasswordAsync(It.IsAny<ApplicationUser>(), 
                It.IsAny<string>(), It.IsAny<string>()), 
            Times.Once);
    }

    [Test]
    public async Task DeleteAsync_DeletesUser()
    {
        TestHelper.SetUserExists(_userManagerMock, true);

        await _accountService.DeleteAsync("");
        
        _userManagerMock.Verify(manager => 
            manager.FindByNameAsync(It.IsAny<string>()), Times.Once);
        _userManagerMock.Verify(manager =>
            manager.DeleteAsync(It.IsAny<ApplicationUser>()), Times.Once);
    }
}