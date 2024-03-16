using System.ComponentModel.DataAnnotations;

namespace NomNom.Contracts.User;

public class SignUpRequest
{
    [Required]
    public string username { get; set; }

    [Required]
    [DataType(DataType.EmailAddress)]
    public string email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string password { get; set; }
}