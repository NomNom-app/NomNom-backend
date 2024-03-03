using NomNom.Contracts.User;
using NomNomAPI.Models;
using NomNomAPI.ServiceErrors;
using NomNomAPI.Services.Users;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace NomNomAPI.Controllers;

public class UsersController : ApiController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public IActionResult CreateUser(CreateUserRequest request)
    {
        List<Error> credentialErrors = new();
        {
            Models.User.SignUp.ValidateUsername(request.username, _userService, ref credentialErrors);
            Models.User.SignUp.ValidateEmail(request.email, _userService, ref credentialErrors);
            Models.User.SignUp.ValidatePassword(request.password, request.passwordRepeated, ref credentialErrors);

            if (credentialErrors.Count > 0)
                return Problem(credentialErrors);
        }


        ErrorOr<User> requestToUserResult = Models.User.From(request);

        if (requestToUserResult.IsError)
            return Problem(requestToUserResult.Errors);

        var user = requestToUserResult.Value;
        ErrorOr<Created> createUserResult = _userService.CreateUser(user);

        return createUserResult.Match(
            created => CreatedAtGetUser(user),
            errors => Problem(errors));
    }

    [HttpGet]
    public IActionResult GetUser([FromQuery] GetUserRequest request)
    {
       ErrorOr<User> getUserResult = Models.User.LogIn.ValidateCredentials(request.username, request.password, _userService);

        if (getUserResult.IsError)
            return Problem(getUserResult.Errors);

        return getUserResult.Match(
            user => Ok(MapUserResponse(user)),
            errors => Problem(errors));
    }

    [HttpPut]
    public IActionResult UpsertUser(UpsertUserRequest request)
    {
        ErrorOr<User> requestToUserResult = Models.User.From(request);

        if (requestToUserResult.IsError)
        {
            return Problem(requestToUserResult.Errors);
        }

        var user = requestToUserResult.Value;
        ErrorOr<UpsertedUser> upsertUserResult = _userService.UpsertUser(user);

        return upsertUserResult.Match(
            upserted => upserted.IsNewlyCreated ? CreatedAtGetUser(user) : NoContent(),
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
            actionName: nameof(GetUser),
            routeValues: new { id = user.Username.GetHashCode() },
            value: MapUserResponse(user));
    }
}