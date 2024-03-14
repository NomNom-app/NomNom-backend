using Microsoft.VisualStudio.TestTools.UnitTesting;
using NomNomAPI.Services.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NomNomAPI.Models;
using ErrorOr;

namespace NomNomAPI.Services.Users.Tests
{
    [TestClass]
    public class UserServiceTests
    {
        [TestMethod]
        public void CreateUser_UsernameAlreadyTaken_ReturnsTrue()
        {
            UserService service = new UserService();

            ErrorOr<User> userEO = User.Create("JohnMcDough", "JohnMc@gmail.com", "HandsomeJohn123");
            Assert.IsFalse(userEO.IsError);
            User user = userEO.Value;

            ErrorOr<User> user2EO = User.Create("JohnMcDough", "JohnDough@gmail.com", "HandsomeDough123");
            Assert.IsFalse(user2EO.IsError);
            User user2 = user2EO.Value;

            ErrorOr<Created> createResult = service.CreateUser(user);
            Assert.IsFalse(createResult.IsError);

            ErrorOr<Created> createResult2 = service.CreateUser(user2);
            Assert.IsTrue(createResult2.IsError);
            List<Error> errors = createResult2.Errors;


            Assert.IsTrue(errors.Contains(ServiceErrors.Errors.User.UsernameAlreadyTaken));
        }

        [TestMethod]
        public void CreateUser_EmailAlreadyTaken_ReturnsTrue()
        {
            UserService service2 = new UserService();

            ErrorOr<User> userEO = User.Create("PeterPeterson", "CoolPete@gmail.com", "Peterson321");
            Assert.IsFalse(userEO.IsError);
            User user = userEO.Value;

            ErrorOr<User> user2EO = User.Create("CoolPete", "CoolPete@gmail.com", "Pete123456789");
            Assert.IsFalse(user2EO.IsError);
            User user2 = user2EO.Value;

            ErrorOr<Created> createResult = service2.CreateUser(user);
            Assert.IsFalse(createResult.IsError);

            ErrorOr<Created> createResult2 = service2.CreateUser(user2);
            Assert.IsTrue(createResult2.IsError);
            List<Error> errors = createResult2.Errors;


            Assert.IsTrue(errors.Contains(ServiceErrors.Errors.User.EmailAlreadyTaken));
        }
    }
}