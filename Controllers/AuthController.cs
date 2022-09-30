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
    public async Task<IActionResult> Register(
        [FromBody] RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var registerResult = await _accountService.Register(model);

        if (registerResult.Succeeded)
        {
            return Ok(await _accountService.Login(
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
    public async Task<IActionResult> Login([FromBody] LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await _accountService.IsValidUserCredentials(
                new LoginUser(model.Login, model.Password)))
        {
            return Unauthorized();
        }

        return Ok(await _accountService.Login(
            new LoginUser(model.Login, model.Password)));
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    [Route("/auth/change")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] PasswordChangeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var login = HttpContext.User.Identity!.Name!;
        if (!await _accountService.IsValidUserCredentials(
                new LoginUser(login, model.OldPassword)))
        {
            return Unauthorized();
        }

        var changeResult = await _accountService.ChangePassword(
            HttpContext, model.NewPassword);

        if (changeResult.Succeeded)
        {
            return Ok(await _accountService.Login(
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
    public async Task<IActionResult> Delete()
    {
        await _accountService.Delete(HttpContext);
        return Ok();
    }
}