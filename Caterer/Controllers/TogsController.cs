using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caterer.Data.Migrations;
using Caterer.Data;
using Caterer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
namespace Caterer.Controllers
{
    public class TogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TogsController(ApplicationDbContext context)
        {
            _context = context;
        }
        //Search Product In ProductName 
        [HttpGet]
        public IActionResult SearchProduct(string searchTerm, int warehouseId)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                var productNames = _context.Products
                    .Where(p =>
                        (p.ProductName.StartsWith(searchTerm) || p.Barcode.StartsWith(searchTerm)) &&
                        p.WarehouseId == warehouseId &&
                        p.RestaurantId == restaurant.RestaurantId)
                    .Select(p => p.ProductName)
                    .ToList();

                return Json(productNames);
            }
            return Json(new List<string>());
        }
        //Get Product Detail based on Product Name 
        public IActionResult GetProductDetails(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                return Json(null);
            }
            var productDetails = _context.Products
                .Include(p => p.Measurement)
                .FirstOrDefault(p => p.ProductName == productId);
            if (productDetails != null)
            {
                var productData = new
                {
                    measurementId = productDetails.MeasurementId,
                    measurementName = productDetails.Measurement?.MeasurementName,
                    unitCost = productDetails.UnitCost,
                };
                return Json(productData);
            }
            return Json(null);
        }
        // GET: GRN Index 
        public async Task<IActionResult> Index()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                var grns = await _context.Togs
                    .Include(a => a.Warehouse)
                    .Where(g => g.RestaurantId == restaurant.RestaurantId)
                     .OrderByDescending(s => s.TogId)
                    .ToListAsync();
                return View(grns);
            }
            return Problem("Entity set 'ApplicationDbContext.Restaurants' is null or user is not associated with any restaurant.");
        }
        // GET: Tog Details 
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var purchaseOrder = await _context.Togs
                .Where(p => p.TogId == id)
                .ToListAsync();

            if (purchaseOrder == null)
            {
                return NotFound();
            }
            var purchaseOrderNo = purchaseOrder.First().TNo;
            var relatedPurchaseOrders = await _context.Togs
                .Where(p => p.TNo == purchaseOrderNo)
                .ToListAsync();
            ViewData["RelatedPurchaseOrders"] = relatedPurchaseOrders;
            return View(purchaseOrder.First());
        }
        //Generate Automatic TNo
        private int GenerateAutomaticTNo()
        {
            var maxPNo = _context.Togs.Max(po => (int?)po.TNo) ?? 0;
            return maxPNo + 1;
        }
        //Generate Automatic PurchaseOrderNo
        private string GenerateAutomaticPurchaseOrderNo()
        {
            var maxPurchaseOrderNo = _context.Togs.Max(po => po.TogNO);
            if (!string.IsNullOrEmpty(maxPurchaseOrderNo) && maxPurchaseOrderNo.StartsWith("TOG-"))
            {
                var numericPart = maxPurchaseOrderNo.Substring(4);
                if (int.TryParse(numericPart, out var maxNumber))
                {
                    var newNumber = (maxNumber + 1).ToString("D4");
                    return $"TOG-{newNumber}";
                }
            }
            return "TOG-1000";
        }
        // GET: Togs/Create
        public IActionResult Create(string productName, string measurement, decimal quantity, decimal unitCost, decimal totalCost, decimal total, string purchaseOrderNo, DateTime purchaseOrderDate, string paymentMethod)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                var warehouseList = _context.Warehouses
             .Where(w => w.RestaurantId == restaurant.RestaurantId)
             .Select(w => new { w.WarehouseId, w.WarehouseName })
             .ToList();
                ViewBag.WarehouseList = warehouseList;
                var tog = new TogViewModel()
                {

                    Measurement = measurement,
                    Quantity = quantity,
                    UnitCost = unitCost,
                    TotalCost = totalCost,
                    Total = total,
                    ProductName = productName,
                    RestaurantId = restaurant.RestaurantId,
                    TogNO = GenerateAutomaticPurchaseOrderNo(),
                };
                tog.TNo = GenerateAutomaticTNo();
                ViewData["RestaurantId"] = restaurant.RestaurantId;
                return View(tog);
            }

            return RedirectToAction("Index", "Home");
        }
        // POST: Togs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, [Bind("TNo,TogNO,TogDate,WarehouseId,WarehouseName,ExpiryDate,ToWarehouseId,SupplierId,SupplierInvoiceNo,AdditionalInformation,ProductName,Measurement,Quantity,UnitCost,TotalCost,Total,Status")] List<string> productname, List<string> Measurement, List<decimal> Quantity, List<decimal> UnitCost, List<decimal> TotalCost, Tog model)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                if (ModelState.IsValid)
                {
                    model.RestaurantId = restaurant.RestaurantId;
                    _context.Add(model);
                    for (int i = 0; i < productname.Count; i++)
                    {
                        if (i == 0)
                            continue;
                        var purchaseorder = new Tog
                        {
                            TNo = model.TNo,
                            TogNO = model.TogNO,
                            TogDate = model.TogDate,
                            RestaurantId = restaurant.RestaurantId,
                            ToWarehouseId = model.ToWarehouseId,
                            ToWarehouse = model.ToWarehouse,
                            AdditionalInformation = model.AdditionalInformation,
                            Status = model.Status,
                            ProductName = i < productname.Count ? productname[i] : null,
                            Measurement = i < Measurement.Count ? Measurement[i] : null,
                            UnitCost = i < UnitCost.Count ? UnitCost[i] : 0,
                            Quantity = i < Quantity.Count ? Quantity[i] : 0,
                            TotalCost = i < TotalCost.Count ? TotalCost[i] : 0,
                            Total = model.Total,
                            WarehouseId = model.WarehouseId,
                            WarehouseName = model.WarehouseName
                        };
                        _context.Add(purchaseorder);
                    }
                    await _context.SaveChangesAsync();
                    UpdatePurchaseOrderStatus();
                    CreateOrUpdateGRN(model);
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }
        //Quantity add in Product Table 
        public IActionResult CreateOrUpdateGRN(Tog model)
        {
            try
            {
                var products = _context.Togs
                    .Where(p => p.TogNO == model.TogNO)
                    .ToList();

                foreach (var product in products)
                {
                    string productName = product.ProductName;
                    decimal grnQuantity = product.Quantity;
                    int warehouseId = model.WarehouseId;
                    int toWarehouseId = model.ToWarehouseId;

                    var existingProduct = _context.Products.SingleOrDefault(p => p.ProductName == productName && p.WarehouseId == warehouseId);

                    if (existingProduct != null)
                    {
                        existingProduct.Quantity -= grnQuantity;
                        if (existingProduct.Quantity < 0)
                        {
                            existingProduct.Quantity = 0;
                        }
                    }
                    var toWarehouseProduct = _context.Products.SingleOrDefault(p => p.ProductName == productName && p.WarehouseId == toWarehouseId);

                    if (toWarehouseProduct != null)
                    {
                        toWarehouseProduct.Quantity += grnQuantity;
                    }
                    else
                    {
                        var newProduct = new Product
                        {
                            ProductName = productName,
                            Quantity = grnQuantity,
                            WarehouseId = toWarehouseId
                        };
                        _context.Products.Add(newProduct);
                    }
                }
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        //Update PurchaseOrder status
        public IActionResult UpdatePurchaseOrderStatus()
        {
            var uniquePurchaseOrderNos = _context.PurchaseOrders
                .Select(po => po.PurchaseOrderNo)
                .Distinct()
                .ToList();
            foreach (var purchaseOrderNo in uniquePurchaseOrderNos)
            {
                var purchaseOrdersWithSameNo = _context.PurchaseOrders
                    .Where(po => po.PurchaseOrderNo == purchaseOrderNo)
                    .ToList();
                var uniqueProductNames = purchaseOrdersWithSameNo
                    .Select(po => po.ProductName)
                    .Distinct()
                    .ToList();
                bool allProductsReceived = true;
                foreach (var purchaseOrder in purchaseOrdersWithSameNo)
                {
                    decimal totalReceivedQuantity = _context.GRNS
                        .Where(grn => grn.PurchaseOrderNo == purchaseOrderNo && grn.ProductName == purchaseOrder.ProductName)
                        .Sum(grn => grn.Quantity);
                    if (totalReceivedQuantity < purchaseOrder.Quantity)
                    {
                        allProductsReceived = false;
                        break;
                    }
                }
                string status;
                if (allProductsReceived)
                {
                    status = "Completed";
                }
                else if (_context.GRNS.Any(grn => grn.PurchaseOrderNo == purchaseOrderNo))
                {
                    status = "Partially";
                }
                else
                {
                    status = "Pending";
                }
                foreach (var purchaseOrder in purchaseOrdersWithSameNo)
                {
                    purchaseOrder.Status = status;
                }
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}