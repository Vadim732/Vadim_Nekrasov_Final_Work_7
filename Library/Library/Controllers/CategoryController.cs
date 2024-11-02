using Library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Controllers;

public class CategoryController : Controller
{
    private readonly LibraryContext _context;

    public CategoryController(LibraryContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> Index()
    {
        List<Category> categories = await _context.Categories.ToListAsync();
        return View(categories);
    }
        
    public IActionResult Create()
    {
        return View();
    }
        
    [HttpPost]
    public async Task<IActionResult> Create(Category category)
    {
        if (ModelState.IsValid)
        {
            _context.Add(category);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
            
        return View(category);
    }
    
    public async Task<IActionResult> Edit(int categoryId)
    {
        Category category = await _context.Categories.FirstOrDefaultAsync(u => u.Id == categoryId);
        if (category != null)
        {
            return View(category);
        }

        return NotFound();
    }
    
    [HttpPost]
    public async Task<IActionResult> Edit(Category category)
    {
        if (ModelState.IsValid)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        return View(category);
    }

    public async Task<IActionResult> Delete(int categoryId)
    {
        Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
        if (category != null)
        {
            _context.Remove(category);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }
        
    public async Task<bool> CheckName(string name)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
        return category == null;
    }
}