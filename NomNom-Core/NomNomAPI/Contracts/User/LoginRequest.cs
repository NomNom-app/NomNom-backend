using System.ComponentModel.DataAnnotations;

namespace NomNom.Contracts.User;

public class LoginRequest
{
    [Required]
    public string username { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string password { get; set; }
}
