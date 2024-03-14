using ErrorOr;
namespace NomNomAPI.ServiceErrors;

public static class Errors
{
    public static class User
    {
        public static Error IncorrectUsernameLength => Error.Validation(code: "User.IncorrectUsernameLength",
            description: $"Username must be at least {Models.User.MIN_USERNAME_LENGTH} characters long"
            + $"and at most {Models.User.MAX_USERNAME_LENGTH} characters long");

        public static Error UsernameAlreadyTaken => Error.Validation(code: "User.UsernameAlreadyTaken",
            description: $"Username is already in use by another account");

        public static Error UsernameContainsInvalidChars => Error.Validation(code: "User.UsernameContainsInvalidChars",
            description: $"Username contains invalid characters");

        public static Error InvalidEmail => Error.Validation(code: "User.InvalidEmail", description: $"Email is invalid");

        public static Error EmailAlreadyTaken => Error.Validation(code: "User.EmailAlreadyTaken", description: $"Email is already in use by another account");

        public static Error IncorrectPasswordLength => Error.Validation(code: "User.IncorrectPasswordLength",
           description: $"Password must be at least {Models.User.MIN_PASSWORD_LENGTH} characters long");

   //     public static Error PasswordsDontMatch => Error.Validation(code: "User.PasswordsDontMatch",
   //description: $"Passwords do not match");

        public static Error PasswordDoesNotContainUppercaseChar => Error.Validation(code: "User.PasswordDoesNotContainUppercaseChar",
description: $"Password does not contain an uppercase character");

        public static Error PasswordDoesNotContainLowercaseChar => Error.Validation(code: "User.PasswordDoesNotContainLowercaseChar",
description: $"Password does not contain a lowercase character");

        public static Error PasswordDoesNotContainANumber => Error.Validation(code: "User.PasswordDoesNotContainANumber",
description: $"Password does not contain a digit");

        public static Error InvalidPassword => Error.Validation(code: "User.InvalidPassword",
description: $"Incorrect password");

        public static Error InvalidUsername => Error.NotFound(
           code: "User.InvalidUsername",
           description: "No account with such username exists");
    }
}
