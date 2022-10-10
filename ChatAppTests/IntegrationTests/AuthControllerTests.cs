using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using ChatApp.ViewModels;
using NUnit.Framework;

namespace ChatAppTests.IntegrationTests;

public class AuthControllerTests : IntegrationTest
{
    [SetUp]
    public async Task SetUp()
    {
        await ResetDatabase();
    }

    [Test]
    public async Task POST_Register_RegistersUserExceptDuplicates()
    {
        var response1 = await Client.PostAsJsonAsync("/auth/register",
            new RegisterViewModel
            {
                Login = "user",
                Email = "email@mail.com",
                Password = "Password1!",
                PasswordConfirmation = "Password1!",
                PhoneNumber = "380681117799"
            });
        var response2 = await Client.PostAsJsonAsync("/auth/register",
            new RegisterViewModel
            {
                Login = "user",
                Email = "email2@mail.com",
                Password = "Password2!",
                PasswordConfirmation = "Password2!",
                PhoneNumber = "380691117799"
            });
        
        Assert.AreEqual(HttpStatusCode.OK, response1.StatusCode);
        var result1 = await response1.Content
            .ReadFromJsonAsync<LoginResult>();
        Assert.IsNotNull(result1);
        Assert.AreEqual("user", result1!.UserName);
        Assert.IsNotNull(result1.AccessToken);
        Assert.IsNotEmpty(result1.AccessToken);

        Assert.AreEqual(
            HttpStatusCode.BadRequest, response2.StatusCode);
        var result2 = await response2.Content
            .ReadFromJsonAsync<Dictionary<string, List<string>>>();
        Assert.IsNotNull(result2);
        Assert.IsNotEmpty(result2!);
    }

    [Test]
    public async Task POST_Register_InvalidModelState_ReturnBadRequest()
    {
        var response = await Client.PostAsJsonAsync("/auth/register",
            new RegisterViewModel
            {
                Login = "",
                Password = "",
                Email = "",
                PasswordConfirmation = "",
                PhoneNumber = ""
            });
        
        Assert.AreEqual(
            HttpStatusCode.BadRequest, response.StatusCode);
        var result = await response.Content
            .ReadFromJsonAsync<Dictionary<string, List<string>>>();
        Assert.IsNotNull(result);
        Assert.IsNotEmpty(result!);
    }

    [Test]
    public async Task POST_Login_InvalidModelState_ReturnBadRequest()
    {
        var response = await Client.PostAsJsonAsync("/auth/login",
            new LoginViewModel
            {
                Login = "user",
                Password = "weak"
            });
        
        Assert.AreEqual(
            HttpStatusCode.BadRequest, response.StatusCode);
        var result = await response.Content
            .ReadFromJsonAsync<Dictionary<string, List<string>>>();
        Assert.IsNotNull(result);
        Assert.IsNotEmpty(result!);
    }

    [Test]
    public async Task POST_Login_UserDoesNotExist_ReturnUnauthorized()
    {
        var response = await Client.PostAsJsonAsync("/auth/login",
            new LoginViewModel
            {
                Login = "username",
                Password = "Password1!"
            });
        
        Assert.AreEqual(
            HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Test]
    public async Task POST_Login_WrongPassword_ReturnUnauthorized()
    {
        await Client.PostAsJsonAsync("/auth/register",
            new RegisterViewModel
            {
                Login = "username",
                Password = "Password1!",
                Email = "email@mail.com",
                PasswordConfirmation = "Password1!",
                PhoneNumber = "380681117799"
            });

        var response = await Client.PostAsJsonAsync("/auth/login",
            new LoginViewModel
            {
                Login = "username",
                Password = "Password2!"
            });
        
        Assert.AreEqual(
            HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Test]
    public async Task POST_Login_ValidCredentials_ReturnOk()
    {
        await Client.PostAsJsonAsync("/auth/register",
            new RegisterViewModel
            {
                Login = "username",
                Password = "Password1!",
                Email = "email@mail.com",
                PasswordConfirmation = "Password1!",
                PhoneNumber = "380681117799"
            });

        var response = await Client.PostAsJsonAsync("/auth/login",
            new LoginViewModel
            {
                Login = "username",
                Password = "Password1!"
            });
        
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content
            .ReadFromJsonAsync<LoginResult>();
        Assert.IsNotNull(result);
        Assert.IsNotNull(result!.AccessToken);
        Assert.IsNotEmpty(result.AccessToken);
        Assert.AreEqual("username", result.UserName);
    }
    
    private async Task POST_ChangePassword(bool validModel, bool validAuth)
    {
        // Register user
        var response = await Client.PostAsJsonAsync("/auth/register",
            new RegisterViewModel
            {
                Login = "username",
                Password = "Password1!",
                PasswordConfirmation = "Password1!",
                Email = "email@mail.com",
                PhoneNumber = "380681117799"
            });
        var loginResult = await response.Content
            .ReadFromJsonAsync<LoginResult>();
        
        var request = new HttpRequestMessage(
            HttpMethod.Post, "/auth/change");
        if (validAuth)
        {
            request.Headers.Add("Authorization", 
                new AuthenticationHeaderValue(
                    "Bearer", loginResult!.AccessToken)
                    .ToString());
        }
        else
        {
            request.Headers.Add("Authorization", 
                new AuthenticationHeaderValue(
                    "Bearer", loginResult!.AccessToken[..^2])
                    .ToString());
        }

        var confirm = validModel ? "Password2!" : "Password3!";
        request.Content = new StringContent(
            JsonSerializer.Serialize(new PasswordChangeViewModel
            {
                OldPassword = "Password1!",
                NewPassword = "Password2!",
                ConfirmNewPassword = confirm
            }), Encoding.UTF8, "application/json");
        
        response = await Client.SendAsync(request);
        if (validAuth && validModel)
        {
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content
                .ReadFromJsonAsync<LoginResult>();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result!.AccessToken);
            Assert.IsNotEmpty(result.AccessToken);
            Assert.AreEqual("username", result.UserName);
        }

        if (!validAuth && validModel)
        {
            Assert.AreEqual(
                HttpStatusCode.Unauthorized, response.StatusCode);
        }

        if (validAuth && !validModel)
        {
            Assert.AreEqual(
                HttpStatusCode.BadRequest, response.StatusCode);
            var result = await response.Content
                .ReadFromJsonAsync<Dictionary<string, List<string>>>();
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result!);
        }
    }

    [Test]
    public async Task POST_ChangePassword_Success_ReturnOk()
    {
        await POST_ChangePassword(true, true);
    }

    [Test]
    public async Task POST_ChangePassword_InvalidModel_ReturnBadRequest()
    {
        await POST_ChangePassword(false, true);
    }

    [Test]
    public async Task POST_ChangePassword_AuthFailed_ReturnUnauthorized()
    {
        await POST_ChangePassword(true, false);
    }

    [Test]
    public async Task POST_Delete_DeleteAndReturnOk()
    {
        var response = await Client.PostAsJsonAsync("/auth/register",
            new RegisterViewModel
            {
                Login = "username",
                Password = "Password1!",
                Email = "email@mail.com",
                PasswordConfirmation = "Password1!",
                PhoneNumber = "380681117799"
            });
        var loginResult = await response.Content
            .ReadFromJsonAsync<LoginResult>();
        
        // Return Ok
        var request = new HttpRequestMessage(
            HttpMethod.Post, "/auth/delete");
        request.Headers.Add("Authorization", 
            new AuthenticationHeaderValue(
                "Bearer", loginResult!.AccessToken).ToString());
        response = await Client.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        
        // Delete account
        response = await Client.PostAsJsonAsync("/auth/login",
            new LoginViewModel
            {
                Login = "username",
                Password = "Password1!"
            });
        Assert.AreEqual(
            HttpStatusCode.Unauthorized, response.StatusCode);
    }
}