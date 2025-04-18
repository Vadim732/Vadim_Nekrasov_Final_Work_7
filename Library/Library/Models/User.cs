﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

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
    [Remote(action: "CheckEmail", controller: "User", ErrorMessage = "This email address is already taken!")]
    public string Email { get; set; }
    [Required]
    [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Enter your phone number correctly!")]
    public string PhoneNumber { get; set; }
    public ICollection<BookUsersRent>? BookUsersRents { get; set; }
}