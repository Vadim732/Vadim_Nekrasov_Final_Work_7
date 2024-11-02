namespace Library.Models;

public class BookUsersRent
{
    public int Id { get; set; }
    public DateTime ReceiptDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    
    public int BookId { get; set; }
    public Book Book { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; }
}