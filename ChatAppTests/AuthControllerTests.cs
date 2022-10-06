using System.Security.Principal;
using ChatApp.BLL.Interfaces;
using ChatApp.BLL.Models;
using ChatApp.Controllers;
using ChatApp.ViewModels;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;

namespace ChatAppTests;

[TestFixture]
public class AuthControllerTests
{
    private Mock<IAccountService> _accountServiceMock = null!;
    private AuthController _authController = null!;

    [SetUp]
    public void SetUp()
    {
        _accountServiceMock = new Mock<IAccountService>();
        _authController = new AuthController(_accountServiceMock.Object);
    }

    [Test]
    public async Task RegisterAsync_InvalidModelState_ReturnBadRequest()
    {
        _authController.ModelState.TryAddModelError("", "");
        
        var result = await _authController.RegisterAsync(
            new RegisterViewModel());
        
        _accountServiceMock.Verify(service => 
            service.RegisterAsync(It.IsAny<RegisterViewModel>()),
            Times.Never);
        _accountServiceMock.Verify(service => 
            service.LoginAsync(It.IsAny<LoginUser>()), Times.Never);
        Assert.AreEqual(_authController.BadRequest(
                _authController.ModelState).GetType(), 
            result.GetType());
    }

    [Test]
    public async Task RegisterAsync_RegisterFailed_ReturnBadRequest()
    {
        TestHelper.SetRegisterResult(_accountServiceMock, false);
        
        var result = await _authController.RegisterAsync(
            new RegisterViewModel());
        
        _accountServiceMock.Verify(service => 
            service.RegisterAsync(It.IsAny<RegisterViewModel>()), 
            Times.Once);
        _accountServiceMock.Verify(service => 
            service.LoginAsync(It.IsAny<LoginUser>()), Times.Never);
        Assert.AreEqual(_authController.BadRequest(new object())
                .GetType(), result.GetType());
    }
    
    [Test]
    public async Task RegisterAsync_RegisterSuccess_ReturnOk()
    {
        TestHelper.SetRegisterResult(_accountServiceMock, true);
        
        var result = await _authController.RegisterAsync(
            new RegisterViewModel());
        
        _accountServiceMock.Verify(service => 
            service.RegisterAsync(It.IsAny<RegisterViewModel>()), 
            Times.Once);
        _accountServiceMock.Verify(service => 
            service.LoginAsync(It.IsAny<LoginUser>()), Times.Once);
        Assert.AreEqual(_authController.Ok(new object()).GetType(), 
            result.GetType());
    }
    
    [Test]
    public async Task LoginAsync_InvalidModelState_ReturnBadRequest()
    {
        _authController.ModelState.TryAddModelError("", "");
        
        var result = await _authController.LoginAsync(new LoginViewModel());

        _accountServiceMock.Verify(service => 
            service.LoginAsync(It.IsAny<LoginUser>()), Times.Never);
        Assert.AreEqual(_authController.BadRequest(
                _authController.ModelState).GetType(), 
            result.GetType());
    }
    
    [Test]
    public async Task LoginAsync_InvalidUserCredentials_ReturnUnauthorized()
    {
        _accountServiceMock.Setup(service =>
                service.IsValidUserCredentialsAsync(It.IsAny<LoginUser>()))
            .ReturnsAsync(false);
        
        var result = await _authController.LoginAsync(new LoginViewModel());

        _accountServiceMock.Verify(service => 
            service.IsValidUserCredentialsAsync(It.IsAny<LoginUser>()),
            Times.Once);
        _accountServiceMock.Verify(service => 
            service.LoginAsync(It.IsAny<LoginUser>()), Times.Never);
        Assert.AreEqual(_authController.Unauthorized().GetType(), 
            result.GetType());
    }
    
    [Test]
    public async Task LoginAsync_Authorized_ReturnOk()
    {
        _accountServiceMock.Setup(service =>
                service.IsValidUserCredentialsAsync(It.IsAny<LoginUser>()))
            .ReturnsAsync(true);

        var result = await _authController.LoginAsync(new LoginViewModel());

        _accountServiceMock.Verify(service => 
                service.IsValidUserCredentialsAsync(It.IsAny<LoginUser>()),
            Times.Once);
        _accountServiceMock.Verify(service => 
            service.LoginAsync(It.IsAny<LoginUser>()), Times.Once);
        Assert.AreEqual(_authController.Ok(new object()).GetType(), 
            result.GetType());
    }
    
    [Test]
    public async Task ChangePasswordAsync_InvalidModelState_ReturnBadRequest()
    {
        _authController.ModelState.TryAddModelError("", "");
        
        var result = await _authController.ChangePasswordAsync(
            new PasswordChangeViewModel());

        _accountServiceMock.Verify(service =>
            service.ChangePasswordAsync(It.IsAny<string>(), 
                It.IsAny<string>()), Times.Never);
        _accountServiceMock.Verify(service =>
            service.LoginAsync(It.IsAny<LoginUser>()), Times.Never);
        Assert.AreEqual(_authController.BadRequest(
                _authController.ModelState).GetType(), 
            result.GetType());
    }
    
    [Test]
    public async Task 
        ChangePasswordAsync_InvalidUserCredentials_ReturnUnauthorized()
    {
        _accountServiceMock.Setup(service =>
                service.IsValidUserCredentialsAsync(It.IsAny<LoginUser>()))
            .ReturnsAsync((LoginUser _) => false);
        TestHelper.SetControllerContext(_authController);

        var result = await _authController.ChangePasswordAsync(
            new PasswordChangeViewModel());

        _accountServiceMock.Verify(service => 
            service.IsValidUserCredentialsAsync(It.IsAny<LoginUser>()),
            Times.Once);
        _accountServiceMock.Verify(service =>
            service.ChangePasswordAsync(It.IsAny<string>(), 
                It.IsAny<string>()), Times.Never);
        _accountServiceMock.Verify(service =>
            service.LoginAsync(It.IsAny<LoginUser>()), Times.Never);
        Assert.AreEqual(_authController.Unauthorized().GetType(), 
            result.GetType());
    }
    
    [Test]
    public async Task ChangePasswordAsync_ChangeFailed_ReturnBadRequest()
    {
        _accountServiceMock.Setup(service =>
                service.IsValidUserCredentialsAsync(It.IsAny<LoginUser>()))
            .ReturnsAsync((LoginUser _) => true);
        TestHelper.SetControllerContext(_authController);
        TestHelper.SetChangeResult(_accountServiceMock, false);
        
        var result = await _authController.ChangePasswordAsync(
            new PasswordChangeViewModel());

        _accountServiceMock.Verify(service => 
                service.IsValidUserCredentialsAsync(It.IsAny<LoginUser>()),
            Times.Once);
        _accountServiceMock.Verify(service =>
            service.ChangePasswordAsync(It.IsAny<string>(), 
                It.IsAny<string>()), Times.Once);
        _accountServiceMock.Verify(service =>
            service.LoginAsync(It.IsAny<LoginUser>()), Times.Never);
        Assert.AreEqual(_authController.BadRequest(new object())
                .GetType(), result.GetType());
    }
    
    [Test]
    public async Task ChangePasswordAsync_ChangeSuccessful_ReturnOk()
    {
        _accountServiceMock.Setup(service =>
                service.IsValidUserCredentialsAsync(It.IsAny<LoginUser>()))
            .ReturnsAsync((LoginUser _) => true);
        TestHelper.SetControllerContext(_authController);
        TestHelper.SetChangeResult(_accountServiceMock, true);
        
        var result = await _authController.ChangePasswordAsync(
            new PasswordChangeViewModel());

        _accountServiceMock.Verify(service => 
                service.IsValidUserCredentialsAsync(It.IsAny<LoginUser>()),
            Times.Once);
        _accountServiceMock.Verify(service =>
            service.ChangePasswordAsync(It.IsAny<string>(), 
                It.IsAny<string>()), Times.Once);
        _accountServiceMock.Verify(service =>
            service.LoginAsync(It.IsAny<LoginUser>()), Times.Once);
        Assert.AreEqual(_authController.Ok(new object()).GetType(), 
            result.GetType());
    }

    [Test]
    public async Task DeleteAsync_DeletesAccount_ReturnOk()
    {
        _authController.ControllerContext.HttpContext =
            new DefaultHttpContext();
        _authController.ControllerContext.HttpContext.User =
            new GenericPrincipal(
                new GenericIdentity(""), Array.Empty<string>());
        
        var result = await _authController.DeleteAsync();
        
        _accountServiceMock.Verify(service => 
            service.DeleteAsync(It.IsAny<string>()), Times.Once);
        Assert.AreEqual(_authController.Ok().GetType(), 
            result.GetType());
    }
}