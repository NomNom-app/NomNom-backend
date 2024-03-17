namespace NomNom.Contracts.User;

public record AuthResponse(
    string Username,
    string Email,
    string Token);
