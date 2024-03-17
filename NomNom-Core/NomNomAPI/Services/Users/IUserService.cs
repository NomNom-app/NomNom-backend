using NomNomAPI.Models;
using ErrorOr;

namespace NomNomAPI.Services.Users
{
    public interface IUserService
    {
        ErrorOr<Created> CreateUser(User user);
        ErrorOr<User> GetUser(string username, string password);
        ErrorOr<UpsertedUser> UpsertUser(User user);
        ErrorOr<Deleted> DeleteUser(string username);
        string GenerateJwtToken(User user);
    }
}
