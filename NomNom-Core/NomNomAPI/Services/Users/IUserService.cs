using NomNomAPI.Models;
using ErrorOr;

namespace NomNomAPI.Services.Users
{
    public interface IUserService
    {
        ErrorOr<Created> CreateUser(User breakfast);
        ErrorOr<User> GetUser(string username);
        ErrorOr<UpsertedUser> UpsertUser(User user);
        ErrorOr<Deleted> DeleteUser(string username);

        public bool IsUsernameAlreadyInUse(string username);
        public bool IsEmailAlreadyInUse(string email);
    }
}
