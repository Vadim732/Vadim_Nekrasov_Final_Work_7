using System.ComponentModel.DataAnnotations;

namespace Library.Models;

public class Book
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Author { get; set; }
    public bool Status { get; set; }
    [Required]
    [Url]
    public string Image { get; set; }
    public DateOnly? YearOfIssue { get; set; }
    public string? Description { get; set; }
    public DateTime? DateAdded { get; set; }
    
    [Required]
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
}