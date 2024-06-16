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
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;
using Caterer.Data.Migrations;
namespace Caterer.Controllers
{
    public class PreProductionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PreProductionsController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: PreProductions
        public async Task<IActionResult> Index()

        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);
            var PreProductions = await _context.PreProductions
            .Where(p => p.RestaurantId == restaurant.RestaurantId)
            .GroupBy(p => p.Id)  
            .Select(g => g.First())  
            .ToListAsync();
            if (PreProductions.Any())
            {
                return View(PreProductions);
            }
            return View();
        }
        //Details
        public async Task<IActionResult> Details(string id)
        {
            var preProduction = await _context.PreProductions
                .FirstOrDefaultAsync(p => p.Id == id);

            if (preProduction == null)
            {
                return NotFound();
            }
            var relatedPreProductions = await _context.PreProductions
                .Where(p => p.MenuItemId == preProduction.MenuItemId)
                .ToListAsync();
            var groupedPreProductions = relatedPreProductions.GroupBy(p => p.MenuItemName).ToList();
            ViewData["GroupedPreProductions"] = groupedPreProductions;
            return View(preProduction);
        }
        //Create
        public IActionResult Create()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                var warehouse = _context.Warehouses.Where(p => p.RestaurantId == restaurant.RestaurantId).ToList();
                ViewBag.warehouse = new SelectList(warehouse, "WarehouseId", "WarehouseName");
                var MenuRecipes = _context.MenuRecipes
                 .Where(p => p.RestaurantId == restaurant.RestaurantId)
                 .Select(p => new { p.MenuItemId, p.MenuItemName })
                 .Distinct()
                 .ToList();
                ViewBag.MenuRecipes = new SelectList(MenuRecipes, "MenuItemId", "MenuItemName");
            }
            return View();
        }
        //Get Menu ItemDetails
        public IActionResult GetMenuItemDetails(string id, int warehouseId)
        {
            var menuItemDetails = (from product in _context.Products
                                   join menuRecipe in _context.MenuRecipes
                                   on product.productcode.ToString() equals menuRecipe.Productcode
                                   where menuRecipe.MenuItemId == id && product.WarehouseId == warehouseId
                                   select new
                                   {
                                       RestaurantId = menuRecipe.RestaurantId,
                                       MenuItemId = menuRecipe.MenuItemId,
                                       MenuItemName = menuRecipe.MenuItemName,
                                       MenuCategory = menuRecipe.MenuCategory,
                                       ProductId = menuRecipe.ProductId,
                                       Productcode = menuRecipe.Productcode,
                                       ProductName = menuRecipe.ProductName,
                                       ProductUnit = menuRecipe.ProductUnit,
                                       ProductCategory = menuRecipe.ProductCategory,
                                       ProductType = menuRecipe.ProductType,
                                       MenuRecipeQty = menuRecipe.MenuRecipeQty,
                                       Packs = menuRecipe.Packs,
                                       Quantity = product.Quantity
                                   }).ToList();
            return Json(menuItemDetails);
        }
        //Post  Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string PreProductionStatus,
      string Type,
      DateTime? PreProductionDate,
      DateTime? ExpiryDate,
      int? WarehouseId,
      List<int> RestaurantId,
      string MenuItemId,
      List<string> MenuItemName,
      List<int> Packs,
      List<string> MenuCategory,
      List<string> ProductId,
      List<string> Productcode,
      List<string> ProductName,
      List<string> ProductUnit,
      List<string> ProductCategory,
      List<decimal> MenuRecipeQty,
      List<string> ProductType,
      List<decimal> TotalQuantity,
      List<decimal> TotalPack,
      List<decimal> ProductTotalQuantity)
        {
            if (ModelState.IsValid)
            {
                List<PreProduction> preproductionData = new List<PreProduction>();
                string latestId = _context.PreProductions.Max(m => m.Id);
                for (int i = 0; i < ProductId.Count; i++)
                {
                    var production = new PreProduction
                    {
                        Id = latestId + 1,
                        WarehouseId = WarehouseId,
                        PreProductionStatus = PreProductionStatus,
                        Type = Type,
                        PreProductionDate = PreProductionDate,
                        ExpiryDate = ExpiryDate,
                        Productcode = Productcode[i],
                        RestaurantId = RestaurantId[i],
                        MenuItemId = MenuItemId,
                        MenuCategory = MenuCategory[i],
                        MenuItemName = MenuItemName[i],
                        ProductId = ProductId[i],
                        ProductName = ProductName[i],
                        ProductCategory = ProductCategory[i],
                        ProductType = ProductType[i],
                        ProductUnit = ProductUnit[i],
                        MenuRecipeQty = MenuRecipeQty[i],
                        Packs = Packs[i],
                        TotalQuantity = TotalQuantity[i],
                        TotalPack = TotalPack[i],
                        ProductTotalQuantity = ProductTotalQuantity[i]
                    };
                    preproductionData.Add(production);
                }
                _context.AddRange(preproductionData);
                await _context.SaveChangesAsync();
                if (PreProductionStatus == "Completed")
                {
                    foreach (var production in preproductionData)
                    {
                        if (!string.IsNullOrEmpty(production.Productcode))
                        {
                            var product = _context.Products
                                .FirstOrDefault(p => p.productcode.ToString() == production.Productcode && p.WarehouseId == production.WarehouseId);

                            if (product != null)
                            {
                                if (production.TotalQuantity != null)
                                {
                                    product.Quantity -= production.TotalQuantity;
                                }
                            }
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Index");  
            }
            else
            {
                return View();
            }
        }
        // GET: PreProductions/Edit/5
        public IActionResult Edit(string? id)
        {
            ViewBag.Id = id;
            var data = (from product in _context.Products
                        join b in _context.PreProductions
                        on product.productcode.ToString() equals b.Productcode
                        where b.Id == id && b.WarehouseId == product.WarehouseId
                        select new
                        {
                            b.PreProductionStatus,
                            b.PreProductionDate,
                            b.ExpiryDate,
                            b.RestaurantId,
                            b.WarehouseId,
                            b.MenuItemId,
                            b.MenuItemName,
                            b.MenuCategory,
                            b.ProductId,
                            b.Productcode,
                            b.ProductName,
                            b.ProductUnit,
                            b.ProductCategory,
                            b.ProductType,
                            b.MenuRecipeQty,
                            b.Packs,
                            b.TotalQuantity,
                            b.TotalPack,
                            product.Quantity,
                            product.WarehouseName
                        }).ToList();
            var firstpreProduction = data.FirstOrDefault();
            ViewBag.FirstpreProduction = firstpreProduction;
            ViewBag.preproductions = data;
            return View();
        }
        //Post Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string Id, string PreProductionStatus, string Type, DateTime? PreProductionDate, DateTime? ExpiryDate,
      int? WarehouseId, List<int> RestaurantId, string MenuItemId, List<string> MenuItemName, List<int> Packs,
      List<string> MenuCategory, List<string> ProductId, List<string> Productcode, List<string> ProductName,
      List<string> ProductUnit, List<string> ProductCategory, List<decimal> MenuRecipeQty, List<string> ProductType,
      List<decimal> TotalQuantity, List<decimal> TotalPack, List<decimal> ProductTotalQuantity)
        {
            if (ModelState.IsValid)
            {
                var productionsToUpdate = await _context.PreProductions
                    .Where(p => p.Id == Id)
                    .ToListAsync();
                if (productionsToUpdate == null || !productionsToUpdate.Any())
                {
                    return RedirectToAction("Index");
                }
                for (int i = 0; i < productionsToUpdate.Count; i++)
                {
                    var production = productionsToUpdate[i];
                    production.WarehouseId = WarehouseId;
                    production.PreProductionStatus = PreProductionStatus;
                    production.Type = Type;
                    production.PreProductionDate = PreProductionDate;
                    production.ExpiryDate = ExpiryDate;
                    production.Productcode = Productcode[i];
                    production.RestaurantId = RestaurantId[i];
                    production.MenuItemId = MenuItemId;
                    production.MenuCategory = MenuCategory[i];
                    production.MenuItemName = MenuItemName[i];
                    production.ProductId = ProductId[i];
                    production.ProductName = ProductName[i];
                    production.ProductCategory = ProductCategory[i];
                    production.ProductType = ProductType[i];
                    production.ProductUnit = ProductUnit[i];
                    production.MenuRecipeQty = MenuRecipeQty[i];
                    production.Packs = Packs[i];
                    production.TotalQuantity = TotalQuantity[i];
                    production.TotalPack = TotalPack[i];
                    production.ProductTotalQuantity = ProductTotalQuantity[i];
                    if (PreProductionStatus == "Completed")
                    {
                        if (!string.IsNullOrEmpty(Productcode[i]) && WarehouseId.HasValue)
                        {
                            var product = await _context.Products
                                .FirstOrDefaultAsync(p => p.productcode.ToString() == Productcode[i] && p.WarehouseId == WarehouseId);
                            if (product != null)
                            {
                                if (TotalQuantity[i] != null)
                                {
                                    product.Quantity -= TotalQuantity[i];
                                }
                            }
                        }
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View();
        }
        //Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PreProductions == null)
            {
                return NotFound();
            }
            var preProduction = await _context.PreProductions
                .FirstOrDefaultAsync(m => m.PreProductionId == id);
            if (preProduction == null)
            {
                return NotFound();
            }
            return View(preProduction);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteEditRow(string id)
        {
            var Preproduction = _context.PreProductions
                       .Where(r => r.Id == id)
                       .ToList();
            foreach (var recipeToDelete in Preproduction)
            {
                _context.PreProductions.Remove(recipeToDelete);
            }
            await _context.SaveChangesAsync();
            return Ok();
        }
        private bool PreProductionExists(int id)
        {
            return _context.PreProductions.Any(e => e.PreProductionId == id);
        }
    }
}