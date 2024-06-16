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
using Microsoft.CodeAnalysis;
namespace Caterer.Controllers
{
    public class ProductionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductionsController(ApplicationDbContext context)
        {
            _context = context;
        }
        //Details
        public async Task<IActionResult> ProductionDetails(int id)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                var warehouse = _context.Warehouses.Where(p => p.RestaurantId == restaurant.RestaurantId).ToList();
                ViewBag.warehouse = new SelectList(warehouse, "WarehouseId", "WarehouseName");
            }
            var orderDetails = _context.OrderDetailWebsites
                                        .Where(o => o.OrderDetailsWebsiteId == id)
                                        .ToList();
            if (orderDetails.Count == 0)
            {
                return NotFound();
            }
            ViewBag.OrderDetailsWebsiteId = id;
            var productions = (from a in orderDetails
                               from b in _context.Productions.ToList()
                               join warehouse in _context.Warehouses.ToList()
                               on b.WarehouseId equals warehouse.WarehouseId
                               where a.CateringItemId.Split(',').Contains(b.MenuItemId.ToString()) && a.OrderDetailsWebsiteId == b.OrderDetailsWebsiteId
                               select new
                               {
                                   b.ProductionDate,
                                   b.ProductionStatus,
                                   b.OrderDetailsWebsiteId,
                                   b.CateringCategoryId,
                                   b.RestaurantId,
                                   b.OrderStatus,
                                   b.MenuItemId,
                                   b.MenuCategory,
                                   b.MenuRecipePacks,
                                   b.TotalNoOfPacks,
                                   b.ProductId,
                                   b.ProductCode,
                                   b.ProductUnit,
                                   b.MenuItemName,
                                   b.MenuRecipeQty,
                                   b.ProductName,
                                   b.ProductCategory,
                                   b.ProductType,
                                   b.UsedQuantity,
                                   b.TotalQuantity,
                                   b.WarehouseId,
                                   warehouse.WarehouseName
                               }).ToList();
            var firstProduction = productions.FirstOrDefault();
            ViewBag.FirstProduction = firstProduction;
            var productions1 = (from a in orderDetails
                                from b in _context.Productions
                                let extrasCateringItemIds = a.ExtraCateringItemId.Split(',').ToList()
                                let extraCateringNoOfPax = a.ExtraNoOfPax.Split(',')
                                                    .Select(s => { int.TryParse(s, out int result); return result; })
                                                    .ToList()
                                let zipped = extrasCateringItemIds.Zip(extraCateringNoOfPax, (itemId, pax) => new { ItemId = itemId, Pax = pax })
                                from c in zipped
                                where b.MenuItemId.ToString() == c.ItemId && a.OrderDetailsWebsiteId == b.OrderDetailsWebsiteId
                                select new
                                {
                                    b.OrderDetailsWebsiteId,
                                    b.CateringCategoryId,
                                    b.RestaurantId,
                                    b.OrderStatus,
                                    b.MenuItemId,
                                    b.MenuCategory,
                                    b.MenuRecipePacks,
                                    b.TotalNoOfPacks,                                    
                                    b.ProductId,
                                    b.ProductCode,
                                    b.ProductUnit,
                                    b.MenuItemName,
                                    b.MenuRecipeQty,
                                    b.ProductName,
                                    b.ProductCategory,
                                    b.ProductType,
                                    b.UsedQuantity,
                                    b.TotalQuantity,
                                    b.WarehouseId,
                                }).ToList();
            var productions2 = (from a in orderDetails
                                from b in _context.Productions
                                let extrasCateringItemIds = a.ExtrasCateringItemId.Split(',').ToList()
                                let extraCateringNoOfPax = a.ExtraCateringNoOfPax.Split(',')
                                                    .Select(s => { int.TryParse(s, out int result); return result; })
                                                    .ToList()
                                let zipped = extrasCateringItemIds.Zip(extraCateringNoOfPax, (itemId, pax) => new { ItemId = itemId, Pax = pax })
                                from c in zipped
                                where b.MenuItemId.ToString() == c.ItemId && a.OrderDetailsWebsiteId == b.OrderDetailsWebsiteId
                                select new
                                {
                                    b.OrderDetailsWebsiteId,
                                    b.CateringCategoryId,
                                    b.RestaurantId,
                                    b.OrderStatus,
                                    b.MenuItemId,
                                    b.MenuCategory,
                                    b.MenuRecipePacks,
                                    b.TotalNoOfPacks,
                                    b.ProductId,
                                    b.ProductCode,
                                    b.ProductUnit,
                                    b.MenuItemName,
                                    b.MenuRecipeQty,
                                    b.ProductName,
                                    b.ProductCategory,
                                    b.ProductType,
                                    b.UsedQuantity,
                                    b.TotalQuantity,
                                    b.WarehouseId,
                                    b.ExtraPacks
                                }).ToList();
            ViewBag.OrderMenus = productions;
            ViewBag.MenuAdjustment = productions1;
            ViewBag.ExtraOrderMenu = productions2;
            return View();
        }
        //Index
        public async Task<IActionResult> ProductionIndex()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                var productions = await _context.Productions
                    .Where(p => p.RestaurantId == restaurant.RestaurantId)
                    .GroupBy(p => p.OrderDetailsWebsiteId)
                    .Select(g => g.First())
                    .ToListAsync();
                if (productions.Any())
                {
                    return View(productions);
                }
            }
            return View();
        }
        //Get Create
        public IActionResult ProductionCreate(int id)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                var warehouse = _context.Warehouses.Where(p => p.RestaurantId == restaurant.RestaurantId).ToList();
                ViewBag.warehouse = new SelectList(warehouse, "WarehouseId", "WarehouseName");
            }
            var orderDetails = _context.OrderDetailWebsites
                                        .Where(o => o.OrderDetailsWebsiteId == id)
                                        .ToList();
            if (orderDetails.Count == 0)
            {
                return NotFound();
            }
            var productions = (from a in orderDetails
                               from b in _context.MenuRecipes
                               where a.CateringItemId.Split(',').Contains(b.MenuItemId.ToString())
                               select new
                               {
                                   a.OrderDetailsWebsiteId,
                                   a.CateringCategoryId,
                                   a.RestaurantId,
                                   a.Status,
                                   a.CateringItemId,
                                   a.NoOfPerson,
                                   b.MenuItemId,
                                   b.MenuCategory,
                                   b.Packs,
                                   b.ProductId,
                                   b.Productcode,
                                   b.ProductUnit,
                                   b.MenuItemName,
                                   b.MenuRecipeQty,
                                   b.ProductName,
                                   b.ProductCategory,
                                   b.ProductType
                               }).ToList();
            var productions1 = (from a in orderDetails
                                from b in _context.MenuRecipes                                
                                let extrasCateringItemIds = a.ExtraCateringItemId.Split(',').ToList()
                                let extraCateringNoOfPax = a.ExtraNoOfPax.Split(',')
                                                    .Select(s => { int.TryParse(s, out int result); return result; })
                                                    .ToList()
                                let zipped = extrasCateringItemIds.Zip(extraCateringNoOfPax, (itemId, pax) => new { ItemId = itemId, Pax = pax })
                                from c in zipped
                                where b.MenuItemId.ToString() == c.ItemId
                                select new
                                {
                                    a.OrderDetailsWebsiteId,
                                    a.CateringCategoryId,
                                    a.RestaurantId,
                                    a.Status,
                                    ExtraNoOfPax = c.Pax,
                                    b.MenuItemId,
                                    b.MenuCategory,
                                    b.Packs,
                                    b.ProductId,
                                    b.Productcode,
                                    b.ProductUnit,
                                    b.MenuItemName,
                                    b.MenuRecipeQty,
                                    b.ProductName,
                                    b.ProductCategory,
                                    b.ProductType
                                }).ToList();
            var productions2 = (from a in orderDetails
                                from b in _context.MenuRecipes
                                let extrasCateringItemIds = a.ExtrasCateringItemId.Split(',').ToList()
                                let extraCateringNoOfPax = a.ExtraCateringNoOfPax.Split(',')
                                                    .Select(s => { int.TryParse(s, out int result); return result; })
                                                    .ToList()
                                let zipped = extrasCateringItemIds.Zip(extraCateringNoOfPax, (itemId, pax) => new { ItemId = itemId, Pax = pax })
                                from c in zipped
                                where b.MenuItemId.ToString() == c.ItemId
                                select new
                                {
                                    a.OrderDetailsWebsiteId,
                                    a.CateringCategoryId,
                                    a.RestaurantId,
                                    a.Status,
                                    ExtraCateringNoOfPax = c.Pax,
                                    b.MenuItemId,
                                    b.MenuCategory,
                                    b.Packs,
                                    b.ProductId,
                                    b.Productcode,
                                    b.ProductUnit,
                                    b.MenuItemName,
                                    b.MenuRecipeQty,
                                    b.ProductName,
                                    b.ProductCategory,
                                    b.ProductType
                                }).ToList();
            ViewBag.OrderMenus = productions;
            ViewBag.MenuAdjustment = productions1;
            ViewBag.ExtraOrderMenu = productions2;
            return View();
        }
        //Post Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductionStore(
          DateTime? ProductionDate, string ProductionStatus,
          int WarehouseId,
          List<string> ProductCode,
          List<int> OrderDetailsWebsiteId,
          List<string> OrderStatus,
          List<int> RestaurantId,
          List<int> CateringCategoryId,
          List<int> TotalNoOfPacks,
          List<int> ExtraPacks,
          List<int> MenuItemId,
          List<string> MenuCategory,
          List<string> MenuItemName,
          List<int> ProductId,
          List<string> ProductName,
          List<string> ProductCategory,
          List<string> ProductType,
          List<string> ProductUnit,
          List<int> MenuRecipePacks,
          List<decimal?> MenuRecipeQty,
          List<decimal?> UsedQuantity,
          List<decimal?> TotalQuantity)
        {
            if (ModelState.IsValid)
            {
                List<Production> productionData = new List<Production>();

                for (int i = 0; i < OrderDetailsWebsiteId.Count; i++)
                {
                    var production = new Production
                    {
                        WarehouseId = WarehouseId,
                        ProductCode = ProductCode[i],
                        ProductionDate = ProductionDate,
                        ProductionStatus = ProductionStatus,
                        OrderDetailsWebsiteId = OrderDetailsWebsiteId[i],
                        OrderStatus = OrderStatus[i],
                        RestaurantId = RestaurantId[i],
                        CateringCategoryId = CateringCategoryId[i],
                        TotalNoOfPacks = TotalNoOfPacks[i],
                        ExtraPacks = ExtraPacks[i],
                        MenuItemId = MenuItemId[i],
                        MenuCategory = MenuCategory[i],
                        MenuItemName = MenuItemName[i],
                        ProductId = ProductId[i],
                        ProductName = ProductName[i],
                        ProductCategory = ProductCategory[i],
                        ProductType = ProductType[i],
                        ProductUnit = ProductUnit[i],
                        MenuRecipePacks = MenuRecipePacks[i],
                        MenuRecipeQty = MenuRecipeQty[i],
                        UsedQuantity = UsedQuantity[i],
                        TotalQuantity = TotalQuantity[i]
                    };
                    productionData.Add(production);
                }
                for (int i = 0; i < ProductId.Count; i++)
                {
                    if (ProductionStatus == "Completed")
                    {
                        if (!string.IsNullOrEmpty(ProductCode[i]))
                        {
                            var products = _context.Products
                                .Where(p => p.productcode.ToString() == ProductCode[i] && p.WarehouseId == WarehouseId)
                                .ToList();
                            if (products != null && products.Any())
                            {
                                foreach (var product in products)
                                {
                                    if (TotalQuantity[i] != null)
                                    {
                                        product.Quantity -= TotalQuantity[i];
                                    }
                                }
                            }
                        }
                    }
                }
                _context.AddRange(productionData);
                await _context.SaveChangesAsync();
                return RedirectToAction("ProductionIndex");
            }
            return View();
        }
        //Get Edit
        public IActionResult ProductionEdit(int id)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                var warehouse = _context.Warehouses.Where(p => p.RestaurantId == restaurant.RestaurantId).ToList();
                ViewBag.warehouse = new SelectList(warehouse, "WarehouseId", "WarehouseName");
            }
            var orderDetails = _context.OrderDetailWebsites
                                        .Where(o => o.OrderDetailsWebsiteId == id)
                                        .ToList();
            if (orderDetails.Count == 0)
            {
                return NotFound();
            }
            ViewBag.OrderDetailsWebsiteId = id;
            var productions = (from a in orderDetails
                               from b in _context.Productions.ToList()
                               join warehouse in _context.Warehouses.ToList()
                               on b.WarehouseId equals warehouse.WarehouseId
                               where a.CateringItemId.Split(',').Contains(b.MenuItemId.ToString()) && a.OrderDetailsWebsiteId == b.OrderDetailsWebsiteId
                               select new
                               {
                                   b.ProductionDate,
                                   b.ProductionStatus,
                                   b.OrderDetailsWebsiteId,
                                   b.CateringCategoryId,
                                   b.RestaurantId,
                                   b.OrderStatus,
                                   b.MenuItemId,
                                   b.MenuCategory,
                                   b.MenuRecipePacks,
                                   b.TotalNoOfPacks,
                                   b.ProductId,
                                   b.ProductCode,
                                   b.ProductUnit,
                                   b.MenuItemName,
                                   b.MenuRecipeQty,
                                   b.ProductName,
                                   b.ProductCategory,
                                   b.ProductType,
                                   b.UsedQuantity,
                                   b.TotalQuantity,
                                   b.WarehouseId,
                                   warehouse.WarehouseName
                               }).ToList();
            var firstProduction = productions.FirstOrDefault();
            ViewBag.FirstProduction = firstProduction;
            var productions1 = (from a in orderDetails
                                from b in _context.Productions
                                let extrasCateringItemIds = a.ExtraCateringItemId.Split(',').ToList()
                                let extraCateringNoOfPax = a.ExtraNoOfPax.Split(',')
                                                    .Select(s => { int.TryParse(s, out int result); return result; })
                                                    .ToList()
                                let zipped = extrasCateringItemIds.Zip(extraCateringNoOfPax, (itemId, pax) => new { ItemId = itemId, Pax = pax })
                                from c in zipped
                                where b.MenuItemId.ToString() == c.ItemId && a.OrderDetailsWebsiteId == b.OrderDetailsWebsiteId
                                select new
                                {
                                    b.OrderDetailsWebsiteId,
                                    b.CateringCategoryId,
                                    b.RestaurantId,
                                    b.OrderStatus,
                                    b.MenuItemId,
                                    b.MenuCategory,
                                    b.MenuRecipePacks,
                                    b.TotalNoOfPacks,
                                    b.ProductId,
                                    b.ProductCode,
                                    b.ProductUnit,
                                    b.MenuItemName,
                                    b.MenuRecipeQty,
                                    b.ProductName,
                                    b.ProductCategory,
                                    b.ProductType,
                                    b.UsedQuantity,
                                    b.TotalQuantity,
                                    b.WarehouseId,
                                }).ToList();
            var productions2 = (from a in orderDetails
                                from b in _context.Productions
                                let extrasCateringItemIds = a.ExtrasCateringItemId.Split(',').ToList()
                                let extraCateringNoOfPax = a.ExtraCateringNoOfPax.Split(',')
                                                    .Select(s => { int.TryParse(s, out int result); return result; })
                                                    .ToList()
                                let zipped = extrasCateringItemIds.Zip(extraCateringNoOfPax, (itemId, pax) => new { ItemId = itemId, Pax = pax })
                                from c in zipped
                                where b.MenuItemId.ToString() == c.ItemId && a.OrderDetailsWebsiteId == b.OrderDetailsWebsiteId
                                select new
                                {
                                    b.OrderDetailsWebsiteId,
                                    b.CateringCategoryId,
                                    b.RestaurantId,
                                    b.OrderStatus,
                                    b.MenuItemId,
                                    b.MenuCategory,
                                    b.MenuRecipePacks,
                                    b.TotalNoOfPacks,
                                    b.ProductId,
                                    b.ProductCode,
                                    b.ProductUnit,
                                    b.MenuItemName,
                                    b.MenuRecipeQty,
                                    b.ProductName,
                                    b.ProductCategory,
                                    b.ProductType,
                                    b.UsedQuantity,
                                    b.TotalQuantity,
                                    b.WarehouseId,
                                    b.ExtraPacks
                                }).ToList();
            ViewBag.OrderMenus = productions;
            ViewBag.MenuAdjustment = productions1;
            ViewBag.ExtraOrderMenu = productions2;
            return View();
        }
        //Post Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductionUpdate(int OrderDetailsWebsiteId,
        DateTime? ProductionDate,
        string ProductionStatus,
        int WarehouseId,
        List<string> OrderStatus,
        List<int> CateringCategoryId,
        List<int> TotalNoOfPacks,
        List<int> ExtraPacks,
        List<int> MenuItemId,
        List<string> MenuCategory,
        List<string> MenuItemName,
        List<int> ProductId,
        List<string> ProductName,
        List<string> ProductCategory,
        List<string> ProductType,
        List<string> ProductUnit,
        List<int> MenuRecipePacks,
        List<decimal?> MenuRecipeQty,
        List<decimal?> UsedQuantity,
        List<decimal?> TotalQuantity)
        {
            if (ModelState.IsValid)
            {
                var productionsToUpdate = await _context.Productions
                    .Where(p => p.OrderDetailsWebsiteId == OrderDetailsWebsiteId)
                    .ToListAsync();

                if (productionsToUpdate == null || !productionsToUpdate.Any())
                {
                    return RedirectToAction("ProductionIndex");
                }
                for (int i = 0; i < productionsToUpdate.Count; i++)
                {
                    var production = productionsToUpdate[i];
                    production.WarehouseId = WarehouseId;
                    production.ProductionDate = ProductionDate;
                    production.ProductionStatus = ProductionStatus;
                    production.OrderStatus = OrderStatus[i];
                    production.CateringCategoryId = CateringCategoryId[i];
                    production.TotalNoOfPacks = TotalNoOfPacks[i];
                    production.ExtraPacks = ExtraPacks[i];
                    production.MenuItemId = MenuItemId[i];
                    production.MenuCategory = MenuCategory[i];
                    production.MenuItemName = MenuItemName[i];
                    production.ProductId = ProductId[i];
                    production.ProductName = ProductName[i];
                    production.ProductCategory = ProductCategory[i];
                    production.ProductType = ProductType[i];
                    production.ProductUnit = ProductUnit[i];
                    production.MenuRecipePacks = MenuRecipePacks[i];
                    production.MenuRecipeQty = MenuRecipeQty[i];
                    production.UsedQuantity = UsedQuantity[i];
                    production.TotalQuantity = TotalQuantity[i];
                    if (ProductionStatus == "Completed")
                    {
                        if (ProductId[i] != 0 && WarehouseId != 0)
                        {
                            var product = await _context.Products
                                .FirstOrDefaultAsync(p => p.ProductId == ProductId[i] && p.WarehouseId == WarehouseId);
                            if (product != null)
                            {
                                if (TotalQuantity[i] != null)
                                {
                                    product.Quantity -= TotalQuantity[i].Value;
                                }
                            }
                        }
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("ProductionIndex");
            }
            return View("~/Views/Productions/ProductionIndex.cshtml");
        }
    }
}