#define DISABLE_CREDENTIALS_VALIDATION  //Disable validation checks for easier debugging

using ErrorOr;
using NomNom.Contracts.User;
using NomNomAPI.ServiceErrors;
using NomNomAPI.Services;
using NomNomAPI.Services.Users;

using System.Net.Mail;  //for email validation

namespace NomNomAPI.Models;

public class User
{
    public const int MIN_USERNAME_LENGTH = 3;
    public const int MAX_USERNAME_LENGTH = 15;

    public const int MIN_PASSWORD_LENGTH = 8;

    public string Username { get; set; }
    public string Email { get; set; }
    public int HashedPassword { get; set; }

    private User(string username, string email, int hashedPassword)
    {
        Username = username;
        Email = email;
        HashedPassword = hashedPassword;
    }

    public static ErrorOr<User> Create(string username, string email, string password)
    {
#if !DISABLE_CREDENTIALS_VALIDATION
        List<Error> errors = new List<Error>();


        ValidateUsername(username, ref errors);
        ValidateEmail(email, ref errors);
        ValidatePassword(password, ref errors);

        if (errors.Count > 0)
            return errors;
#endif

        return new User(username, email, password.GetHashCode());
    }

    public static ErrorOr<User> From(SignUpRequest request)
    {
        return Create(request.username, request.email, request.password);
    }


    private static void ValidateUsername(string username, ref List<Error> errors)
    {
        if (username.Length is < MIN_USERNAME_LENGTH or > MAX_USERNAME_LENGTH)
            errors.Add(Errors.User.IncorrectUsernameLength);

        bool usernameContainsInvalidChars = !username.All(c => char.IsAscii(c));   //Only allow ascii characters?

        if (usernameContainsInvalidChars)
            errors.Add(Errors.User.UsernameContainsInvalidChars);
    }

    private static void ValidateEmail(string email, ref List<Error> errors)
    {
        try
        {
            MailAddress mail = new MailAddress(email);
        }
        catch (Exception e)
        {
            errors.Add(Errors.User.InvalidEmail);
        }
    }

    private static void ValidatePassword(string password, ref List<Error> errors)
    {
        if (password.Length < MIN_PASSWORD_LENGTH)
            errors.Add(Errors.User.IncorrectPasswordLength);

        bool uppercasePresent = false;
        bool lowercasePresent = false;
        bool numberPresent = false;

        for (int i = 0; i < password.Length; i++)
        {
            char c = password[i];

            if (char.IsLetter(c))
            {
                if (char.IsUpper(c))
                    uppercasePresent = true;
                else
                    lowercasePresent = true;
            }
            else if (char.IsNumber(c))
            {
                numberPresent = true;
            }


            if (uppercasePresent && lowercasePresent && numberPresent)
                break;
        }

        if (uppercasePresent == false)
            errors.Add(Errors.User.PasswordDoesNotContainUppercaseChar);

        if (lowercasePresent == false)
            errors.Add(Errors.User.PasswordDoesNotContainLowercaseChar);

        if (numberPresent == false)
            errors.Add(Errors.User.PasswordDoesNotContainANumber);
    }
}
