using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Caterer.Data;
using Caterer.Models;
using System.Security.Claims;
namespace Caterer.Controllers
{
    public class MenuItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public MenuItemsController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpPost]
        public JsonResult SaveCategories(MenuCategory Measurementmodel)
        {
            if (_context.MenuCategories.Any(c => c.MenuCategoryName == Measurementmodel.MenuCategoryName && c.RestaurantId == Measurementmodel.RestaurantId))
            {
                return new JsonResult("MenuCategoryName for this RestaurantId already exists. Please choose a different name.");
            }
            var Data = new MenuCategory()
            {
                RestaurantId = Measurementmodel.RestaurantId,
                MenuCategoryName = Measurementmodel.MenuCategoryName,
                SelectedItem = Measurementmodel.SelectedItem,
            };
            _context.MenuCategories.Add(Data);
            _context.SaveChanges();
            return new JsonResult("Category Saved Successfully....!");
        }
        //Get Edit Categories
        public JsonResult EditCategories(int id)
        {
            var data = _context.MenuCategories.Where(ev => ev.MenuCategoryId == id).SingleOrDefault();
            return new JsonResult(data);
        }
        //Post Edit Category
        [HttpPost]
        public JsonResult EditCategory(MenuCategory editedCategory)
        {
            try
            {
                var existingCategory = _context.MenuCategories.Find(editedCategory.MenuCategoryId);
                if (existingCategory == null)
                {
                    return new JsonResult("Category not found!");
                }
                existingCategory.MenuCategoryName = editedCategory.MenuCategoryName;
                existingCategory.SelectedItem = editedCategory.SelectedItem;
                var cateringItemsToUpdate = _context.CateringItems
                    .Where(ci => ci.MenuCategoryId == existingCategory.MenuCategoryId)
                    .ToList();
                foreach (var cateringItem in cateringItemsToUpdate)
                {
                    cateringItem.MenuCategoryName = editedCategory.MenuCategoryName;
                    cateringItem.SelectedItem = editedCategory.SelectedItem;
                    _context.Update(cateringItem);
                }
                _context.SaveChanges();
                return new JsonResult("Category Edited Successfully....!");
            }
            catch (Exception ex)
            {
                return new JsonResult($"Error editing category: {ex.Message}");
            }
        }
        //Post Delete Category
        [HttpPost]
        public JsonResult DeleteCategory(int id)
        {
            try
            {
                var categoryToDelete = _context.MenuCategories.Find(id);
                if (categoryToDelete == null)
                {
                    return new JsonResult("Category not found!");
                }
                var cateringItemsToDelete = _context.CateringItems
                    .Where(ci => ci.MenuCategoryId == id)
                    .ToList();
                foreach (var cateringItem in cateringItemsToDelete)
                {
                    _context.CateringItems.Remove(cateringItem);
                }
                _context.MenuCategories.Remove(categoryToDelete);
                _context.SaveChanges();
                return new JsonResult("Category Deleted Successfully....!");
            }
            catch (Exception ex)
            {
                return new JsonResult($"Error deleting category: {ex.Message}");
            }
        }
        // GET: MenuItems
        public async Task<IActionResult> Index()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                var menuCategories = await _context.MenuCategories
                    .Where(mc => mc.RestaurantId == restaurant.RestaurantId)   
                    .Include(mc => mc.MenuItems.Where(mi => mi.RestaurantId == restaurant.RestaurantId))
                    .ToListAsync();
                ViewData["RestaurantId"] = restaurant.RestaurantId;
                return View(menuCategories);
            }
            return View(new List<MenuCategory>());
        }
        // GET: MenuItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MenuItems == null)
            {
                return NotFound();
            }
            var menuItem = await _context.MenuItems
                .FirstOrDefaultAsync(m => m.MenuItemId == id);
            if (menuItem == null)
            {
                return NotFound();
            }
            return View(menuItem);
        }
        // GET: MenuItems/Create
        public IActionResult Create(int? categoryId)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                var menuCategories = _context.MenuCategories
                    .Where(mc => mc.RestaurantId == restaurant.RestaurantId)
                    .ToList();
                ViewBag.MenuCategories = new SelectList(menuCategories, "MenuCategoryId", "MenuCategoryName");
                ViewBag.RestaurantId = restaurant.RestaurantId;
                ViewBag.SelectedCategoryId = categoryId;
                return View();
            }
            return RedirectToAction("Index", "Home");  
        }
        // POST: MenuItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create(MenuItemViewModel menuItemViewModel, IFormFile productImage)
        {
            try
            {
                string filename = "";

                if (productImage != null)
                {
                    string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Images/Menuitems");
                    filename = Guid.NewGuid().ToString() + "_" + productImage.FileName;
                    string filepath = Path.Combine(uploadFolder, filename);
                    using (var fileStream = new FileStream(filepath, FileMode.Create))
                    {
                        await productImage.CopyToAsync(fileStream);
                    }
                }
                if (ModelState.IsValid)
                {
                    bool isItemNameExists = _context.MenuItems
                        .Any(mi => mi.ItemName == menuItemViewModel.ItemName && mi.RestaurantId == menuItemViewModel.RestaurantId);

                    if (isItemNameExists)
                    {
                        ModelState.AddModelError("ItemName", "This ItemName already exists for the selected Restaurant. Please choose a different ItemName.");
                        ViewBag.MenuCategories = new SelectList(_context.MenuCategories, "MenuCategoryId", "MenuCategoryName");
                        return View(menuItemViewModel);
                    }
                    var menuItem = new MenuItem
                    {
                        ItemName = menuItemViewModel.ItemName,
                        RestaurantId = menuItemViewModel.RestaurantId,
                        MenuCategoryId = menuItemViewModel.MenuCategoryId,
                        ExtrasId = menuItemViewModel.ExtrasId,
                        ItemDescription = menuItemViewModel.ItemDescription,
                        ItemPrice = menuItemViewModel.ItemPrice,
                        DineInPrice = menuItemViewModel.DineInPrice,
                        DiscountedPrice = menuItemViewModel.DiscountedPrice,
                        VATPercentage = menuItemViewModel.VATPercentage,
                        ItemAvailable = menuItemViewModel.ItemAvailable,
                        EnableVariants = menuItemViewModel.EnableVariants,
                        EnableAlwaysAvailable = menuItemViewModel.EnableAlwaysAvailable,
                        Monday = menuItemViewModel.Monday,
                        MondayStart = menuItemViewModel.MondayStart,
                        MondayEnd = menuItemViewModel.MondayEnd,
                        Tuesday = menuItemViewModel.Tuesday,
                        TuesdayStart = menuItemViewModel.TuesdayStart,
                        TuesdayEnd = menuItemViewModel.TuesdayEnd,
                        Wednesday = menuItemViewModel.Wednesday,
                        WednesdayStart = menuItemViewModel.WednesdayStart,
                        WednesdayEnd = menuItemViewModel.WednesdayEnd,
                        Thursday = menuItemViewModel.Thursday,
                        ThursdayStart = menuItemViewModel.ThursdayStart,
                        ThursdayEnd = menuItemViewModel.ThursdayEnd,
                        Friday = menuItemViewModel.Friday,
                        FridayStart = menuItemViewModel.FridayStart,
                        FridayEnd = menuItemViewModel.FridayEnd,
                        Saturday = menuItemViewModel.Saturday,
                        SaturdayStart = menuItemViewModel.SaturdayStart,
                        SaturdayEnd = menuItemViewModel.SaturdayEnd,
                        Sunday = menuItemViewModel.Sunday,
                        SundayStart = menuItemViewModel.SundayStart,
                        SundayEnd = menuItemViewModel.SundayEnd,
                        ItemImage = filename
                    };
                    _context.Add(menuItem);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                return Problem("An error occurred while processing your request. Please try again later.");
            }
            ViewBag.MenuCategories = new SelectList(_context.MenuCategories, "MenuCategoryId", "MenuCategoryName");
            return View(menuItemViewModel);
        }
        // GET: MenuItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MenuItems == null)
            {
                return NotFound();
            }
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                var extrasList = await _context.Extras.Where(e => e.MenuItemId == id).ToListAsync();
                menuItem.ExtraList = extrasList;
                var menuCategories = _context.MenuCategories
                    .Where(mc => mc.RestaurantId == restaurant.RestaurantId)
                    .ToList();
                ViewBag.MenuCategories = new SelectList(menuCategories, "MenuCategoryId", "MenuCategoryName");
                return View(menuItem);
            }
            return RedirectToAction("Index", "Home");  
        }
        // POST: MenuItems/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Edit(int id, MenuItemViewModel menuItemViewModel, IFormFile productImage)
        {
            try
            {
                string filename = "";
                if (productImage != null)
                {
                    string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Images/Menuitems");
                    filename = Guid.NewGuid().ToString() + "_" + productImage.FileName;
                    string filepath = Path.Combine(uploadFolder, filename);
                    using (var fileStream = new FileStream(filepath, FileMode.Create))
                    {
                        await productImage.CopyToAsync(fileStream);
                    }
                }
                if (ModelState.IsValid)
                {
                    var menuItem = await _context.MenuItems.FindAsync(id);

                    if (menuItem == null)
                    {
                        return NotFound();
                    }
                    menuItem.ItemName = menuItemViewModel.ItemName;
                    menuItem.MenuCategoryId = menuItemViewModel.MenuCategoryId;
                    menuItem.ExtrasId = menuItemViewModel.ExtrasId;
                    menuItem.ItemDescription = menuItemViewModel.ItemDescription;
                    menuItem.ItemPrice = menuItemViewModel.ItemPrice;
                    menuItem.DineInPrice = menuItemViewModel.DineInPrice;
                    menuItem.DiscountedPrice = menuItemViewModel.DiscountedPrice;
                    menuItem.VATPercentage = menuItemViewModel.VATPercentage;
                    menuItem.ItemAvailable = menuItemViewModel.ItemAvailable;
                    menuItem.EnableVariants = menuItemViewModel.EnableVariants;
                    menuItem.EnableAlwaysAvailable = menuItemViewModel.EnableAlwaysAvailable;
                    menuItem.Monday = menuItemViewModel.Monday;
                    menuItem.MondayStart = menuItemViewModel.MondayStart;
                    menuItem.MondayEnd = menuItemViewModel.MondayEnd;
                    menuItem.Tuesday = menuItemViewModel.Tuesday;
                    menuItem.TuesdayStart = menuItemViewModel.TuesdayStart;
                    menuItem.TuesdayEnd = menuItemViewModel.TuesdayEnd;
                    menuItem.Wednesday = menuItemViewModel.Wednesday;
                    menuItem.WednesdayStart = menuItemViewModel.WednesdayStart;
                    menuItem.WednesdayEnd = menuItemViewModel.WednesdayEnd;
                    menuItem.Thursday = menuItemViewModel.Thursday;
                    menuItem.ThursdayStart = menuItemViewModel.ThursdayStart;
                    menuItem.ThursdayEnd = menuItemViewModel.ThursdayEnd;
                    menuItem.Friday = menuItemViewModel.Friday;
                    menuItem.FridayStart = menuItemViewModel.FridayStart;
                    menuItem.FridayEnd = menuItemViewModel.FridayEnd;
                    menuItem.Saturday = menuItemViewModel.Saturday;
                    menuItem.SaturdayStart = menuItemViewModel.SaturdayStart;
                    menuItem.SaturdayEnd = menuItemViewModel.SaturdayEnd;
                    menuItem.Sunday = menuItemViewModel.Sunday;
                    menuItem.SundayStart = menuItemViewModel.SundayStart;
                    menuItem.SundayEnd = menuItemViewModel.SundayEnd;
                    if (!string.IsNullOrEmpty(filename))
                    {
                        menuItem.ItemImage = filename;
                    }
                    var cateringItems = _context.CateringItems.Where(ci => ci.MenuItemId == id);
                    foreach (var cateringItem in cateringItems)
                    {
                        cateringItem.ItemName = menuItemViewModel.ItemName;
                    }
                    _context.Update(menuItem);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                return Problem("An error occurred while processing your request. Please try again later.");
            }
            ViewBag.MenuCategories = new SelectList(_context.MenuCategories, "MenuCategoryId", "MenuCategoryName");
            return View(menuItemViewModel);
        }
        //Delete Edit Row 
        [HttpPost]
        public async Task<IActionResult> DeleteEditRow(int id)
        {
            var Recipe = await _context.MenuItems.FindAsync(id);
            if (Recipe == null)
            {
                return NotFound();
            }
            _context.MenuItems.Remove(Recipe);
            await _context.SaveChangesAsync();
            return Ok();
        }
        // GET: MenuItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MenuItems == null)
            {
                return NotFound();
            }
            var menuItem = await _context.MenuItems
                .FirstOrDefaultAsync(m => m.MenuItemId == id);
            if (menuItem == null)
            {
                return NotFound();
            }
            return View(menuItem);
        }
        // POST: MenuItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MenuItems == null)
            {
                return Problem("Entity set 'ApplicationDbContext.MenuItems'  is null.");
            }
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem != null)
            {
                _context.MenuItems.Remove(menuItem);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        //CHECK DUPLICATES
        [HttpPost]
        public JsonResult CheckDuplicateItemName(string itemName, int restaurantId)
        {
            bool exists = _context.MenuItems.Any(mi => mi.ItemName == itemName && mi.RestaurantId == restaurantId);
            return Json(new { exists = exists });

        }
        private bool MenuItemExists(int id)
        {
            return _context.MenuItems.Any(e => e.MenuItemId == id);
        }
    }
}
