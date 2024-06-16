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
using Microsoft.AspNetCore.Http;
using Caterer.Data.Migrations;
using Microsoft.CodeAnalysis;
namespace Caterer.Controllers.InventoryManagement
{
    public class PurchaseOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PurchaseOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: PurchaseOrder
        public async Task<IActionResult> PurchaseOrderList(int? warehouseId)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);

            if (restaurant != null)
            {
                var warehouses = _context.Warehouses
                    .Where(w => w.RestaurantId == restaurant.RestaurantId)
                    .ToList();

                IQueryable<PurchaseOrder> purchaseOrders = _context.PurchaseOrders
                  .Where(p => p.RestaurantId == restaurant.RestaurantId)
                  .Include(p => p.Supplier)
                  .OrderByDescending(s => s.PurchaseOrderId);

                ViewData["Warehouses"] = warehouses;

                if (!warehouseId.HasValue)
                {
                    var defaultWarehouse = warehouses.FirstOrDefault();
                    warehouseId = defaultWarehouse?.WarehouseId;
                }
                if (warehouseId.HasValue)
                {
                    purchaseOrders = purchaseOrders.Where(p => p.WarehouseId == warehouseId.Value);
                }
                var products = await purchaseOrders.ToListAsync();
                return View("~/Views/InventoryManagement/PurchaseOrders/PurchaseOrderList.cshtml", products);
            }
            return Problem("Entity set 'ApplicationDbContext.Restaurants' is null or user is not associated with any restaurant.");
        }
        //Supplier Popup Create
        [HttpPost]
        public JsonResult SaveSupplier([Bind("SupplierId,SupplierName,RestaurantId,WarehouseId,Email,PhoneNumber,Status,OpeningBalance,CreditPeriod,CreditLimit,ShippingAddress,BillingAddress")] Supplier Suppliermodel)
        {
            var supplier = new Supplier
            {
                RestaurantId = Suppliermodel.RestaurantId,
                WarehouseId = Suppliermodel.WarehouseId,
                SupplierName = Suppliermodel.SupplierName,
                PhoneNumber = Suppliermodel.PhoneNumber,
                Email = Suppliermodel.Email,
                Status = Suppliermodel.Status,
                OpeningBalance = Suppliermodel.OpeningBalance,
                CreditPeriod = Suppliermodel.CreditPeriod,
                CreditLimit = Suppliermodel.CreditLimit,
                BillingAddress = Suppliermodel.BillingAddress,
                ShippingAddress = Suppliermodel.ShippingAddress
            };
            _context.Suppliers.Add(supplier);
            _context.SaveChanges();
            return Json(supplier.SupplierId);
        }
        // GET: PurchaseOrder/Details/5
        public async Task<IActionResult> PurchaseOrderDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var purchaseOrder = await _context.PurchaseOrders
                .Where(p => p.PurchaseOrderId == id)
                .Include(a => a.Supplier)
                .FirstOrDefaultAsync();
            if (purchaseOrder == null)
            {
                return NotFound();
            }
            var purchaseOrderNo = purchaseOrder.PurchaseOrderNo;
            var relatedPurchaseOrders = await _context.PurchaseOrders
                .Where(p => p.PurchaseOrderNo == purchaseOrderNo)
                .ToListAsync();
            var relatedGRNs = await _context.GRNS
                .Where(g => g.PurchaseOrderNo == purchaseOrderNo)
                .ToListAsync();
            var query = from po in _context.PurchaseOrders
                        join supplier in _context.Suppliers on po.SupplierId equals supplier.SupplierId
                        select new Supplier
                        {
                            SupplierId = supplier.SupplierId,
                            SupplierName = supplier.SupplierName,
                            ShippingAddress = supplier.ShippingAddress,
                            PhoneNumber = supplier.PhoneNumber,
                            Email = supplier.Email,
                        };
            var purchaseOrders = query.ToList();
            ViewData["RelatedPurchaseOrders"] = relatedPurchaseOrders;
            ViewData["RelatedGRNs"] = relatedGRNs;
            ViewData["RelatedPurchaseOrder"] = purchaseOrder;
            return View("~/Views/InventoryManagement/PurchaseOrders/PurchaseOrderDetails.cshtml", purchaseOrder);
        }
        //PNo Number 
        private int GenerateAutomaticPNo()
        {
            var maxPNo = _context.PurchaseOrders.Max(po => (int?)po.PNo) ?? 0;
            return maxPNo + 1;
        }
        //Po Order No
        private string GenerateAutomaticPurchaseOrderNo()
        {
            var maxPurchaseOrderNo = _context.PurchaseOrders.Max(po => po.PurchaseOrderNo);
            if (!string.IsNullOrEmpty(maxPurchaseOrderNo) && maxPurchaseOrderNo.StartsWith("PO-"))
            {
                var numericPart = maxPurchaseOrderNo.Substring(3);
                if (int.TryParse(numericPart, out var maxNumber))
                {
                    var newNumber = (maxNumber + 1).ToString("D4");
                    return $"PO-{newNumber}";
                }
            }
            return "PO-1000";
        }
        //ProductName Search
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
        //GET Product Details
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
                    inStock = productDetails.Instock,
                    unitCost = productDetails.UnitCost,

                };
                return Json(productData);
            }
            return Json(null);
        }
        // GET: PurchaseOrder/Create
        public IActionResult PurchaseOrderCreate()
        {
            ViewBag.Warehouses = new SelectList(_context.Warehouses, "WarehouseId", "WarehouseName");
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants
                .FirstOrDefault(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                var suppliers = _context.Suppliers
                    .Where(s => s.RestaurantId == restaurant.RestaurantId)
                 .Select(w => new SelectListItem
                 {
                     Value = w.SupplierId.ToString(),
                     Text = w.SupplierName
                 })
                   .ToList();
                ViewData["Suppliers"] = suppliers;
                var warehouses = _context.Warehouses
                  .Where(w => w.RestaurantId == restaurant.RestaurantId)
                  .Select(w => new SelectListItem
                  {
                      Value = w.WarehouseId.ToString(),
                      Text = w.WarehouseName
                  })
                  .ToList();
                ViewData["Warehouses"] = warehouses;
                var purchaseorder = new PurchaseViewModel()
                {
                    PurchaseOrderNo = GenerateAutomaticPurchaseOrderNo(),
                    RestaurantId = restaurant.RestaurantId
                };
                purchaseorder.PNo = GenerateAutomaticPNo();
                ViewData["RestaurantId"] = restaurant.RestaurantId;
                return View("~/Views/InventoryManagement/PurchaseOrders/PurchaseOrderCreate.cshtml", purchaseorder);
            }
            return RedirectToAction("Index", "Home");
        }
        //Purchase Order Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PurchaseOrderCreate(int id, [Bind("PNo,SupplierId,PurchaseOrderDate,ExpectedDate, WarehouseId, WarehouseName, Paymentmethod,DeliveryInstruction,ProductName,Measurement,InStock,Quantity,UnitCost,TotalCost,Total,PurchaseOrderNo,Status")] List<string> productname, List<string> Measurement, List<int> InStock, List<decimal> Quantity, List<decimal> UnitCost, List<decimal> TotalCost, PurchaseOrder model)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);
            if (restaurant == null)
            {
                return RedirectToAction("Index", "Home");
            }
            model.RestaurantId = restaurant.RestaurantId;

            if (ModelState.IsValid)
            {
                _context.Add(model);

                for (int i = 0; i < productname.Count; i++)
                {
                    if (i == 0)
                        continue;
                    var purchaseorder = new PurchaseOrder
                    {
                        PNo = model.PNo,
                        SupplierId = model.SupplierId,
                        PurchaseOrderDate = model.PurchaseOrderDate,
                        ExpectedDate = model.ExpectedDate,
                        Paymentmethod = model.Paymentmethod,
                        DeliveryInstruction = model.DeliveryInstruction,
                        PurchaseOrderNo = model.PurchaseOrderNo,
                        Status = model.Status,
                        ProductName = productname[i],
                        Measurement = Measurement[i],
                        Quantity = Quantity[i],
                        InStock = InStock[i],
                        UnitCost = UnitCost[i],
                        RestaurantId = restaurant.RestaurantId,
                        TotalCost = TotalCost[i],
                        WarehouseId = model.WarehouseId,
                        WarehouseName = model.WarehouseName,
                        Total = model.Total
                    };
                    _context.Add(purchaseorder);
                }
                await _context.SaveChangesAsync();
                UpdatePurchaseOrderStatus();
                return RedirectToAction("PurchaseOrderList");
            }
            return View("~/Views/InventoryManagement/PurchaseOrders/PurchaseOrderCreate.cshtml", model);
        }


        //Get Purchase Order Edit
        public async Task<IActionResult> PurchaseOrderEdit(int? id)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.OwnerEmail == userEmail);
            ViewBag.Suppliers = new SelectList(_context.Suppliers, "SupplierId", "SupplierName");
            if (id == null)
            {
                return NotFound();
            }
            var menuRecipe = await _context.PurchaseOrders.FindAsync(id);
            if (menuRecipe == null)
            {
                return NotFound();
            }
            var relatedPurchaseOrders = await _context.PurchaseOrders
                .Where(p => p.PNo == menuRecipe.PNo)
                .ToListAsync();
            ViewData["RelatedPurchaseOrders"] = relatedPurchaseOrders;
            return View("~/Views/InventoryManagement/PurchaseOrders/PurchaseOrderEdit.cshtml", menuRecipe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PurchaseOrderEdit(int id, [Bind("PNo,SupplierId,PurchaseOrderDate,ExpectedDate,WarehouseId,WarehouseName,Paymentmethod,DeliveryInstruction,ProductName,Measurement,InStock,Quantity,UnitCost,TotalCost,Total,PurchaseOrderNo,Status")] List<string> productname, List<string> Measurement, List<int> InStock, List<decimal> Quantity, List<decimal> UnitCost, List<decimal> TotalCost, PurchaseOrder model)
        {
            if (id != model.PurchaseOrderId)
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
                    var relatedPurchaseOrders = await _context.PurchaseOrders
                        .Where(p => p.PNo == model.PNo)
                        .ToListAsync();
                    for (int i = 0; i < productname.Count; i++)
                    {
                        var purchaseOrder = relatedPurchaseOrders.ElementAtOrDefault(i) ?? new PurchaseOrder();
                        purchaseOrder.PNo = model.PNo;
                        purchaseOrder.RestaurantId = model.RestaurantId;
                        purchaseOrder.SupplierId = model.SupplierId;
                        purchaseOrder.PurchaseOrderDate = model.PurchaseOrderDate;
                        purchaseOrder.ExpectedDate = model.ExpectedDate;
                        purchaseOrder.Paymentmethod = model.Paymentmethod;
                        purchaseOrder.DeliveryInstruction = model.DeliveryInstruction;
                        purchaseOrder.PurchaseOrderNo = model.PurchaseOrderNo;
                        purchaseOrder.Status = model.Status;
                        purchaseOrder.ProductName = i < productname.Count ? productname[i] : null;
                        purchaseOrder.Measurement = i < Measurement.Count ? Measurement[i] : null;
                        purchaseOrder.Quantity = Convert.ToDecimal(Quantity[i]);
                        purchaseOrder.InStock = Convert.ToDecimal(InStock[i]);
                        purchaseOrder.UnitCost = Convert.ToDecimal(UnitCost[i]);
                        purchaseOrder.TotalCost = Convert.ToDecimal(TotalCost[i]);
                        purchaseOrder.WarehouseId = model.WarehouseId;
                        purchaseOrder.WarehouseName = model.WarehouseName;
                        purchaseOrder.Total = model.Total;
                        if (purchaseOrder.PurchaseOrderId == 0)
                        {
                            _context.Add(purchaseOrder);
                        }
                        else
                        {
                            _context.Update(purchaseOrder);
                        }
                    }
                    var purchaseOrdersToDelete = relatedPurchaseOrders.Where(p => !productname.Contains(p.ProductName)).ToList();
                    foreach (var purchaseOrderToDelete in purchaseOrdersToDelete)
                    {
                        _context.PurchaseOrders.Remove(purchaseOrderToDelete);
                    }
                    await _context.SaveChangesAsync();
                    ViewBag.Suppliers = new SelectList(_context.Suppliers, "SupplierId", "SupplierName");
                    ViewData["RelatedPurchaseOrders"] = relatedPurchaseOrders;
                    return RedirectToAction("PurchaseOrderList");
                }
                catch (DbUpdateConcurrencyException)
                {
                }
            }
            return View("~/Views/InventoryManagement/PurchaseOrders/PurchaseOrderList.cshtml", model);
        }
        //Update PurchaseOrder Status
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

            return RedirectToAction("PurchaseOrderList");
        }
        private bool PurchaseOrderExists(int id)
        {
            return _context.PurchaseOrders.Any(e => e.PurchaseOrderId == id);
        }

    }
}