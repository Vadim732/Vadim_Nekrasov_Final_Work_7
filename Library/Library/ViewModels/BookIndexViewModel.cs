using Library.Models;

namespace Library.ViewModels;

public class BookIndexViewModel
{
    public List<Book> Books { get; set; }
    public PageViewModel PageViewModel { get; set; }
    public string FilterName { get; set; }
    public string FilterAuthor { get; set; }
    public bool FilterStatusBusy { get; set; }
    public bool FilterStatusFree { get; set; }
}