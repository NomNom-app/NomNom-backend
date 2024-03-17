#define LOG_USER_SERVICE

using NomNomAPI.Models;
using NomNomAPI.ServiceErrors;
using ErrorOr;
using NomNomAPI.DebugUtils;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using NomNomAPI.Auth;
using Microsoft.Extensions.Options;

namespace NomNomAPI.Services.Users;

public class UserService : IUserService
{
    private readonly JwtSettings _jwtSettings;
    private readonly Dictionary<int, User> _users;   //FTM: key is hash code of username

    public UserService(IOptions<JwtSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;
        _users = new();
    }

    public ErrorOr<Created> CreateUser(User user)
    {
       if (IsUsernameAlreadyInUse(user.Username))
            return Errors.User.UsernameAlreadyTaken;

       if (IsEmailAlreadyInUse(user.Email))
            return Errors.User.EmailAlreadyTaken;

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

    public ErrorOr<User> GetUser(string username, string password)
    {
        if (!_users.TryGetValue(username.GetHashCode(), out var user))
            return Errors.User.InvalidUsername;

        if (user.HashedPassword != password.GetHashCode())
            return Errors.User.InvalidPassword;

#if LOG_USER_SERVICE
        Log.LogMessage($"User with username {username} returned.");
#endif

        return user;
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

   
    private bool IsUsernameAlreadyInUse(string username)
    {
       return _users.ContainsKey(username.GetHashCode());
    }

    //Hacky until we get a proper database
    private bool IsEmailAlreadyInUse(string email)
    {
        foreach (KeyValuePair<int, User> usersElement in _users)
        {
            if (usersElement.Value.Email.Equals(email))
                return true;
        }

        return false;
    }

    public string GenerateJwtToken(User user)
    {
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.ID.ToString()),
            new Claim(JwtRegisteredClaimNames.GivenName, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var securityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }

}
