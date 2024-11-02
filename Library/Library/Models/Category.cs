using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Library.Models;

public class Category
{
    public int Id { get; set; }
    [Required]
    [Remote(action: "CheckName", controller: "Category", ErrorMessage = "A category with this name already exists!")]
    public string Name { get; set; }
}