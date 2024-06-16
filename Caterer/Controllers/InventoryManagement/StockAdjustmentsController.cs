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
    public class StockAdjustmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public StockAdjustmentsController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: StockAdjustments
        public async Task<IActionResult> StockAdjustmentList()
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
                var stockAdjustments = await _context.StockAdjustments
                    .Include(sa => sa.Product)
                    .Where(sa => sa.RestaurantId == restaurant.RestaurantId)
                    .ToListAsync();
                return View("~/Views/InventoryManagement/StockAdjustments/StockAdjustmentList.cshtml", stockAdjustments);
            }
            catch (Exception ex)
            {
                return Problem("An error occurred while processing your request. Please try again later.");
            }
        }
        //Get Products By WarehouseId
        [HttpGet]
        public async Task<IActionResult> GetProductsByWarehouseId(int warehouseId)
        {
            var products = await _context.Products
                .Where(p => p.WarehouseId == warehouseId)
                .ToListAsync();
            var productViewModels = products.Select(p => new ProductViewModel
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName
            }).ToList();
            return Json(productViewModels);
        }
        // GET: StockAdjustments/Create
        public IActionResult StockAdjustmentCreate()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.OwnerEmail == userEmail);
            var warehouse = _context.Warehouses.Where(p => p.RestaurantId == restaurant.RestaurantId).ToList();
            ViewBag.warehouse = new SelectList(warehouse, "WarehouseId", "WarehouseName");
            if (restaurant != null)
            {
                ViewData["RestaurantId"] = restaurant.RestaurantId;

                return View("~/Views/InventoryManagement/StockAdjustments/StockAdjustmentCreate.cshtml");
            }
            return RedirectToAction("Index", "Home");
        }
        //Post Stock Adjustment Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StockAdjustmentCreate(StockAdjustment stockAdjustment)
        {
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);
                if (restaurant == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                stockAdjustment.RestaurantId = restaurant.RestaurantId;
                if (ModelState.IsValid)
                {
                    var product = await _context.Products.FindAsync(stockAdjustment.ProductId);
                    var warehouse = _context.Warehouses.Where(p => p.RestaurantId == restaurant.RestaurantId).ToList();
                    ViewBag.warehouse = new SelectList(warehouse, "WarehouseId", "WarehouseName");

                    if (product != null)
                    {
                        if (stockAdjustment.AdjustmentType == "Increase")
                        {
                            product.Quantity += stockAdjustment.Quantity;
                        }
                        else if (stockAdjustment.AdjustmentType == "Decrease")
                        {
                            if (product.Quantity >= stockAdjustment.Quantity)
                            {
                                product.Quantity -= stockAdjustment.Quantity;
                            }
                            else
                            {
                                ModelState.AddModelError(nameof(StockAdjustment.Quantity), "Insufficient stock for the requested decrease.");
                                return View("~/Views/InventoryManagement/StockAdjustments/StockAdjustmentCreate.cshtml", stockAdjustment);
                            }
                        }
                        _context.Add(stockAdjustment);
                        await _context.SaveChangesAsync();
                        _context.Update(product);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(StockAdjustmentList));
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                return View("~/Views/InventoryManagement/StockAdjustments/StockAdjustmentCreate.cshtml", stockAdjustment);
            }
            catch (Exception ex)
            {
                return Problem("An error occurred while processing your request. Please try again later.");
            }
        }
        // GET: StockAdjustments/Edit/5
        public async Task<IActionResult> StockAdjustmentEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var stockAdjustment = await _context.StockAdjustments.FindAsync(id);
            if (stockAdjustment == null)
            {
                return NotFound();
            }
            var products = _context.Products.ToList();
            ViewBag.Products = new SelectList(products, "ProductId", "ProductName", stockAdjustment.ProductId);
            return View("~/Views/InventoryManagement/StockAdjustments/StockAdjustmentEdit.cshtml", stockAdjustment);
        }
        // POST: StockAdjustments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StockAdjustmentEdit(int id, StockAdjustment stockAdjustment)
        {
            if (id != stockAdjustment.StockAdjustmentId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var existingStockAdjustment = await _context.StockAdjustments.FindAsync(id);
                    if (existingStockAdjustment == null)
                    {
                        return NotFound();
                    }
                    var product = await _context.Products.FindAsync(existingStockAdjustment.ProductId);
                    if (product == null)
                    {
                        return NotFound();
                    }
                    AdjustProductQuantity(existingStockAdjustment, product, adjust: false);
                    existingStockAdjustment.ProductId = stockAdjustment.ProductId;
                    existingStockAdjustment.AdjustmentType = stockAdjustment.AdjustmentType;
                    existingStockAdjustment.Quantity = stockAdjustment.Quantity;
                    existingStockAdjustment.AdjustmentReason = stockAdjustment.AdjustmentReason;
                    AdjustProductQuantity(existingStockAdjustment, product, adjust: true);
                    _context.Update(existingStockAdjustment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockAdjustmentExists(stockAdjustment.StockAdjustmentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(StockAdjustmentList));
            }
            var products = _context.Products.ToList();
            ViewBag.Products = new SelectList(products, "ProductId", "ProductName", stockAdjustment.ProductId);
            return View("~/Views/InventoryManagement/StockAdjustments/StockAdjustmentEdit.cshtml", stockAdjustment);
        }
        // Helper method to adjust product quantity based on the stock adjustment
        private void AdjustProductQuantity(StockAdjustment stockAdjustment, Product product, bool adjust)
        {
            if (adjust)
            {
                if (stockAdjustment.AdjustmentType == "Increase")
                {
                    product.Quantity += stockAdjustment.Quantity;
                }
                else if (stockAdjustment.AdjustmentType == "Decrease")
                {
                    if (product.Quantity >= stockAdjustment.Quantity)
                    {
                        product.Quantity -= stockAdjustment.Quantity;
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(StockAdjustment.Quantity), "Insufficient stock for the requested decrease.");
                    }
                }
            }
        }
        // GET: StockAdjustments/Delete/5
        public async Task<IActionResult> StockAdjustmentDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var stockAdjustment = await _context.StockAdjustments
                .Include(sa => sa.Product)
                .FirstOrDefaultAsync(m => m.StockAdjustmentId == id);
            if (stockAdjustment == null)
            {
                return NotFound();
            }
            return View("~/Views/InventoryManagement/StockAdjustments/StockAdjustmentDelete.cshtml", stockAdjustment);
        }
        // POST: StockAdjustments/Delete/5
        [HttpPost, ActionName("StockAdjustmentDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StockAdjustmentDeleteConfirmed(int id)
        {
            var stockAdjustment = await _context.StockAdjustments.FindAsync(id);
            if (stockAdjustment == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(stockAdjustment.ProductId);
            if (product == null)
            {
                return NotFound();
            }
            AdjustProductQuantity(stockAdjustment, product, adjust: false);
            _context.StockAdjustments.Remove(stockAdjustment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(StockAdjustmentList));
        }
        private bool StockAdjustmentExists(int id)
        {
            return _context.StockAdjustments.Any(e => e.StockAdjustmentId == id);
        }
    }
}