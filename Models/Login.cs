#pragma warning disable CS8618

using System.ComponentModel.DataAnnotations;

namespace TheWall.Models;

public class LoginUser
{
    [Key]
    public int UserId { get; set; }
    [Required(ErrorMessage = "Este campo es requerido")]
    [EmailAddress]
    public string EmailLogin { get; set; }
    [Required(ErrorMessage = "Este campo es requerido")]
    [DataType(DataType.Password)]
    public string PasswordLogin { get; set; }
}