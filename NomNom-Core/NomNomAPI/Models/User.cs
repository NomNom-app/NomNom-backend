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
    public const int MAX_USERNAME_LENGTH = 30;

    public const int MIN_PASSWORD_LENGTH = 7;

    public string Username { get; set; }
    public string Email { get; set; }
    public int HashedPassword { get; set; }

    private User(string username, string email, int hashedPassword)
    {
        Username = username;
        Email = email;
        HashedPassword = hashedPassword;
    }

    public static ErrorOr<User> Create(string username, string email, string password, string passwordConfirm)
    {
        return new User(username, email, password.GetHashCode());
    }

    public static ErrorOr<User> From(CreateUserRequest request)
    {
        return Create(request.username, request.email, request.password, request.passwordRepeated);
    }

    public static ErrorOr<User> From(UpsertUserRequest request)
    {
        return Create(request.username, request.email, request.password, request.passwordRepeated);
    }


    public static class SignUp
    {
        public static bool ValidateUsername(string username, IUserService service, ref List<Error> errors)
        {
            int numErrors = errors.Count;

            if (service.IsUsernameAlreadyInUse(username))
            {
                errors.Add(Errors.User.UsernameAlreadyTaken);
                return false;
            }
                

            if (username.Length is < MIN_USERNAME_LENGTH or > MAX_USERNAME_LENGTH)
                errors.Add(Errors.User.IncorrectUsernameLength);

            bool usernameContainsInvalidChars = !username.All(c => char.IsAscii(c));   //Only allow ascii characters?

            if (usernameContainsInvalidChars)
                errors.Add(Errors.User.UsernameContainsInvalidChars);

            if (errors.Count > numErrors)
                return false;

            return true;
        }

        public static bool ValidateEmail(string email, IUserService service, ref List<Error> errors)
        {
            int numErrors = errors.Count;

            try
            {
                MailAddress mail = new MailAddress(email);
            }
            catch (Exception e)
            {
                errors.Add(Errors.User.InvalidEmail);
                return false;
            }

            if (service.IsEmailAlreadyInUse(email))
                errors.Add(Errors.User.EmailAlreadyTaken);

            if (errors.Count > numErrors)
                return false;

            return true;
        }

        public static bool ValidatePassword(string password, string passwordConfirm, ref List<Error> errors)
        {
            int numErrors = errors.Count;

            if (password.Length < MIN_PASSWORD_LENGTH)
                errors.Add(Errors.User.IncorrectPasswordLength);

            bool lengthsMatch = (password.Length == passwordConfirm.Length);
            bool passwordsMatch = lengthsMatch;
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


                if (lengthsMatch)
                {
                    if (password[i] != passwordConfirm[i])
                        passwordsMatch = false;
                }
            }

            if (passwordsMatch == false)
                errors.Add(Errors.User.PasswordsDontMatch);

            if (uppercasePresent == false)
                errors.Add(Errors.User.PasswordDoesNotContainUppercaseChar);

            if (lowercasePresent == false)
                errors.Add(Errors.User.PasswordDoesNotContainLowercaseChar);

            if (numberPresent == false)
                errors.Add(Errors.User.PasswordDoesNotContainANumber);

            if (errors.Count > numErrors)
                return false;

            return true;
        }
    }

    public static class LogIn
    {
        public static ErrorOr<User> ValidateCredentials(string username, string password, IUserService service)
        {
            ErrorOr<User> getUserResult = service.GetUser(username);

            if (getUserResult.IsError)
                return getUserResult.Errors;
               
            int hashedPassword = password.GetHashCode();

            if (hashedPassword != getUserResult.Value.HashedPassword)
                return Errors.User.InvalidPassword;

            return getUserResult.Value;
        }
    }
}
