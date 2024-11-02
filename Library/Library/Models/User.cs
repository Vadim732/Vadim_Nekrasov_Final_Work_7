using System.ComponentModel.DataAnnotations;

namespace Library.Models;

public class User
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Surname { get; set; }
    [Required]
    [EmailAddress(ErrorMessage = "Enter your valid email address!")]
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
}