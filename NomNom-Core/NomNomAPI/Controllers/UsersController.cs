using NomNom.Contracts.User;
using NomNomAPI.Models;
using NomNomAPI.Services.Users;

using ErrorOr;

using Microsoft.AspNetCore.Mvc;
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
        ErrorOr<User> SignUpRequestResult = Models.User.From(request);

        if (SignUpRequestResult.IsError)
            return Problem(SignUpRequestResult.Errors);

        var user = SignUpRequestResult.Value;
        ErrorOr<Created> createUserResult = _userService.CreateUser(user);

        if (createUserResult.IsError)
            return Problem(createUserResult.Errors);

        return Ok();
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult Login(LoginRequest request)
    {
        ErrorOr<User> getUserResult = _userService.GetUser(request.Username, request.Password);

        if (getUserResult.IsError)
            return Problem(getUserResult.Errors);

        var user = getUserResult.Value;

        _userService.GenerateJwtToken(user);


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