namespace NomNom.Contracts.User;

public record RegisterNewUserRequest(string username, string email, string password);