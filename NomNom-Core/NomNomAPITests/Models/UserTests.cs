using Microsoft.VisualStudio.TestTools.UnitTesting;
using ErrorOr;
using NomNomAPI.Services.Users;
using NomNomAPI.Controllers;
using NomNom.Contracts.User;

namespace NomNomAPI.Models.Tests;

[TestClass()]
public class UserTests
{
    [TestMethod()]
    public void SignUp_ValidateUsername_UsernameTaken_ReturnsFalse()
    {
        //Arrange
        IUserService userService = new UserService();
        UsersController controller = new UsersController(userService);
        RegisterNewUserRequest userRequest = new RegisterNewUserRequest(username: "johnMcDough", email: "johnMc123@gmail.com", password: "HandsomeJohn123");

        controller.Register(userRequest);


        //Act
        User.SignUp.ValidateUsername("johnMcDough", userService, ref errors);

    }

    [TestMethod()]
    public void LogIn()
    {
        Assert.Fail();
    }
}