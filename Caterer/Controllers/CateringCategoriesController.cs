using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Caterer.Data;
using Caterer.Models;
using System.Security.Claims;
namespace Caterer.Controllers
{
    public class CateringCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CateringCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: CateringCategories
        public async Task<IActionResult> Index()
        {
            return View(await _context.CateringCategories.ToListAsync());
        }
        // GET: CateringCategories/Create
        public IActionResult Create()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants
                .FirstOrDefault(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                // If associated, pass the restaurant information to the view
                ViewData["RestaurantId"] = restaurant.RestaurantId;
                return View();
            }   
            return RedirectToAction("Index", "Home");  
        }
        //Post Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CateringCategoryId,CateringCategoryName,ItemAvailable,CategoryPrice")] CateringCategory cateringCategory)
        {      
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;        
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);         
            if (restaurant == null)
            {           
                return RedirectToAction("Index", "Home"); 
            }          
            cateringCategory.RestaurantId = restaurant.RestaurantId;
            if (ModelState.IsValid)
            {
                _context.Add(cateringCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), "CateringItems");
            }
            return View(cateringCategory);
        }
        // GET: CateringCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CateringCategories == null)
            {
                return NotFound();
            }          
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;          
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);
            if (restaurant == null)
            {             
                return RedirectToAction("Index", "Home");
            }          
            var cateringCategory = await _context.CateringCategories
                .Where(c => c.RestaurantId == restaurant.RestaurantId && c.CateringCategoryId == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            if (cateringCategory == null)
            {
                return NotFound();
            }          
            ViewData["RestaurantId"] = restaurant.RestaurantId;
            return View(cateringCategory);
        }   
        //Post Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CateringCategoryId,CateringCategoryName,ItemAvailable,RestaurantId,CategoryPrice")] CateringCategory cateringCategory)
        {
            if (id != cateringCategory.CateringCategoryId)
            {
                return NotFound();
            }          
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;         
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);
            if (restaurant == null)
            {         
                return RedirectToAction("Index", "Home"); 
            }          
            var existingCateringCategory = await _context.CateringCategories
                .Where(c => c.RestaurantId == restaurant.RestaurantId && c.CateringCategoryId == id)
                .FirstOrDefaultAsync();
            if (existingCateringCategory == null)
            {
                return NotFound();
            }          
            _context.Entry(existingCateringCategory).CurrentValues.SetValues(cateringCategory);
            existingCateringCategory.RestaurantId = restaurant.RestaurantId;  
            if (ModelState.IsValid)
            {
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CateringCategoryExists(cateringCategory.CateringCategoryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), "CateringItems");
            }
            return View(cateringCategory);
        }
        // GET: CateringCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CateringCategories == null)
            {
                return NotFound();
            }
            var cateringCategory = await _context.CateringCategories
                .FirstOrDefaultAsync(m => m.CateringCategoryId == id);
            if (cateringCategory == null)
            {
                return NotFound();
            }
            return View(cateringCategory);
        }
        //Post Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CateringCategories == null)
            {
                return Problem("Entity set 'ApplicationDbContext.CateringCategories' is null.");
            }
            var cateringCategory = await _context.CateringCategories.FindAsync(id);
            if (cateringCategory != null)
            {             
                var cateringItemsToDelete = await _context.CateringItems
                    .Where(item => item.CateringCategoryId == id)
                    .ToListAsync();
                _context.CateringItems.RemoveRange(cateringItemsToDelete);              
                _context.CateringCategories.Remove(cateringCategory);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), "CateringItems");
        }
        private bool CateringCategoryExists(int id)
        {
            return _context.CateringCategories.Any(e => e.CateringCategoryId == id);
        }
    }
}
