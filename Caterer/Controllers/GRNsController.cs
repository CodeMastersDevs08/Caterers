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
    public class GRNsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public GRNsController(ApplicationDbContext context)
        {
            _context = context;
        }
        //Genreate GNO No
        private int GenerateAutomaticPNo()
        {
            var maxPNo = _context.GRNS.Max(po => (int?)po.GNo) ?? 0;
            return maxPNo + 1;
        }
        //Generate GRNNO No
        private string GenerateAutomaticPurchaseOrderNo()
        {
            var maxPurchaseOrderNo = _context.GRNS.Max(po => po.GRNNO);
            if (!string.IsNullOrEmpty(maxPurchaseOrderNo) && maxPurchaseOrderNo.StartsWith("GRN-"))
            {
                var numericPart = maxPurchaseOrderNo.Substring(4);
                if (int.TryParse(numericPart, out var maxNumber))
                {
                    var newNumber = (maxNumber + 1).ToString("D4");
                    return $"GRN-{newNumber}";
                }
            }
            return "GRN-1000";
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
                    expiredate = productDetails.ExpireDate
                };
                return Json(productData);
            }
            return Json(null);
        }
        // GET: GRN Index 
        public async Task<IActionResult> Index(int? warehouseId)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                var warehouses = _context.Warehouses
                  .Where(w => w.RestaurantId == restaurant.RestaurantId)
                  .ToList();

                IQueryable<GRN> productsQuery = _context.GRNS
                 .Where(p => p.RestaurantId == restaurant.RestaurantId)
                 .Include(p => p.Supplier)
                 .OrderByDescending(s => s.GrnId);

                ViewData["Warehouses"] = warehouses;

                if (!warehouseId.HasValue)
                {
                    var defaultWarehouse = warehouses.FirstOrDefault();
                    warehouseId = defaultWarehouse?.WarehouseId;
                }

                if (warehouseId.HasValue)
                {
                    productsQuery = productsQuery.Where(p => p.WarehouseId == warehouseId.Value);
                }

                var products = await productsQuery.ToListAsync();
                return View(products);
            }
            return Problem("Entity set 'ApplicationDbContext.Restaurants' is null or user is not associated with any restaurant.");
        }
        // GET: GRN Details 
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var purchaseOrder = await _context.GRNS
                .Where(p => p.GrnId == id)
                  .Include(a => a.Supplier)
                .ToListAsync();
            if (purchaseOrder == null)
            {
                return NotFound();
            }
            var purchaseOrderNo = purchaseOrder.First().GNo;
            var relatedPurchaseOrders = await _context.GRNS
                .Where(p => p.GNo == purchaseOrderNo)
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
            return View(purchaseOrder.First());
        }
        // GET: GRNs/Create
        public IActionResult Create(int supplierId, string productName, string measurement, decimal quantity, decimal unitCost, decimal totalCost, decimal total, string purchaseOrderNo, DateTime purchaseOrderDate, string paymentMethod)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                var suppliers = _context.Suppliers
                    .Where(s => s.RestaurantId == restaurant.RestaurantId)
                    .ToList();
                var purchaseorder = new GRNViewModel()
                {
                    SupplierName = suppliers,
                    Measurement = measurement,
                    Quantity = quantity,
                    UnitCost = unitCost,
                    TotalCost = totalCost,
                    Total = total,
                    PurchaseOrderNo = purchaseOrderNo,
                    PurchaseOrderDate = purchaseOrderDate,
                    ProductName = productName,
                    GRNNO = GenerateAutomaticPurchaseOrderNo(),
                    RestaurantId = restaurant.RestaurantId
                };
                purchaseorder.GNo = GenerateAutomaticPNo();
                ViewData["RestaurantId"] = restaurant.RestaurantId;
                return View(purchaseorder);
            }
            return RedirectToAction("Index", "Home");
        }
        // POST: GRNs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, [Bind("GrnId,GNo,GRNNO,GRNDate,GRNType,PurchaseOrderNo,WarehouseId, WarehouseName,ExpiryDate,PurchaseOrderDate,SupplierId,Paymentmethod,SupplierInvoiceNo,AdditionalInformation,ProductName,Measurement,Quantity,UnitCost,TotalCost,Total,Status")] List<string> productname, List<string> Measurement, List<decimal> Quantity, List<decimal> UnitCost, List<DateTime> expirydate, List<decimal> TotalCost, GRN model)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                if (ModelState.IsValid)
                {
                    model.RestaurantId = restaurant.RestaurantId;
                    _context.Add(model);
                    // Loop through the lists to create and save related entries
                    for (int i = 0; i < productname.Count; i++)
                    {
                        if (i == 0)
                            continue;
                        var purchaseorder = new GRN
                        {
                            GNo = model.GNo,
                            GRNNO = model.GRNNO,
                            GRNType = model.GRNType,
                            GRNDate = model.GRNDate,
                            RestaurantId = restaurant.RestaurantId,
                            SupplierId = model.SupplierId,
                            PurchaseOrderNo = model.PurchaseOrderNo,
                            PurchaseOrderDate = model.PurchaseOrderDate,
                            Paymentmethod = model.Paymentmethod,
                            AdditionalInformation = model.AdditionalInformation,
                            Status = model.Status,
                            ProductName = i < productname.Count ? productname[i] : null,
                            Measurement = i < Measurement.Count ? Measurement[i] : null,
                            UnitCost = i < UnitCost.Count ? UnitCost[i] : 0,
                            ExpiryDate = i < expirydate.Count ? expirydate[i] : DateTime.MinValue,
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
        //GET Product Details for GRN
        public List<GRNViewModel> GetProductDetailsByPurchaseOrderNo(string purchaseOrderNo)
        {
            var products = _context.PurchaseOrders
                .Where(po => po.PurchaseOrderNo == purchaseOrderNo)
                .Select(po => new GRNViewModel
                {
                    ProductName = po.ProductName,
                    Measurement = po.Measurement,
                    Quantity = po.Quantity,
                    UnitCost = po.UnitCost,
                    TotalCost = po.TotalCost,
                    WarehouseId = po.WarehouseId,
                    WarehouseName = po.WarehouseName
                })
                .ToList();
            return products;
        }
        // GET: GRNs/Create
        public IActionResult CreateGRN(int supplierId, int shopId, string purchaseOrderNo, DateTime purchaseOrderDate)
        {
            var supplier = _context.Suppliers.ToList();
            var productList = GetProductDetailsByPurchaseOrderNo(purchaseOrderNo);
            var purchaseorder = new GRNViewModel()
            {
                SupplierName = supplier,
                PurchaseOrderNo = purchaseOrderNo,
                PurchaseOrderDate = purchaseOrderDate,
                ProductDetails = productList,
                GRNNO = GenerateAutomaticPurchaseOrderNo()
            };
            purchaseorder.GNo = GenerateAutomaticPNo();
            return View(purchaseorder);
        }
        // POST: GRNs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGRN(int id, [Bind("GrnId,GNo,GRNNO,GRNDate,GRNType,PurchaseOrderNo,WarehouseId, WarehouseName,ExpiryDate,PurchaseOrderDate,SupplierId,Paymentmethod,SupplierInvoiceNo,AdditionalInformation,ProductName,Measurement,Quantity,UnitCost,TotalCost,Total,Status")] List<string> productname, List<string> Measurement, List<decimal> Quantity, List<decimal> UnitCost, List<DateTime> expirydate, List<decimal> TotalCost, GRN model)
        {
            if (ModelState.IsValid)
            {
                _context.Add(model);
                for (int i = 0; i < productname.Count; i++)
                {
                    if (i == 0)
                        continue;
                    var purchaseorder = new GRN
                    {
                        GNo = model.GNo,
                        GRNNO = model.GRNNO,
                        GRNDate = model.GRNDate,
                        SupplierId = model.SupplierId,
                        PurchaseOrderNo = model.PurchaseOrderNo,
                        PurchaseOrderDate = model.PurchaseOrderDate,
                        Paymentmethod = model.Paymentmethod,
                        AdditionalInformation = model.AdditionalInformation,
                        ProductName = i < productname.Count ? productname[i] : null,
                        Measurement = i < Measurement.Count ? Measurement[i] : null,
                        UnitCost = i < UnitCost.Count ? UnitCost[i] : 0,
                        ExpiryDate = i < expirydate.Count ? expirydate[i] : DateTime.MinValue,
                        Quantity = i < Quantity.Count ? Quantity[i] : 0,
                        TotalCost = i < TotalCost.Count ? TotalCost[i] : 0,
                        Total = model.Total,
                        WarehouseId = model.WarehouseId,
                        WarehouseName = model.WarehouseName
                    };
                    _context.Add(purchaseorder);
                    var product = _context.Products.FirstOrDefault(p => p.ProductName == productname[i]);
                    if (product != null)
                    {
                        product.Quantity += purchaseorder.Quantity;
                        _context.Update(product);
                    }
                }
                await _context.SaveChangesAsync();
                CreateOrUpdateGRN(model);
                UpdatePurchaseOrderStatus();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        //Quantity add in Product Table 
        public IActionResult CreateOrUpdateGRN(GRN model)
        {
            try
            {
                string productName = model.ProductName;
                decimal grnQuantity = model.Quantity;
                int warehouseId = model.WarehouseId;
                var existingProduct = _context.Products.SingleOrDefault(p => p.ProductName == productName && p.WarehouseId == warehouseId);
                if (existingProduct != null)
                {
                    existingProduct.Quantity += grnQuantity;
                }
                else
                {
                    var newProduct = new Product
                    {
                        ProductName = productName,
                        Quantity = grnQuantity,
                        WarehouseId = warehouseId
                    };
                    _context.Products.Add(newProduct);
                }
                _context.SaveChanges();
                var grnRecord = _context.GRNS.SingleOrDefault(g => g.ProductName == productName && g.WarehouseId == warehouseId && g.addonetime == 0);
                if (grnRecord != null)
                {
                    grnRecord.addonetime = 1;
                    _context.SaveChanges();
                }
                var mergeQuery = $@"
            MERGE INTO Products AS target
            USING (
                SELECT g.productname, g.warehouseId, SUM(g.quantity) AS total_quantity
                FROM grns g
                WHERE g.addonetime = 0 AND g.warehouseId = {warehouseId}
                GROUP BY g.productname, g.warehouseId
            ) AS source
            ON target.productname = source.productname AND target.warehouseId = source.warehouseId
            WHEN MATCHED THEN
                UPDATE SET target.quantity = target.quantity + source.total_quantity
            WHEN NOT MATCHED THEN
                INSERT (productname, quantity, warehouseId)
                VALUES (source.productname, source.total_quantity, source.warehouseId);

            -- Update addonetime in the grn table for processed rows
            UPDATE grns
            SET addonetime = 1
            WHERE addonetime = 0 AND warehouseId = {warehouseId};
        ";
                _context.Database.ExecuteSqlRaw(mergeQuery);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }
        //GET Missing Quantity for CreateUpdateGRN
        public List<GRNViewModel> GetMissingProductDetailsByPurchaseOrderNo(string purchaseOrderNo)
        {
            var purchaseOrderProducts = _context.PurchaseOrders
                .Where(po => po.PurchaseOrderNo == purchaseOrderNo)
                .Select(po => new GRNViewModel
                {
                    PurchaseOrderNo = po.PurchaseOrderNo,
                    ProductName = po.ProductName,
                    Measurement = po.Measurement,
                    Quantity = po.Quantity,
                    UnitCost = po.UnitCost,
                    TotalCost = po.TotalCost,
                    WarehouseId = po.WarehouseId,
                    WarehouseName = po.WarehouseName,
                })
                .ToList();
            var grnProducts = _context.GRNS
                .Where(grn => grn.PurchaseOrderNo == purchaseOrderNo)
                .GroupBy(grn => new { grn.ProductName, grn.PurchaseOrderNo })
                .Select(grnGroup => new
                {
                    PurchaseOrderNo = grnGroup.Key.PurchaseOrderNo,
                    ProductName = grnGroup.Key.ProductName,
                    ReceivedQuantity = grnGroup.Sum(grn => grn.Quantity)
                })
                .ToList();
            var missingProducts = purchaseOrderProducts
                .GroupJoin(grnProducts,
                    po => new { po.ProductName, po.PurchaseOrderNo },
                    grn => new { grn.ProductName, grn.PurchaseOrderNo },
                    (po, grnGroup) => new GRNViewModel
                    {
                        ProductName = po.ProductName,
                        Measurement = po.Measurement,
                        Quantity = po.Quantity - grnGroup.Sum(grn => grn.ReceivedQuantity),
                        UnitCost = po.UnitCost,
                        TotalCost = (po.Quantity - grnGroup.Sum(grn => grn.ReceivedQuantity)) * po.UnitCost,
                        WarehouseId = po.WarehouseId,
                        WarehouseName = po.WarehouseName
                    })
                .Where(missingProduct => missingProduct.Quantity > 0)
                .ToList();
            return missingProducts;
        }
        // GET: GRNs/Create
        public IActionResult CreateUpdateGRN(int supplierId, int shopId, string purchaseOrderNo, DateTime purchaseOrderDate)
        {
            var supplier = _context.Suppliers.ToList();
            var productList = GetMissingProductDetailsByPurchaseOrderNo(purchaseOrderNo);
            var purchaseorder = new GRNViewModel()
            {
                SupplierName = supplier,
                PurchaseOrderNo = purchaseOrderNo,
                PurchaseOrderDate = purchaseOrderDate,
                ProductDetails = productList,
                GRNNO = GenerateAutomaticPurchaseOrderNo()
            };
            purchaseorder.GNo = GenerateAutomaticPNo();
            return View(purchaseorder);
        }
        // POST: GRNs/CreateUpdateGRN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUpdateGRN(int id, [Bind("GrnId,GNo,GRNNO,GRNDate,GRNType,PurchaseOrderNo, WarehouseId, WarehouseName, ExpiryDate,PurchaseOrderDate,SupplierId,Paymentmethod,SupplierInvoiceNo,AdditionalInformation,ProductName,Measurement,Quantity,UnitCost,TotalCost,Total,Status")] List<string> productname, List<string> Measurement, List<decimal> Quantity, List<decimal> UnitCost, List<DateTime> expirydate, List<decimal> TotalCost, GRN model)
        {
            if (ModelState.IsValid)
            {
                _context.Add(model);
                for (int i = 0; i < productname.Count; i++)
                {
                    if (i == 0)
                        continue;
                    var purchaseorder = new GRN
                    {
                        GNo = model.GNo,
                        GRNNO = model.GRNNO,
                        GRNDate = model.GRNDate,
                        SupplierId = model.SupplierId,
                        PurchaseOrderNo = model.PurchaseOrderNo,
                        PurchaseOrderDate = model.PurchaseOrderDate,
                        Paymentmethod = model.Paymentmethod,
                        AdditionalInformation = model.AdditionalInformation,
                        ProductName = i < productname.Count ? productname[i] : null,
                        Measurement = i < Measurement.Count ? Measurement[i] : null,
                        UnitCost = i < UnitCost.Count ? UnitCost[i] : 0,
                        ExpiryDate = i < expirydate.Count ? expirydate[i] : DateTime.MinValue,
                        Quantity = i < Quantity.Count ? Quantity[i] : 0,
                        TotalCost = i < TotalCost.Count ? TotalCost[i] : 0,
                        Total = model.Total,
                        WarehouseId = model.WarehouseId,
                        WarehouseName = model.WarehouseName
                    };
                    _context.Add(purchaseorder);
                }
                var partiallyReceivedProducts = _context.PurchaseOrders
                    .Where(po => po.PurchaseOrderNo == model.PurchaseOrderNo && po.Status == "Partially")
                    .ToList();
                foreach (var partiallyReceivedProduct in partiallyReceivedProducts)
                {
                    var existingGRN = _context.GRNS.FirstOrDefault(g =>
                        g.PurchaseOrderNo == model.PurchaseOrderNo &&
                        g.ProductName == partiallyReceivedProduct.ProductName);
                    if (existingGRN == null)
                    {
                        var missingQuantity = partiallyReceivedProduct.Quantity - Quantity[partiallyReceivedProducts.IndexOf(partiallyReceivedProduct)];
                        if (missingQuantity > 0)
                        {
                            var grnForMissingQuantity = new GRN
                            {
                                GNo = model.GNo,
                                GRNNO = model.GRNNO,
                                GRNDate = model.GRNDate,
                                SupplierId = model.SupplierId,
                                PurchaseOrderNo = model.PurchaseOrderNo,
                                PurchaseOrderDate = model.PurchaseOrderDate,
                                Paymentmethod = model.Paymentmethod,
                                AdditionalInformation = model.AdditionalInformation,
                                Status = model.Status,
                                ProductName = partiallyReceivedProduct.ProductName,
                                Measurement = partiallyReceivedProduct.Measurement,
                                UnitCost = partiallyReceivedProduct.UnitCost,
                                Quantity = missingQuantity,
                                TotalCost = partiallyReceivedProduct.TotalCost,
                                Total = model.Total,
                                WarehouseId = partiallyReceivedProduct.WarehouseId,
                                WarehouseName = partiallyReceivedProduct.WarehouseName
                            };
                            _context.Add(grnForMissingQuantity);
                        }
                    }
                }
                await _context.SaveChangesAsync();
                UpdatePurchaseOrderStatus();
                UpdateGRN();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public IActionResult UpdateGRN()
        {
            try
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
                    var grnRecords = _context.GRNS
                        .Where(grn => grn.PurchaseOrderNo == purchaseOrderNo)
                        .ToList();
                    decimal totalReceivedQuantity = grnRecords.Sum(grn => grn.Quantity);
                    decimal totalOrderedQuantity = purchaseOrdersWithSameNo.Sum(po => po.Quantity);
                    decimal missingQuantity = totalOrderedQuantity - totalReceivedQuantity;

                    if (missingQuantity > 0)
                    {
                        var missingProduct = purchaseOrdersWithSameNo.FirstOrDefault();
                        if (missingProduct != null)
                        {
                            var grn = _context.GRNS.SingleOrDefault(g => g.PurchaseOrderNo == purchaseOrderNo && g.ProductName == missingProduct.ProductName && g.WarehouseId == missingProduct.WarehouseId);
                            if (grn == null)
                            {
                                grn = new GRN
                                {
                                    PurchaseOrderNo = purchaseOrderNo,
                                    ProductName = missingProduct.ProductName,
                                    Quantity = missingQuantity,
                                    WarehouseId = missingProduct.WarehouseId
                                };
                                _context.GRNS.Add(grn);
                            }
                            else
                            {
                                grn.Quantity += missingQuantity;
                            }
                        }
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
                // Determine the status based on whether all products have been received or exceeded
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
