using Library.Models;
using Library.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Controllers;

public class UserController : Controller
{
    private readonly LibraryContext _context;

    public UserController(LibraryContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        List<User> users = await _context.Users.ToListAsync();
        
        return View(users);
    }
    
    public async Task<IActionResult> Create()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(User user)
    {
        if (ModelState.IsValid)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        return View(user);
    }
    
    public async Task<IActionResult> Edit(int userId)
    {
        User user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user != null)
        {
            return View(user);
        }

        return NotFound();
    }
    
    [HttpPost]
    public async Task<IActionResult> Edit(User user)
    {
        if (ModelState.IsValid)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        return View(user);
    }
    
    public async Task<IActionResult> Delete(int userId)
    {
        User user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user != null)
        {
            _context.Remove(user);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }
    
    public IActionResult ConfirmEmail(int userId)
    {
        return View(new EmailConfirmationViewModel { UserId = userId });
    }
    
    [HttpPost]
    public async Task<IActionResult> ConfirmEmail(EmailConfirmationViewModel model)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.UserId);
    
        if (user != null && user.Email.ToLower() == model.Email.ToLower())
        {
            return RedirectToAction("Profile", new { userId = model.UserId });
        }
    
        ModelState.AddModelError("", "The email you entered is incorrect!");
        return View(model);
    }
    
    public async Task<IActionResult> Profile(int userId)
    {
        User user = await _context.Users.Include(u => u.BookUsersRents.Where(bur => bur.ReturnDate == null)).ThenInclude(bur => bur.Book).FirstOrDefaultAsync(u => u.Id == userId);
        if (user != null)
        {
            return View(user);
        }

        return NotFound();
    }

    public async Task<bool> CheckEmail(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        return user == null;
    }
}