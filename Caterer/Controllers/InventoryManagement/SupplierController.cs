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
namespace Caterer.Controllers.InventoryManagement
{
    public class SupplierController : Controller
    {
        private readonly ApplicationDbContext _context;
        public SupplierController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: Supplier
        public async Task<IActionResult> SupplierList()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                var suppliers = await _context.Suppliers
     .Where(s => s.RestaurantId == restaurant.RestaurantId)
    
     .OrderByDescending(s => s.SupplierId)
     .ToListAsync();
                return View("~/Views/InventoryManagement/Supplier/SupplierList.cshtml", suppliers);
            }
            return Problem("Entity set 'ApplicationDbContext.Suppliers' is null or user is not associated with any restaurant.");
        }
        //Save Warehouse Data 
        [HttpPost]
        public JsonResult SaveWarehouse1(Warehouse warehousemodel)
        {
            var WarehousesData = new Warehouse()
            {
                RestaurantId = warehousemodel.RestaurantId,
                WarehouseName = warehousemodel.WarehouseName,
                PhoneNumber = warehousemodel.PhoneNumber,
                Email = warehousemodel.Email,
                BillingAddress = warehousemodel.BillingAddress,
                BankDetails = warehousemodel.BankDetails
            };
            _context.Warehouses.Add(WarehousesData);
            _context.SaveChanges();
            return Json(WarehousesData.WarehouseId);
        }
        // GET: Supplier/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Suppliers == null)
            {
                return NotFound();
            }
            var suppliers = await _context.Suppliers
                .FirstOrDefaultAsync(m => m.SupplierId == id);
            if (suppliers == null)
            {
                return NotFound();
            }
            return View("~/Views/InventoryManagement/Supplier/Details.cshtml", suppliers);
        }
        // GET: Supplier/Create
        public IActionResult SupplierCreate()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants
                .FirstOrDefault(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                ViewData["RestaurantId"] = restaurant.RestaurantId;
                var warehouses = _context.Warehouses
                    .Where(w => w.RestaurantId == restaurant.RestaurantId)
                    .Select(w => new SelectListItem
                    {
                        Value = w.WarehouseId.ToString(),
                        Text = w.WarehouseName
                    })
                    .ToList();
                ViewData["Warehouses"] = warehouses;
                return View("~/Views/InventoryManagement/Supplier/SupplierCreate.cshtml");
            }
            return RedirectToAction("Index", "Home"); 
        }
        //Post Supplier Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupplierCreate([Bind("SupplierId,SupplierName,WarehouseId,Email,PhoneNumber,Status,OpeningBalance,CreditPeriod,CreditLimit,ShippingAddress,BillingAddress")] Supplier suppliers)
        {
            if (ModelState.IsValid)
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var restaurant = await _context.Restaurants
                    .FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);
                if (restaurant == null)
                {
                    return RedirectToAction("Index", "Home");  
                }
                suppliers.RestaurantId = restaurant.RestaurantId;
                _context.Add(suppliers);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(SupplierList));
            }
            return View("~/Views/InventoryManagement/Supplier/SupplierList.cshtml", suppliers);
        }
        public async Task<IActionResult> SupplierEdit(int? id)
        {
            if (id == null || _context.Suppliers == null)
            {
                return NotFound();
            }
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants
                .FirstOrDefault(r => r.OwnerEmail == userEmail);
            if (restaurant == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["RestaurantId"] = restaurant.RestaurantId;
            supplier.RestaurantId = restaurant.RestaurantId;
            var warehouses = _context.Warehouses
                .Where(w => w.RestaurantId == restaurant.RestaurantId)
                .Select(w => new SelectListItem
                {
                    Value = w.WarehouseId.ToString(),
                    Text = w.WarehouseName
                })
                .ToList();
            ViewData["Warehouses"] = warehouses;
            return View("~/Views/InventoryManagement/Supplier/SupplierEdit.cshtml", supplier);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupplierEdit(int id, [Bind("SupplierId,SupplierName,WarehouseId,Email,PhoneNumber,Status,OpeningBalance,CreditPeriod,CreditLimit,ShippingAddress,BillingAddress")] Supplier suppliers)
        {
            if (id != suppliers.SupplierId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                    var restaurant = _context.Restaurants
                        .FirstOrDefault(r => r.OwnerEmail == userEmail);

                    if (restaurant == null)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    suppliers.RestaurantId = restaurant.RestaurantId;
                    var warehouseList = _context.Warehouses
                        .Where(w => w.RestaurantId == suppliers.RestaurantId)
                        .Select(w => new SelectListItem
                        {
                            Value = w.WarehouseId.ToString(),
                            Text = w.WarehouseName
                        })
                        .ToList();

                    ViewData["Warehouses"] = warehouseList;

                    _context.Update(suppliers);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SuppliersExists(suppliers.SupplierId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(SupplierList));
            }
            var warehouses = _context.Warehouses
                .Where(w => w.RestaurantId == suppliers.RestaurantId)
                .Select(w => new SelectListItem
                {
                    Value = w.WarehouseId.ToString(),
                    Text = w.WarehouseName
                })
                .ToList();
            ViewData["Warehouses"] = warehouses;
            return View("~/Views/InventoryManagement/Supplier/SupplierEdit.cshtml", suppliers);
        }
        // GET: Supplier/Delete/5
        public async Task<IActionResult> SupplierDelete(int? id)
        {
            if (id == null || _context.Suppliers == null)
            {
                return NotFound();
            }
            var suppliers = await _context.Suppliers
                .FirstOrDefaultAsync(m => m.SupplierId == id);
            if (suppliers == null)
            {
                return NotFound();
            }
            return View("~/Views/InventoryManagement/Supplier/SupplierDelete.cshtml", suppliers);
        }
        [HttpPost, ActionName("SupplierDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Suppliers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Suppliers'  is null.");
            }
            var suppliers = await _context.Suppliers.FindAsync(id);
            if (suppliers != null)
            {
                _context.Suppliers.Remove(suppliers);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(SupplierList));
        }
        private bool SuppliersExists(int id)
        {
            return (_context.Suppliers?.Any(e => e.SupplierId == id)).GetValueOrDefault();
        }
    }
}
