namespace NomNom.Contracts.User;

public record LoginRequest(
    string Username,
    string Password);