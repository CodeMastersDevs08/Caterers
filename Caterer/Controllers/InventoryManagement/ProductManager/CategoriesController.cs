using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Caterer.Data;
using Caterer.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
namespace Caterer.Controllers.InventoryManagement.ProductManager
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        IWebHostEnvironment hostingenvironment;
        public CategoriesController(ApplicationDbContext context, IWebHostEnvironment hc)
        {
            _context = context;
            hostingenvironment = hc;
        }
        // GET: Categories
        public async Task<IActionResult> CategoryList()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                var categories = await _context.Categories
                    .Where(c => c.RestaurantId == restaurant.RestaurantId)  
                           .OrderByDescending(s => s.CategoryId)
                    .ToListAsync();
                return View("~/Views/InventoryManagement/ProductManager/Categories/CategoryList.cshtml", categories);
            }
            return Problem("Entity set 'ApplicationDbContext.Restaurants' is null or user is not associated with any restaurant.");
        }
        // GET: Categories/Details/
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }
            var categories = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (categories == null)
            {
                return NotFound();
            }
            return View("~/Views/InventoryManagement/ProductManager/Categories/Details.cshtml", categories);
        }
        //Get Categories
        [HttpGet]
        public IActionResult GetCategories(string term)
        {
            var searchTerm = $"%{term}%";  
            var filteredCategories = _context.Categories
                .FromSqlRaw("SELECT * FROM Categories WHERE CategoryName LIKE {0}", searchTerm)
                .Select(c => new { label = c.CategoryName, value = c.CategoryId })
                .ToList();
            return Json(filteredCategories);
        }
        // GET: Categories/Create
        public IActionResult CategoryCreate()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants
                .FirstOrDefault(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                ViewData["RestaurantId"] = restaurant.RestaurantId;
                if (TempData.ContainsKey("DuplicateCategoryError"))
                {
                    ViewData["DuplicateCategoryError"] = TempData["DuplicateCategoryError"];
                }
                return View("~/Views/InventoryManagement/ProductManager/Categories/CategoryCreate.cshtml");
            }
            return RedirectToAction("Index", "Home"); 
        }
        //Post Category Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CategoryCreate(Categoryviewmodel categoryModel, IFormFile ProductImage)
        {
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var restaurant = await _context.Restaurants
                    .FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);
                if (restaurant == null)
                {
                    return RedirectToAction("Index", "Home");  
                }
                if (_context.Categories.Any(c => c.CategoryName == categoryModel.CategoryName))
                {
                    TempData["DuplicateCategoryError"] = "Category with the same name already exists.";
                    return RedirectToAction(nameof(CategoryCreate));
                }
                string filename = "";
                if (ProductImage != null)
                {
                    string uploadFolder = Path.Combine(hostingenvironment.WebRootPath, "Images/CategoryImages");
                    filename = Guid.NewGuid().ToString() + "_" + ProductImage.FileName;
                    string filepath = Path.Combine(uploadFolder, filename);
                    using (var fileStream = new FileStream(filepath, FileMode.Create))
                    {
                        await ProductImage.CopyToAsync(fileStream);
                    }
                }
                var category = new Category
                {
                    CategoryName = categoryModel.CategoryName,
                    CategoryLogo = filename, 
                    RestaurantId = restaurant.RestaurantId  
                };
                if (ModelState.IsValid)
                {
                    _context.Add(category);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(CategoryList));
                }
                return View("~/Views/InventoryManagement/ProductManager/Categories/CategoryCreate.cshtml", categoryModel);
            }
            catch (Exception ex)
            {
                return Problem("An error occurred while processing your request. Please try again later.");
            }
        }
        // GET: Categories/Edit/5
        public IActionResult CategoryEdit(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View("~/Views/InventoryManagement/ProductManager/Categories/CategoryEdit.cshtml", category);
        }
        //Post Category Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CategoryEdit(int id, [Bind("CategoryId,CategoryName,CategoryLogo")] Category category, IFormFile ProductImage)
        {
            try
            {
                var existingCategory = _context.Categories.Find(id);
                if (ProductImage != null)
                {
                    if (!string.IsNullOrEmpty(existingCategory.CategoryLogo))
                    {
                        string existingFilePath = Path.Combine(hostingenvironment.WebRootPath, "Images/CategoryImages", existingCategory.CategoryLogo);
                        if (System.IO.File.Exists(existingFilePath))
                        {
                            System.IO.File.Delete(existingFilePath);
                        }
                    }
                    string filename = Guid.NewGuid().ToString() + "_" + ProductImage.FileName;
                    string uploadFolder = Path.Combine(hostingenvironment.WebRootPath, "Images/CategoryImages");
                    string filepath = Path.Combine(uploadFolder, filename);
                    using (var fileStream = new FileStream(filepath, FileMode.Create))
                    {
                        await ProductImage.CopyToAsync(fileStream);
                    }
                    existingCategory.CategoryLogo = filename;
                }
                existingCategory.CategoryName = category.CategoryName;
                _context.Update(existingCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CategoryList));
            }
            catch (Exception ex)
            {
                return Problem("An error occurred while processing your request. Please try again later.");
            }
        }
        // GET: Categories/Delete/5
        public async Task<IActionResult> CategoryDelete(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }
            var categories = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (categories == null)
            {
                return NotFound();
            }
            return View("~/Views/InventoryManagement/ProductManager/Categories/CategoryDelete.cshtml", categories);
        }
        // POST: Categories/Delete/5
        [HttpPost, ActionName("CategoryDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
            }
            var categories = await _context.Categories.FindAsync(id);
            if (categories != null)
            {
                _context.Categories.Remove(categories);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(CategoryList));
        }
        private bool CategoriesExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}
