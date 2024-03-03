#define LOG_USER_SERVICE

using NomNomAPI.Models;
using NomNomAPI.ServiceErrors;
using NomNomAPI.Utils;
using ErrorOr;

namespace NomNomAPI.Services.Users;

public class UserService : IUserService
{
    private static readonly Dictionary<int, User> _users = new();   //FTM: key is hash code of username

    public ErrorOr<Created> CreateUser(User user)
    {
        _users.Add(user.Username.GetHashCode(), user);

#if LOG_USER_SERVICE
        Log.LogMessage($"User with username {user.Username} created.");
#endif

        return Result.Created;
    }

    public ErrorOr<Deleted> DeleteUser(string username)
    {
        _users.Remove(username.GetHashCode());

#if LOG_USER_SERVICE
        Log.LogMessage($"User with username {username} deleted.");
#endif

        return Result.Deleted;
    }

    public ErrorOr<User> GetUser(string username)
    {
        if (_users.TryGetValue(username.GetHashCode(), out var user))
        {
#if LOG_USER_SERVICE
            Log.LogMessage($"User with username {username} returned.");
#endif
            return user;
        }

        return Errors.User.InvalidUsername;
    }

    public ErrorOr<UpsertedUser> UpsertUser(User user)
    {
        var isNewlyCreated = !_users.ContainsKey(user.Username.GetHashCode());
        _users[user.Username.GetHashCode()] = user;

#if LOG_USER_SERVICE
        Log.LogMessage($"User with username {user.Username} returned.");
#endif

        return new UpsertedUser(isNewlyCreated);
    }

   
    public bool IsUsernameAlreadyInUse(string username)
    {
       return _users.ContainsKey(username.GetHashCode());
    }

    //Hacky until we get a proper database
    public bool IsEmailAlreadyInUse(string email)
    {
        foreach (KeyValuePair<int, User> usersElement in _users)
        {
            if (usersElement.Value.Email.Equals(email))
                return true;
        }

        return false;
    }

}
