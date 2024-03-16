using NomNom.Contracts.User;
using NomNomAPI.Models;
using NomNomAPI.ServiceErrors;
using NomNomAPI.Services.Users;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace NomNomAPI.Controllers;

public class UsersController : ApiController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult SignUp(SignUpRequest request)
    {
        ErrorOr<User> registerNewUserResult = Models.User.From(request);

        if (registerNewUserResult.IsError)
            return Problem(registerNewUserResult.Errors);

        var user = registerNewUserResult.Value;
        ErrorOr<Created> createUserResult = _userService.CreateUser(user);

        if (createUserResult.IsError)
            return Problem(createUserResult.Errors);

        return Ok();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LogInRequest request)
    {
        ErrorOr<User> getUserResult = _userService.GetUser(request.username, request.password);

        if (getUserResult.IsError)
            return Problem(getUserResult.Errors);

        var user = getUserResult.Value;

        List<Claim> claims = new List<Claim>() {
            new Claim(ClaimTypes.NameIdentifier, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
        };

        ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        AuthenticationProperties properties = new AuthenticationProperties()
        {
            AllowRefresh = true,
            IsPersistent = true,
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), properties);

        return Ok();
    }

    //[HttpDelete]
    //public IActionResult DeleteUser(string username)
    //{
    //    ErrorOr<Deleted> deleteUserResult = _userService.DeleteUser(username);

    //    return deleteUserResult.Match(
    //        deleted => NoContent(),
    //        errors => Problem(errors));
    //}

    //private static UserResponse MapUserResponse(User user)
    //{
    //    return new UserResponse(user.Username, user.Email);
    //}

    //private CreatedAtActionResult CreatedAtGetUser(User user)
    //{
    //    return CreatedAtAction(
    //        actionName: nameof(Login),
    //        routeValues: new { id = user.Username.GetHashCode() },
    //        value: MapUserResponse(user));
    //}
}