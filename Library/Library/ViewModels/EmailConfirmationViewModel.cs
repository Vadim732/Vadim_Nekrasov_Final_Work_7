using System.ComponentModel.DataAnnotations;

namespace Library.ViewModels;

public class EmailConfirmationViewModel
{
    public int UserId { get; set; }
        
    [Required]
    [EmailAddress(ErrorMessage = "Enter a valid email address!")]
    public string Email { get; set; }
}