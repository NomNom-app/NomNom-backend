namespace NomNom.Contracts.User;
public record UpsertUserRequest(string username, string email, string password, string passwordRepeated);
