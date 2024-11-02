using Library.Models;
using Library.Services;
using Library.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Library.Controllers;

public class BookController : Controller
{
    private readonly LibraryContext _context;

    public BookController(LibraryContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string name, string author, bool? status, SortBookState sortBookState = SortBookState.DateAddedDesc, int page = 1)
    {
        IQueryable<Book> books = _context.Books.Include(b => b.Category).OrderByDescending(b => b.DateAdded);

        if (!string.IsNullOrWhiteSpace(name))
        {
            books = books.Where(b => b.Name.Contains(name));
        }
        if (!string.IsNullOrWhiteSpace(author))
        {
            books = books.Where(b => b.Author.Contains(author));
        }
        if (status.HasValue)
        {
            books = books.Where(b => b.Status == status.Value);
        }
        
        ViewBag.NameSort = sortBookState == SortBookState.NameAsc ? SortBookState.NameDesc : SortBookState.NameAsc;
        ViewBag.AuthorSort = sortBookState == SortBookState.AuthorAsc ? SortBookState.AuthorDesc : SortBookState.AuthorAsc;
        ViewBag.StatusSort = sortBookState == SortBookState.StatusAsc ? SortBookState.StatusDesc : SortBookState.StatusAsc;
        ViewBag.DateAddedSort = sortBookState == SortBookState.DateAddedAsc ? SortBookState.DateAddedDesc : SortBookState.DateAddedAsc;

        switch (sortBookState)
        {
            case SortBookState.NameAsc:
                books = books.OrderBy(b => b.Name);
                break;
            case SortBookState.NameDesc:
                books = books.OrderByDescending(b => b.Name);
                break;
            case SortBookState.AuthorAsc:
                books = books.OrderBy(b => b.Author);
                break;
            case SortBookState.AuthorDesc:
                books = books.OrderByDescending(b => b.Author);
                break;
            case SortBookState.StatusAsc:
                books = books.OrderBy(b => b.Status);
                break;
            case SortBookState.StatusDesc:
                books = books.OrderByDescending(b => b.Status);
                break;
        }
        
        int pageSize = 2;
        int count = books.Count();
        var items = books.Skip((page - 1) * pageSize).Take(pageSize);
        PageViewModel pvm = new PageViewModel(books.Count(), page, pageSize);

        var bivm = new BookIndexViewModel()
        {
            Books = items.ToList(),
            PageViewModel = pvm,
            FilterName = name,
            FilterAuthor = author,
            FilterStatusBusy = (status == true),
            FilterStatusFree = (status == false)
        };
        
        return View(bivm);
    }
    
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(Book book)
    {
        if (ModelState.IsValid)
        {
            book.DateAdded = DateTime.UtcNow;
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        return View(book);
    }
    
    public async Task<IActionResult> Details(int bookId)
    {
        Book book = await _context.Books.Include(b => b.Category).FirstOrDefaultAsync(b => b.Id == bookId);
        if (book != null)
        {
            return View(book);
        }

        return NotFound();
    }

    public async Task<IActionResult> Edit(int bookId)
    {
        Book book = await _context.Books.Include(b => b.Category).FirstOrDefaultAsync(b => b.Id == bookId);
        ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
        if (book != null)
        {
            return View(book);
        }

        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Book book)
    {
        if (ModelState.IsValid)
        {
            var existingBook = await _context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == book.Id);
            if (existingBook != null)
            {
                book.DateAdded = existingBook.DateAdded;

                _context.Books.Update(book);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return NotFound();
        }
        
        ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
        return View(book);
    }

    
    public async Task<IActionResult> Delete(int bookId)
    {
        Book book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId);
        if (book != null)
        {
            _context.Remove(book);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }
    
    public async Task<IActionResult> Rent(int bookId)
    {
        Book book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId);
        if (book != null)
        {
            return View(book);
        }
        
        return NotFound();
    }
    
    [HttpPost]
    public async Task<IActionResult> Rent(int bookId, string email)
    {
        User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        Book book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId);
    
        if (user != null)
        {
            if (book != null)
            {
                List<BookUsersRent> bur = await _context.BookUsersRents
                    .Where(b => b.UserId == user.Id && b.ReturnDate == null).ToListAsync();
                
                if (bur.Count >= 3)
                {
                    ModelState.AddModelError("", "You took too many books! The maximum you can take is 3 books.");
                    return View(book);
                }

                book.Status = true;
                BookUsersRent rb = new BookUsersRent()
                {
                    BookId = book.Id,
                    UserId = user.Id,
                    ReceiptDate = DateTime.UtcNow
                };

                _context.Update(book);
                await _context.AddAsync(rb);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return NotFound();
        }
    
        ModelState.AddModelError("", "The user with the specified email does not exist!");
        return View(book);
    }
    
    public async Task<IActionResult> ReturnBook(int bookId)
    {
        var rentalRecord = await _context.BookUsersRents
            .Include(bur => bur.Book)
            .FirstOrDefaultAsync(bur => bur.Id == bookId && bur.ReturnDate == null);

        if (rentalRecord == null)
        {
            return NotFound();
        }
        
        rentalRecord.Book.Status = false;
        rentalRecord.ReturnDate = DateTime.UtcNow;

        _context.Update(rentalRecord.Book);
        _context.Update(rentalRecord);
        await _context.SaveChangesAsync();
        
        return RedirectToAction("Profile", "User", new { userId = rentalRecord.UserId });
    }
}