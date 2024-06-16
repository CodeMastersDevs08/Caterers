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
using System.Text.RegularExpressions;
namespace Caterer.Controllers.InventoryManagement.ProductManager
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        IWebHostEnvironment hostingenvironment;
        public ProductsController(ApplicationDbContext context, IWebHostEnvironment hc)
        {
            _context = context;
            hostingenvironment = hc;
        }

        //Genarate Product Code
        private int GenerateProductCode()
        {
            var latestProduct = _context.Products
                .OrderByDescending(p => p.productcode)
                .FirstOrDefault();
            int nextCode = 1;
            if (latestProduct != null && latestProduct.productcode != null)
            {
                nextCode = latestProduct.productcode + 1;
            }
            return nextCode;
        }
        //Save Warehouse Data 
        [HttpPost]
        public JsonResult SaveWarehouse(Warehouse warehousemodel)
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
        //Save Units
        [HttpPost]
        public JsonResult SaveUnits(Measurement Measurementmodel)
        {
            bool measurementExists = _context.Measurements
                .Any(m => m.RestaurantId == Measurementmodel.RestaurantId && m.MeasurementName == Measurementmodel.MeasurementName);
            if (measurementExists)
            {
                return new JsonResult(new { success = false, message = "MeasurementName already exists." });
            }
            var Data = new Measurement()
            {
                RestaurantId = Measurementmodel.RestaurantId,
                MeasurementName = Measurementmodel.MeasurementName,
            };
            _context.Measurements.Add(Data);
            _context.SaveChanges();
            return new JsonResult(new { success = true, measurementId = Data.MeasurementId });
        }
        //Save Category 
        [HttpPost]
        public JsonResult SaveCategories(Category categoryModel)
        {
            try
            {
                bool categoryExists = _context.Categories
                    .Any(c => c.RestaurantId == categoryModel.RestaurantId && c.CategoryName == categoryModel.CategoryName);
                if (categoryExists)
                {
                    return Json(new { success = false, message = "CategoryName already exists." });
                }
                var data = new Category()
                {
                    RestaurantId = categoryModel.RestaurantId,
                    CategoryName = categoryModel.CategoryName,
                    CategoryLogo = categoryModel.CategoryLogo,
                };
                _context.Categories.Add(data);
                _context.SaveChanges();
                return Json(new { success = true, categoryId = data.CategoryId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while saving the category." });
            }
        }
        // Save Tax
        [HttpPost]
        public JsonResult SaveTax(Tax Taxmodel)
        {
            bool taxExists = _context.Taxes
                .Any(t => t.RestaurantId == Taxmodel.RestaurantId && t.TaxName == Taxmodel.TaxName);
            if (taxExists)
            {
                return new JsonResult(new { success = false, message = "TaxName already exists." });
            }
            var Data = new Tax()
            {
                RestaurantId = Taxmodel.RestaurantId,
                TaxNo = Taxmodel.TaxNo,
                TaxType = Taxmodel.TaxType,
                TaxName = Taxmodel.TaxName,
                TaxPercentage = Taxmodel.TaxPercentage
            };
            _context.Taxes.Add(Data);
            _context.SaveChanges();
            return new JsonResult(new { success = true, taxId = Data.TaxId });
        }
        // GET: Products
        public async Task<IActionResult> ProductList(int? warehouseId)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);

            if (restaurant != null)
            {
                // Fetch Warehouses related to the specific restaurant
                var warehouses = _context.Warehouses
                    .Where(w => w.RestaurantId == restaurant.RestaurantId)
                    .ToList();

                IQueryable<Product> productsQuery = _context.Products
                    .Where(p => p.RestaurantId == restaurant.RestaurantId)
                    .Include(p => p.Category)
                    .Include(p => p.Tax)
                    .Include(p => p.Measurement)
                    .OrderByDescending(s => s.ProductId);

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

                return View("~/Views/InventoryManagement/ProductManager/Products/ProductList.cshtml", products);
            }

            return Problem("Entity set 'ApplicationDbContext.Restaurants' is null or the user is not associated with any restaurant.");
        }
        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }
            var products = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (products == null)
            {
                return NotFound();
            }
            return View(products);
        }

        //Get Product Create
        public IActionResult ProductCreate()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants
                .FirstOrDefault(r => r.OwnerEmail == userEmail);

            if (restaurant != null)
            {
                var categories = _context.Categories
                    .Where(c => c.RestaurantId == restaurant.RestaurantId && c.CategoryName != null)
                    .ToList();

                var measurement = _context.Measurements
                    .Where(m => m.RestaurantId == restaurant.RestaurantId)
                    .ToList();

                var tax = _context.Taxes
                    .Where(t => t.RestaurantId == restaurant.RestaurantId)
                    .GroupBy(t => t.TaxNo)
                    .Select(group => group.FirstOrDefault())
                    .ToList();

                var warehouses = _context.Warehouses
                    .Where(w => w.RestaurantId == restaurant.RestaurantId)
                    .ToList();

                var productModel = new ProductViewModel()
                {
                    CategoryName = categories,
                    MeasurementName = measurement,
                    TaxList = tax,
                };
                productModel.productcode = GenerateProductCode();
                ViewData["RestaurantId"] = restaurant.RestaurantId;

                return View("~/Views/InventoryManagement/ProductManager/Products/ProductCreate.cshtml", productModel);
            }

            return RedirectToAction("Index", "Home");
        }

        //Post Product Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductCreate([Bind("ProductName,ProductType,ProductImage,WarehouseName, MeasurementId,CategoryId,productcode,TaxId,Quantity,Barcode,ProductDescription,UnitPrice,UnitCost,StockControl,ExpireDate,Instock,SafetyStock,OpeningStockDate,OpeningStock,Mrp,WarehouseId,CreatedBy")] ProductViewModel productModel, IFormFile ProductImage)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);
            if (restaurant == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var categories = _context.Categories
                .Where(c => c.RestaurantId == restaurant.RestaurantId && c.CategoryName != null)
                .ToList();
            var measurement = _context.Measurements
                .Where(m => m.RestaurantId == restaurant.RestaurantId)
                .ToList();
            var warehouses = _context.Warehouses
                .Where(w => w.RestaurantId == restaurant.RestaurantId)
                .ToList();
            var tax = _context.Taxes
      .Where(t => t.RestaurantId == restaurant.RestaurantId) 
      .GroupBy(t => t.TaxNo)
      .Select(group => group.FirstOrDefault())
      .ToList();

            string filename = "";
            if (ProductImage != null)
            {
                string uploadFolder = Path.Combine(hostingenvironment.WebRootPath, "Images/ProductImages");
                filename = Guid.NewGuid().ToString() + "_" + ProductImage.FileName;
                string filepath = Path.Combine(uploadFolder, filename);
                using (var fileStream = new FileStream(filepath, FileMode.Create))
                {
                    await ProductImage.CopyToAsync(fileStream);
                }
            }
            if (!Regex.IsMatch(productModel.ProductName, "^[A-Za-z0-9_-]+$"))
            {
                ModelState.AddModelError(nameof(ProductViewModel.ProductName), "Product name can only contain letters, numbers, underscores, and hyphens.");
            }
            // Check if the product name already exists for the specific RestaurantId in the database
            if (_context.Products.Any(p => p.ProductName == productModel.ProductName &&
                                        p.RestaurantId == restaurant.RestaurantId &&
                                        p.WarehouseId == productModel.WarehouseId))
            {
                ModelState.AddModelError(nameof(ProductViewModel.ProductName), "Product with this name already exists for the current restaurant and warehouse.");
            }
            // Check if the barcode already exists for the specific RestaurantId and WarehouseId in the database
            if (_context.Products.Any(p => p.Barcode == productModel.Barcode &&
                                            p.RestaurantId == restaurant.RestaurantId &&
                                            p.WarehouseId == productModel.WarehouseId))
            {
                ModelState.AddModelError(nameof(ProductViewModel.Barcode), "Product with this barcode already exists for the current restaurant and warehouse.");
            }
            if (ModelState.IsValid)
            {
                foreach (var warehouse in warehouses)
                {
                    var product = new Product
                    {

                        CreatedBy = warehouse.WarehouseId == productModel.WarehouseId,
                        RestaurantId = restaurant.RestaurantId,
                        WarehouseId = warehouse.WarehouseId,
                        WarehouseName = warehouse.WarehouseName,
                        ProductName = productModel.ProductName,
                        ProductType = productModel.ProductType,
                        MeasurementId = productModel.MeasurementId,
                        CategoryId = productModel.CategoryId,
                        productcode = GenerateProductCode(),
                        Barcode = productModel.Barcode,
                        ProductDescription = productModel.ProductDescription,
                        Quantity = productModel.WarehouseId == warehouse.WarehouseId ? productModel.Quantity.HasValue ? productModel.Quantity.Value : 0.00m : 0.00m,
                        UnitPrice = productModel.UnitPrice.HasValue ? productModel.UnitPrice.Value : 0.00m,
                        UnitCost = productModel.UnitCost.HasValue ? productModel.UnitCost.Value : 0.00m,
                        StockControl = productModel.StockControl,
                        ExpireDate = productModel.ExpireDate,
                        Instock = productModel.Instock,
                        SafetyStock = productModel.SafetyStock.HasValue ? productModel.SafetyStock.Value : 0.00m,
                        TaxId = productModel.TaxId,
                        OpeningStock = productModel.OpeningStock,
                        OpeningStockDate = productModel.OpeningStockDate,
                        Mrp = productModel.Mrp.HasValue ? productModel.Mrp.Value : 0.00m,
                        ProductImage = filename
                    };
                    _context.Add(product);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ProductList));
            }
            productModel.MeasurementName = measurement;
            productModel.CategoryName = categories;
            productModel.TaxList = tax;
            return View("~/Views/InventoryManagement/ProductManager/Products/ProductCreate.cshtml", productModel);
        }
        // GET: Products/Edit/5
        public async Task<IActionResult> ProductEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var products = await _context.Products.FindAsync(id);
            if (products == null)
            {
                return NotFound();
            }
            var categories = _context.Categories
                      .Where(c => c.RestaurantId == products.RestaurantId && c.CategoryName != null)
                      .ToList();
            var measurement = _context.Measurements
                      .Where(m => m.RestaurantId == products.RestaurantId)
                      .ToList();
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants
                .FirstOrDefault(r => r.OwnerEmail == userEmail);
            var serviceTaxDineIn = restaurant.ServiceTaxDineIn;
            ViewData["DefaultTaxText"] = $"Service Tax Dine In: {serviceTaxDineIn}%";
            ViewData["RestaurantId"] = restaurant.RestaurantId;
            var tax = _context.Taxes
     .Where(t => t.RestaurantId == restaurant.RestaurantId) // Filter taxes by RestaurantId
     .GroupBy(t => t.TaxNo)
     .Select(group => group.FirstOrDefault())
     .ToList();
            var productModel = new ProductViewModel
            {
                ProductId = products.ProductId,
                ProductName = products.ProductName,
                WarehouseId = products.WarehouseId,
                WarehouseName = products.WarehouseName,
                ProductType = products.ProductType,
                ProductImage = products.ProductImage,
                MeasurementId = products.MeasurementId,
                CategoryId = products.CategoryId,
                TaxId = products.TaxId,
                productcode = products.productcode,
                Barcode = products.Barcode,
                ProductDescription = products.ProductDescription,
                Quantity = products.Quantity,
                UnitPrice = products.UnitPrice,
                UnitCost = products.UnitCost,
                StockControl = products.StockControl,
                ExpireDate = products.ExpireDate,
                Instock = products.Instock,
                SafetyStock = products.SafetyStock,
                CategoryName = categories,
                MeasurementName = measurement,
                OpeningStock = products.OpeningStock,
                OpeningStockDate = products.OpeningStockDate,
                Mrp = products.Mrp,
                TaxList = tax,
            };
            return View("~/Views/InventoryManagement/ProductManager/Products/ProductEdit.cshtml", productModel);
        }
        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductEdit(int id, [Bind("ProductId,ProductName,ProductType,MeasurementId,WarehouseName, WarehouseId,CategoryId,productcode, TaxId, Quantity,Barcode,ProductDescription,UnitPrice,UnitCost,StockControl,ExpireDate,Instock,SafetyStock,OpeningStockDate,OpeningStock,Mrp")] ProductViewModel productModel, IFormFile Image)
        {
            if (id != productModel.ProductId)
            {
                return NotFound();
            }
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);
            ViewData["RestaurantId"] = restaurant.RestaurantId;
            var serviceTaxDineIn = restaurant.ServiceTaxDineIn;
            ViewData["DefaultTaxText"] = $"Service Tax Dine In: {serviceTaxDineIn}%";
            var categories = _context.Categories
                .Where(c => c.RestaurantId == restaurant.RestaurantId && c.CategoryName != null)
                .ToList();
            var measurement = _context.Measurements
                .Where(m => m.RestaurantId == restaurant.RestaurantId)
                .ToList();
            var tax = _context.Taxes
     .Where(t => t.RestaurantId == restaurant.RestaurantId) // Filter taxes by RestaurantId
     .GroupBy(t => t.TaxNo)
     .Select(group => group.FirstOrDefault())
     .ToList();
            if (!Regex.IsMatch(productModel.ProductName, "^[A-Za-z0-9_-]+$"))
            {
                ModelState.AddModelError(nameof(ProductViewModel.ProductName), "Product name can only contain letters, numbers, underscores, and hyphens.");
            }
            // Check if the product name already exists for the specific RestaurantId in the database
            if (_context.Products.Any(p => p.ProductName == productModel.ProductName &&
                                p.RestaurantId == restaurant.RestaurantId &&
                                p.WarehouseId == productModel.WarehouseId &&
                                p.ProductId != id))
            {
                ModelState.AddModelError(nameof(ProductViewModel.ProductName), "Product with this name already exists for the current restaurant and warehouse.");
            }

            // Check if the barcode already exists for the specific RestaurantId and WarehouseId in the database
            if (_context.Products.Any(p => p.Barcode == productModel.Barcode &&
                                            p.RestaurantId == restaurant.RestaurantId &&
                                            p.WarehouseId == productModel.WarehouseId &&
                                            p.ProductId != id))
            {
                ModelState.AddModelError(nameof(ProductViewModel.Barcode), "Product with this barcode already exists for the current restaurant and warehouse.");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _context.Products.FindAsync(id);
                    if (Image != null)
                    {
                        if (!string.IsNullOrEmpty(existingProduct.ProductImage))
                        {
                            string existingFilePath = Path.Combine(hostingenvironment.WebRootPath, "Images/ProductImages", existingProduct.ProductImage);
                            if (System.IO.File.Exists(existingFilePath))
                            {
                                System.IO.File.Delete(existingFilePath);
                            }
                        }
                        string filename = Guid.NewGuid().ToString() + "_" + Image.FileName;
                        string uploadFolder = Path.Combine(hostingenvironment.WebRootPath, "Images/ProductImages");
                        string filepath = Path.Combine(uploadFolder, filename);
                        using (var fileStream = new FileStream(filepath, FileMode.Create))
                        {
                            await Image.CopyToAsync(fileStream);
                        }
                        existingProduct.ProductImage = filename;
                    }
                    existingProduct.ProductId = productModel.ProductId;
                    existingProduct.ProductName = productModel.ProductName;
                    existingProduct.WarehouseId = productModel.WarehouseId;
                    existingProduct.WarehouseName = productModel.WarehouseName;
                    existingProduct.ProductType = productModel.ProductType;
                    existingProduct.MeasurementId = productModel.MeasurementId;
                    existingProduct.CategoryId = productModel.CategoryId;
                    existingProduct.productcode = productModel.productcode;
                    existingProduct.Barcode = productModel.Barcode;
                    existingProduct.ProductDescription = productModel.ProductDescription;
                    existingProduct.Quantity = productModel.Quantity;
                    existingProduct.UnitPrice = productModel.UnitPrice;
                    existingProduct.UnitCost = productModel.UnitCost;
                    existingProduct.StockControl = productModel.StockControl;
                    existingProduct.ExpireDate = productModel.ExpireDate;
                    existingProduct.Instock = productModel.Instock;
                    existingProduct.SafetyStock = productModel.SafetyStock;
                    existingProduct.OpeningStock = productModel.OpeningStock;
                    existingProduct.OpeningStockDate = productModel.OpeningStockDate;
                    existingProduct.Mrp = productModel.Mrp;
                    existingProduct.TaxId = productModel.TaxId;
                    _context.Update(existingProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductsExists(productModel.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ProductList));
            }
            productModel.MeasurementName = measurement;
            productModel.CategoryName = categories;
            productModel.TaxList = tax;
            return View("~/Views/InventoryManagement/ProductManager/Products/ProductEdit.cshtml", productModel);
        }
        // GET: Categories/Delete/5
        public async Task<IActionResult> ProductDelete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            return View("~/Views/InventoryManagement/ProductManager/Products/ProductDelete.cshtml", product);
        }
        // POST: Categories/Delete/5
        [HttpPost, ActionName("ProductDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Category' is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(product.ProductImage))
            {
                string filePath = Path.Combine(hostingenvironment.WebRootPath, "Images/ProductImages", product.ProductImage);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ProductList));
        }
        public IActionResult StockAlert()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.OwnerEmail == userEmail);

            if (restaurant != null)
            {
                var productsWithLowStock = _context.Products
                    .Where(p => p.Quantity < p.SafetyStock && p.RestaurantId == restaurant.RestaurantId)
                    .Include(p => p.Category)
                    .Include(p => p.Tax)
                    .Include(p => p.Measurement)
                    .ToList();
                return View("~/Views/InventoryManagement/ProductManager/Products/StockAlert.cshtml", productsWithLowStock);
            }
            else
            {
                return NotFound("User is not associated with any restaurant.");
            }
        }
        private bool ProductsExists(int id)
        {
            return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}