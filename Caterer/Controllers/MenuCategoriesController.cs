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
    public class MenuCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public MenuCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: MenuCategories
        public async Task<IActionResult> Index()
        {
            return View(await _context.MenuCategories.ToListAsync());
        }
        // GET: MenuCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MenuCategories == null)
            {
                return NotFound();
            }
            var menuCategory = await _context.MenuCategories
                .FirstOrDefaultAsync(m => m.MenuCategoryId == id);
            if (menuCategory == null)
            {
                return NotFound();
            }
            return View(menuCategory);
        }
        // GET: MenuCategories/Create
        public IActionResult Create()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants
                .FirstOrDefault(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                ViewData["RestaurantId"] = restaurant.RestaurantId;
                return View();
            }
            return RedirectToAction("Index", "Home");  
        }
        // POST: MenuCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MenuCategoryId,MenuCategoryName,ItemAvailable,SelectedItem")] MenuCategory menuCategory)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);
            if (restaurant == null)
            {
                return RedirectToAction("Index", "Home");  
            }
            menuCategory.RestaurantId = restaurant.RestaurantId;
            if (_context.MenuCategories.Any(mc => mc.RestaurantId == restaurant.RestaurantId && mc.MenuCategoryName == menuCategory.MenuCategoryName))
            {
                ModelState.AddModelError("MenuCategoryName", "This Menu Category already exists for the restaurant.");
                return View(menuCategory);
            }
            if (ModelState.IsValid)
            {
                _context.Add(menuCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), "MenuItems");
            }
            return View(menuCategory);
        }
        // GET: MenuCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MenuCategories == null)
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
            var menuCategory = await _context.MenuCategories
                .Where(mc => mc.RestaurantId == restaurant.RestaurantId && mc.MenuCategoryId == id)
                .FirstOrDefaultAsync();

            if (menuCategory == null)
            {
                return NotFound();
            }
            return View(menuCategory);
        }
        // POST: MenuCategories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MenuCategoryId,MenuCategoryName,ItemAvailable,SelectedItem")] MenuCategory menuCategory)
        {
            if (id != menuCategory.MenuCategoryId)
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
            var existingMenuCategory = await _context.MenuCategories
                .Where(mc => mc.RestaurantId == restaurant.RestaurantId && mc.MenuCategoryId == id)
                .FirstOrDefaultAsync();

            if (existingMenuCategory == null)
            {
                return NotFound();
            }
            _context.Entry(existingMenuCategory).CurrentValues.SetValues(menuCategory);
            existingMenuCategory.RestaurantId = restaurant.RestaurantId; 
            var cateringItemsToUpdate = _context.CateringItems
                .Where(ci => ci.MenuCategoryId == existingMenuCategory.MenuCategoryId)
                .ToList();
            foreach (var cateringItem in cateringItemsToUpdate)
            {
                cateringItem.MenuCategoryName = existingMenuCategory.MenuCategoryName;
                _context.Update(cateringItem);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenuCategoryExists(menuCategory.MenuCategoryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), "MenuItems");
            }
            return View(menuCategory);
        }
        // GET: MenuCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MenuCategories == null)
            {
                return NotFound();
            }
            var menuCategory = await _context.MenuCategories
                .FirstOrDefaultAsync(m => m.MenuCategoryId == id);
            if (menuCategory == null)
            {
                return NotFound();
            }
            return View(menuCategory);
        }
        // POST: MenuCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var menuCategory = await _context.MenuCategories.FindAsync(id);

                if (menuCategory != null)
                {
                    var menuItemsToDelete = await _context.MenuItems
                        .Where(item => item.MenuCategoryId == id)
                        .ToListAsync();
                    _context.MenuItems.RemoveRange(menuItemsToDelete);
                    _context.MenuCategories.Remove(menuCategory);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index), "MenuItems");
            }
            catch (Exception ex)
            {
                return Problem("An error occurred while processing your request. Please try again later.");
            }
        }
        private bool MenuCategoryExists(int id)
        {
            return _context.MenuCategories.Any(e => e.MenuCategoryId == id);
        }
    }
}
