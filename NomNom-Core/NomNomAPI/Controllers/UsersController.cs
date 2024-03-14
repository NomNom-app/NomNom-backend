using NomNom.Contracts.User;
using NomNomAPI.Models;
using NomNomAPI.ServiceErrors;
using NomNomAPI.Services.Users;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace NomNomAPI.Controllers;

public class UsersController : ApiController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public IActionResult Register(RegisterNewUserRequest request)
    {

        ErrorOr<User> registerNewUserResult = Models.User.From(request);

        if (registerNewUserResult.IsError)
            return Problem(registerNewUserResult.Errors);

        var user = registerNewUserResult.Value;
        ErrorOr<Created> createUserResult = _userService.CreateUser(user);

        if (createUserResult.IsError)
            return Problem(createUserResult.Errors);

        return createUserResult.Match(
            created => CreatedAtGetUser(user),
            errors => Problem(errors));
    }

    [HttpGet]
    public IActionResult LogIn([FromQuery] LogInUserRequest request)
    {
        ErrorOr<User> getUserResult = _userService.GetUser(request.username, request.password);

        if (getUserResult.IsError)
            return Problem(getUserResult.Errors);

        return getUserResult.Match(
            user => Ok(MapUserResponse(user)),
            errors => Problem(errors));
    }

    [HttpDelete]
    public IActionResult DeleteUser(string username)
    {
        ErrorOr<Deleted> deleteUserResult = _userService.DeleteUser(username);

        return deleteUserResult.Match(
            deleted => NoContent(),
            errors => Problem(errors));
    }

    private static UserResponse MapUserResponse(User user)
    {
        return new UserResponse(user.Username, user.Email);
    }

    private CreatedAtActionResult CreatedAtGetUser(User user)
    {
        return CreatedAtAction(
            actionName: nameof(LogIn),
            routeValues: new { id = user.Username.GetHashCode() },
            value: MapUserResponse(user));
    }
}