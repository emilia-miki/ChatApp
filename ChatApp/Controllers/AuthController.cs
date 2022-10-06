using ChatApp.BLL.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChatApp.BLL.Models;
using ChatApp.ViewModels;

namespace ChatApp.Controllers;

public class AuthController : Controller
{
    private readonly IAccountService _accountService;

    public AuthController(IAccountService accountService)
    {
        _accountService = accountService;
    }
    
    [AllowAnonymous]
    [HttpPost]
    [Route("/auth/register")]
    public async Task<IActionResult> RegisterAsync(
        [FromBody] RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var registerResult = await _accountService.RegisterAsync(model);

        if (registerResult.Succeeded)
        {
            return Ok(await _accountService.LoginAsync(
                new LoginUser(model.Login, model.Password)));
        }

        foreach (var error in registerResult.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return BadRequest(ModelState);
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("/auth/login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await _accountService.IsValidUserCredentialsAsync(
                new LoginUser(model.Login, model.Password)))
        {
            return Unauthorized();
        }

        return Ok(await _accountService.LoginAsync(
            new LoginUser(model.Login, model.Password)));
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    [Route("/auth/change")]
    public async Task<IActionResult> ChangePasswordAsync(
        [FromBody] PasswordChangeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var login = ControllerContext.HttpContext.User.Identity!.Name!;
        if (!await _accountService.IsValidUserCredentialsAsync(
                new LoginUser(login, model.OldPassword)))
        {
            return Unauthorized();
        }

        var changeResult = await _accountService.ChangePasswordAsync(
            login, model.NewPassword);

        if (changeResult.Succeeded)
        {
            return Ok(await _accountService.LoginAsync(
                new LoginUser(login, model.NewPassword)));
        }

        foreach (var error in changeResult.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return BadRequest(ModelState);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    [Route("/auth/delete")]
    public async Task<IActionResult> DeleteAsync()
    {
        await _accountService.DeleteAsync(
            ControllerContext.HttpContext.User.Identity!.Name!);
        return Ok();
    }
}