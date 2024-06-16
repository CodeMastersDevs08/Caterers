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
    public class WarehousesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public WarehousesController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: Warehouses
        public async Task<IActionResult> Index()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                var warehouses = await _context.Warehouses
                    .Where(w => w.RestaurantId == restaurant.RestaurantId)
                    .ToListAsync();
                ViewBag.Warehouses = warehouses;

                return View(warehouses);
            }
            return RedirectToAction("Index", "Home");  
        }

        // GET: Warehouses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var warehouse = await _context.Warehouses
                .FirstOrDefaultAsync(m => m.WarehouseId == id);
            if (warehouse == null)
            {
                return NotFound();
            }
            var warehouseNames = await _context.Warehouses
                .Where(w => w.RestaurantId == warehouse.RestaurantId)  
                .Select(w => w.WarehouseName)
                .ToListAsync();
            ViewBag.WarehouseNames = warehouseNames;
            return View(warehouse);
        }
         
        // GET: Warehouses/Create
        public IActionResult Create()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                ViewData["RestaurantId"] = restaurant.RestaurantId;
                return View();
            }
            return RedirectToAction("Index", "Home");  
        }
        // POST: Warehouses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WarehouseId,WarehouseName,RestaurantId,Email,PhoneNumber,BillingAddress,BankDetails")] Warehouse warehouse)
        {
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);
                if (restaurant == null)
                {
                    return RedirectToAction("Index", "Home");  
                }
                var existingWarehouse = await _context.Warehouses
                    .FirstOrDefaultAsync(w => w.WarehouseName == warehouse.WarehouseName && w.RestaurantId == restaurant.RestaurantId);
                if (existingWarehouse != null)
                {
                    ModelState.AddModelError("WarehouseName", "A warehouse with the same name already exists for this restaurant.");
                    ViewData["RestaurantId"] = restaurant.RestaurantId;
                    return View(warehouse);
                }
                warehouse.RestaurantId = restaurant.RestaurantId;
                if (ModelState.IsValid)
                {
                    _context.Add(warehouse);
                    await _context.SaveChangesAsync();
                    var existingProducts = await _context.Products.Where(p => p.RestaurantId == restaurant.RestaurantId && p.CreatedBy == true).ToListAsync();
                    foreach (var product in existingProducts)
                    {
                        var newProduct = new Product
                        {
                            WarehouseId = warehouse.WarehouseId,  
                            RestaurantId = restaurant.RestaurantId,  
                            ProductName = product.ProductName,
                            ProductType = product.ProductType,
                            MeasurementId = product.MeasurementId,
                            CategoryId = product.CategoryId,
                            productcode = product.productcode,
                            Barcode = product.Barcode,
                            ProductDescription = product.ProductDescription,
                            Quantity = 0,
                            UnitPrice = product.UnitPrice,
                            UnitCost = product.UnitCost,
                            StockControl = product.StockControl,
                            ExpireDate = product.ExpireDate,
                            Instock = product.Instock,
                            SafetyStock = product.SafetyStock,
                            TaxId = product.TaxId,
                            OpeningStock = product.OpeningStock,
                            OpeningStockDate = product.OpeningStockDate,
                            Mrp = product.Mrp,
                            CreatedBy = false,
                            ProductImage = product.ProductImage,
                        };
                        _context.Products.Add(newProduct);

                    }
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewData["RestaurantId"] = restaurant.RestaurantId;
                return View(warehouse);
            }
            catch (Exception ex)
            {
                return Problem("An error occurred while processing your request. Please try again later.");
            }
        }
        // GET: Warehouses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Warehouses == null)
            {
                return NotFound();
            }
            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);
            if (restaurant == null || warehouse.RestaurantId != restaurant.RestaurantId)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["RestaurantId"] = restaurant.RestaurantId;
            return View(warehouse);
        }
        // POST: Warehouses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WarehouseId,RestaurantId,WarehouseName,Email,PhoneNumber,BillingAddress,BankDetails")] Warehouse warehouse)
        {
            if (id != warehouse.WarehouseId)
            {
                return NotFound();
            }
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);

            if (restaurant == null || warehouse.RestaurantId != restaurant.RestaurantId)
            {
                return RedirectToAction("Index", "Home");
            }
            warehouse.RestaurantId = restaurant.RestaurantId;  
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(warehouse);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WarehouseExists(warehouse.WarehouseId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RestaurantId"] = restaurant.RestaurantId;
            return View(warehouse);
        }
        // GET: Warehouses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Warehouses == null)
            {
                return NotFound();
            }
            var warehouse = await _context.Warehouses
                .FirstOrDefaultAsync(m => m.WarehouseId == id);
            if (warehouse == null)
            {
                return NotFound();
            }
            return View(warehouse);
        }
        // POST: Warehouses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Warehouses == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Warehouse'  is null.");
            }
            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse != null)
            {
                _context.Warehouses.Remove(warehouse);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool WarehouseExists(int id)
        {
            return _context.Warehouses.Any(e => e.WarehouseId == id);
        }
    }
}