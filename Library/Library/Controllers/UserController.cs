using Library.Models;
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
    
    public async Task<bool> CheckEmail(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        return user == null;
    }
}