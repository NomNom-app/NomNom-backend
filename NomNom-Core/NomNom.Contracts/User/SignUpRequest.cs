﻿namespace NomNom.Contracts.User;

public record SignUpRequest(
    string Username,
    string Email,
    string Password);