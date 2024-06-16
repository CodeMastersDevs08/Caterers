using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Caterer.Data;
using Caterer.Models;
using System.Runtime.InteropServices;
using System.Linq;
using System.Diagnostics;
using System.Security.Claims;
namespace Caterer.Controllers
{
    public class MenuRecipesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public MenuRecipesController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }
        // GET: MenuRecipes
        public async Task<IActionResult> Index()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);
            if (restaurant == null)
            {
                return RedirectToAction("Index", "Home");  
            }
            var menuRecipes = await _context.MenuRecipes
                .Where(m => m.RestaurantId == restaurant.RestaurantId)
                .Include(c => c.Product)
                .OrderByDescending(d => d.MenuRecipeId)
                .ToListAsync();
            return View(menuRecipes);
        }
        //Get Menu Item Details 
        [HttpGet]
        public IActionResult GetMenuItemDetails(int id)
        {
            var menuItem = _context.MenuItems
                .Where(item => item.MenuItemId == id)
                .Select(item => new
                {
                    ItemName = item.ItemName,
                    MenuCategoryId = item.MenuCategory.MenuCategoryName,
                    ItemImage = item.ItemImage
                })
                .FirstOrDefault();
            if (menuItem == null)
            {
                return Json(null);
            }
            return Json(menuItem);
        }
        //Check Duplicate Menu Item
        [HttpPost]
        public async Task<IActionResult> CheckDuplicateMenuItem(string menuItemId)
        {
            bool isDuplicate = await _context.MenuRecipes
                .AnyAsync(m => m.MenuItemId == menuItemId);
            return Json(new { isDuplicate });
        }
        //Get Product Details1
        public IActionResult GetProductDetails1(int id)
        {
            var menuItem = _context.Products
               .Where(item => item.ProductId == id && item.CreatedBy == true)
                .Select(item => new
                {
                    Productcode = item.productcode,
                    ProductType = item.ProductType,
                    ProductName = item.ProductName,
                    ProductUnit = item.Measurement.MeasurementName,
                    ProductCategory = item.Category.CategoryName,
                    MenuRecipeQty = item.Quantity
                })
                .FirstOrDefault();

            if (menuItem == null)
            {
                return Json(null);
            }
            return Json(menuItem);
        }

        // GET: MenuRecipes/Create
        public IActionResult Create()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.OwnerEmail == userEmail);
            if (restaurant == null)
            {
                return RedirectToAction("Index", "Home"); 
            }
            var menuItems = _context.MenuItems.Where(mi => mi.RestaurantId == restaurant.RestaurantId).ToList();
            var products = _context.Products.Where(p => p.RestaurantId == restaurant.RestaurantId && p.CreatedBy == true).ToList();
            ViewBag.MenuItems = new SelectList(menuItems, "MenuItemId", "ItemName");
            ViewBag.Product = new SelectList(products, "ProductId", "ProductName");
            ViewData["ResturantId"] = restaurant.RestaurantId;
            return View();
        }
        //Post Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, [Bind("MenuRecipeId,ResturantId,MenuRecipeNo,MenuItemId,MenuItemName,MenuCategory,MenuRecipeImage,ProductId,ProductType,ProductName,ProductUnit,ProductCategory,CateringRecipeQty")] List<string> productType, List<string> productName, List<string> productcode, List<string> productUnit, List<string> productCategory, List<decimal> MenuRecipeQty, List<string> productId, MenuRecipe model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                    var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);
                    if (restaurant == null)
                    {
                        return RedirectToAction("Index", "Home");  
                    }
                    model.RestaurantId = restaurant.RestaurantId;
                    _context.Add(model);
                    for (int i = 0; i < productType.Count; i++)
                    {
                        if (i == 0)
                            continue;
                        var menuRecipe = new MenuRecipe
                        {
                            RestaurantId = model.RestaurantId,
                            MenuItemId = model.MenuItemId,
                            MenuItemName = model.MenuItemName,
                            MenuCategory = model.MenuCategory,
                            Packs = model.Packs,
                            MenuRecipeImage = model.MenuRecipeImage,
                            MenuRecipeNo = model.MenuRecipeNo,
                            ProductId = productId[i],
                            Productcode = productcode[i],
                            ProductType = productType[i],
                            ProductName = productName[i],
                            ProductUnit = productUnit[i],
                            ProductCategory = productCategory[i],
                            MenuRecipeQty = Convert.ToDecimal(MenuRecipeQty[i])
                        };
                        _context.Add(menuRecipe);
                    }
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return Problem("An error occurred while processing your request. Please try again later.");
            }
        }
        //Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var cateringRecipe = await _context.MenuRecipes
                .FirstOrDefaultAsync(p => p.MenuRecipeId == id);
            if (cateringRecipe == null)
            {
                return NotFound();
            }
            var cateringRecipeNo = cateringRecipe.MenuRecipeNo;
            var relatedPurchaseOrders = await _context.MenuRecipes
                .Where(p => p.MenuRecipeNo == cateringRecipeNo)
                .ToListAsync();
            ViewData["RelatedPurchaseOrders"] = relatedPurchaseOrders;
            return View(cateringRecipe);
        }

        //Get Edit 
        public async Task<IActionResult> Edit(int? id)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.OwnerEmail == userEmail);
            var products = _context.Products.Where(p => p.RestaurantId == restaurant.RestaurantId && p.CreatedBy == true).ToList();
            ViewBag.MenuItems = new SelectList(_context.MenuItems, "MenuItemId", "ItemName");
            ViewBag.Product = new SelectList(products, "ProductId", "ProductName");
            if (id == null)
            {
                return NotFound();
            }
            var menuRecipe = await _context.MenuRecipes.FindAsync(id);
            if (menuRecipe == null)
            {
                return NotFound();
            }
            var relatedPurchaseOrders = await _context.MenuRecipes
                .Where(p => p.MenuItemId == menuRecipe.MenuItemId)
                .ToListAsync();
            ViewData["RelatedPurchaseOrders"] = relatedPurchaseOrders;
            return View(menuRecipe);
        }
        //Post Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MenuRecipeId,ResturantId,MenuRecipeNo,MenuItemId,MenuItemName,MenuCategory,MenuRecipeImage,ProductId,ProductType,ProductName,ProductUnit,ProductCategory,CateringRecipeQty")] List<string> productType, List<string> productName, List<string> productcode, List<string> productUnit, List<string> productCategory, List<decimal> MenuRecipeQty, List<string> productId, MenuRecipe model)
        {
            if (id != model.MenuRecipeId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                    var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);
                    if (restaurant == null)
                    {
                        return RedirectToAction("Index", "Home"); 
                    }
                    var relatedMenuRecipes = await _context.MenuRecipes
                        .Where(p => p.MenuRecipeNo == model.MenuRecipeNo && p.RestaurantId == restaurant.RestaurantId)
                        .ToListAsync();
                    for (int i = 0; i < productType.Count; i++)
                    {
                        var menuRecipe = relatedMenuRecipes.ElementAtOrDefault(i) ?? new MenuRecipe();
                        menuRecipe.RestaurantId = model.RestaurantId;
                        menuRecipe.MenuItemId = model.MenuItemId;
                        menuRecipe.MenuItemName = model.MenuItemName;
                        menuRecipe.MenuCategory = model.MenuCategory;
                        menuRecipe.MenuRecipeImage = model.MenuRecipeImage;
                        menuRecipe.Packs = model.Packs;
                        menuRecipe.MenuRecipeNo = model.MenuRecipeNo;
                        menuRecipe.ProductId = i < productId.Count ? productId[i] : null;
                        menuRecipe.Productcode = i < productcode.Count ? productcode[i] : null;
                        menuRecipe.ProductType = i < productType.Count ? productType[i] : null;
                        menuRecipe.ProductName = i < productName.Count ? productName[i] : null;
                        menuRecipe.ProductUnit = i < productUnit.Count ? productUnit[i] : null;
                        menuRecipe.ProductCategory = i < productCategory.Count ? productCategory[i] : null;
                        menuRecipe.MenuRecipeQty = Convert.ToDecimal(MenuRecipeQty[i]);
                        _context.Update(menuRecipe);
                    }
                    var recipesToDelete = _context.MenuRecipes
                        .Where(r => r.MenuItemId == model.MenuItemId || r.MenuRecipeNo == model.MenuRecipeNo)
                        .ToList();

                    foreach (var recipeToDelete in recipesToDelete)
                    {
                        _context.MenuRecipes.Remove(recipeToDelete);
                    }
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                }
            }
            return View(model);
        }
        //Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var menuRecipe = await _context.MenuRecipes
             .FirstOrDefaultAsync(p => p.MenuRecipeId == id);
            if (menuRecipe == null)
            {
                return NotFound();
            }
            return View(menuRecipe);
        }
        // POST: Menurecipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var menuRecipe = _context.MenuRecipes.Find(id);
            if (menuRecipe == null)
            {
                return NotFound();
            }
            string menuItemId = menuRecipe.MenuItemId;
            _context.MenuRecipes.Remove(menuRecipe);
            _context.SaveChanges();
            var relatedMenuRecipes = _context.MenuRecipes
                .Where(p => p.MenuItemId == menuItemId)
                .ToList();
            foreach (var relatedRecipe in relatedMenuRecipes)
            {
                _context.MenuRecipes.Remove(relatedRecipe);
            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}