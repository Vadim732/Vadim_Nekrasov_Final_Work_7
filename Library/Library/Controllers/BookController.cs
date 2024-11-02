using Library.Models;
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

    public async Task<IActionResult> Index()
    {
        List<Book> books = await _context.Books.Include(b => b.Category).OrderByDescending(b => b.DateAdded).ToListAsync();
        return View(books);
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
            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

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
}