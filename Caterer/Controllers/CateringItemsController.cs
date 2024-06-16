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
    public class CateringItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public CateringItemsController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }
        //Save Categories
        [HttpPost]
        public JsonResult SaveCategories(CateringCategory Measurementmodel)
        {
            if (_context.CateringCategories.Any(c => c.CateringCategoryName == Measurementmodel.CateringCategoryName && c.RestaurantId == Measurementmodel.RestaurantId))
            {
                return new JsonResult("MenuCategoryName for this RestaurantId already exists. Please choose a different name.");
            }
            var Data = new CateringCategory()
            {
                RestaurantId = Measurementmodel.RestaurantId,
                CateringCategoryName = Measurementmodel.CateringCategoryName,
                CategoryPrice = Measurementmodel.CategoryPrice,
            };
            _context.CateringCategories.Add(Data);
            _context.SaveChanges();
            return new JsonResult("Category Saved Successfully....!");
        }
        //Get Edit Categories
        public JsonResult EditCategories(int id)
        {
            var data = _context.CateringCategories.Where(ev => ev.CateringCategoryId == id).SingleOrDefault();
            return new JsonResult(data);
        }
        //Post Edit Category
        [HttpPost]
        public JsonResult EditCateringCategory(CateringCategory cateringCategory)
        {
            var existingCategory = _context.CateringCategories.Find(cateringCategory.CateringCategoryId);
            if (existingCategory == null)
            {
                return new JsonResult("Catering Category not found.");
            }
            existingCategory.CateringCategoryName = cateringCategory.CateringCategoryName;
            existingCategory.CategoryPrice = cateringCategory.CategoryPrice;
            _context.SaveChanges();
            return new JsonResult("Catering Category Edited Successfully....!");
        }
        //Post Delete Catering Category
        [HttpPost]
        public JsonResult DeleteCateringCategory(int id)
        {
            var categoryToDelete = _context.CateringCategories.Find(id);
            if (categoryToDelete == null)
            {
                return new JsonResult("Catering Category not found.");
            }
            _context.CateringCategories.Remove(categoryToDelete);
            _context.SaveChanges();
            return new JsonResult("Catering Category Deleted Successfully....!");
        }
        // GET: CateringItems
        public async Task<IActionResult> Index()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                var cateringCategories = await _context.CateringCategories
                    .Where(cc => cc.RestaurantId == restaurant.RestaurantId)
                    .Include(cc => cc.CateringItems)
                    .ThenInclude(ci => ci.MenuCategory)   
                    .ToListAsync();
                ViewData["RestaurantId"] = restaurant.RestaurantId;
                return View(cateringCategories);
            }
            return View(new List<CateringCategory>());
        }
      //Get Item Name
        [HttpGet]
        public IActionResult GetItemNames(int menuCategoryId)
        {
            var itemNames = _context.MenuItems
                .Where(m => m.MenuCategoryId == menuCategoryId)
                .Select(m => new { value = m.MenuItemId, text = m.ItemName, })
                .ToList();
            return Json(itemNames);
        }
      //Get Menu Category Details
        [HttpGet]
        public IActionResult GetMenuCategoryDetails(int menuCategoryId)
        {
            var menuCategory = _context.MenuCategories
                .Where(m => m.MenuCategoryId == menuCategoryId)
                .FirstOrDefault();
            return Json(new
            {
                menuCategoryName = menuCategory?.MenuCategoryName,
                selectedItem = menuCategory?.SelectedItem
            });
        }
        //Get Item Details
        [HttpGet]
        public IActionResult GetItemDetails(int itemId)
        {
            var itemDetails = _context.MenuItems
                .Where(m => m.MenuItemId == itemId)
                .Select(m => new { itemPrice = m.ItemPrice, itemImage = m.ItemImage, itemName = m.ItemName })
                .FirstOrDefault();

            return Json(itemDetails);
        }
        // GET: CateringItems/Create
        public IActionResult Create(int? categoryId)
        {            
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;       
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {             
                ViewBag.SelectedCategoryId = categoryId;            
                ViewBag.RestaurantId = restaurant.RestaurantId;             
                var cateringCategories = _context.CateringCategories
                    .Where(c => c.RestaurantId == restaurant.RestaurantId)
                    .ToList();            
                ViewBag.MenuCategories = new SelectList(
                    _context.MenuCategories
                        .Where(mc => mc.RestaurantId == restaurant.RestaurantId)
                        .ToList(),
                    "MenuCategoryId",
                    "MenuCategoryName"
                );
                ViewBag.CateringCategories = new SelectList(cateringCategories, "CateringCategoryId", "CateringCategoryName");
                return View();
            }         
            return RedirectToAction("Index", "Home"); 
        }
        //Post Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CateringItemViewModel cateringItemViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var cateringItem = new CateringItem
                    {
                        ItemName = cateringItemViewModel.ItemName,
                        RestaurantId = cateringItemViewModel.RestaurantId,
                        MenuItemId = cateringItemViewModel.MenuItemId,
                        MenuCategoryId = cateringItemViewModel.MenuCategoryId,
                        MenuCategoryName = cateringItemViewModel.MenuCategoryName,
                        SelectedItem = cateringItemViewModel.SelectedItem,
                        CateringCategoryId = cateringItemViewModel.CateringCategoryId,
                        CateringExtrasId = cateringItemViewModel.CateringExtrasId,
                        ItemDescription = cateringItemViewModel.ItemDescription,
                        ItemPrice = cateringItemViewModel.ItemPrice,
                        DineInPrice = cateringItemViewModel.DineInPrice,
                        DiscountedPrice = cateringItemViewModel.DiscountedPrice,
                        VATPercentage = cateringItemViewModel.VATPercentage,
                        ItemAvailable = cateringItemViewModel.ItemAvailable,
                        EnableVariants = cateringItemViewModel.EnableVariants,
                        EnableAlwaysAvailable = cateringItemViewModel.EnableAlwaysAvailable,
                        Monday = cateringItemViewModel.Monday,
                        MondayStart = cateringItemViewModel.MondayStart,
                        MondayEnd = cateringItemViewModel.MondayEnd,
                        Tuesday = cateringItemViewModel.Tuesday,
                        TuesdayStart = cateringItemViewModel.TuesdayStart,
                        TuesdayEnd = cateringItemViewModel.TuesdayEnd,
                        Wednesday = cateringItemViewModel.Wednesday,
                        WednesdayStart = cateringItemViewModel.WednesdayStart,
                        WednesdayEnd = cateringItemViewModel.WednesdayEnd,
                        Thursday = cateringItemViewModel.Thursday,
                        ThursdayStart = cateringItemViewModel.ThursdayStart,
                        ThursdayEnd = cateringItemViewModel.ThursdayEnd,
                        Friday = cateringItemViewModel.Friday,
                        FridayStart = cateringItemViewModel.FridayStart,
                        FridayEnd = cateringItemViewModel.FridayEnd,
                        Saturday = cateringItemViewModel.Saturday,
                        SaturdayStart = cateringItemViewModel.SaturdayStart,
                        SaturdayEnd = cateringItemViewModel.SaturdayEnd,
                        Sunday = cateringItemViewModel.Sunday,
                        SundayStart = cateringItemViewModel.SundayStart,
                        SundayEnd = cateringItemViewModel.SundayEnd,
                        ItemImage = cateringItemViewModel.ProductImage 
                    };
                    _context.Add(cateringItem);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {               
                return Problem("An error occurred while processing your request. Please try again later.");
            }
            ViewBag.CateringCategories = new SelectList(_context.CateringCategories, "CateringCategoryId", "CateringCategoryName");
            return View(cateringItemViewModel);
        }
        // GET: CateringItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var cateringItem = await _context.CateringItems
                .Include(ci => ci.ExtraList) 
                .FirstOrDefaultAsync(ci => ci.CateringItemId == id);
            if (cateringItem == null)
            {
                return NotFound();
            }          
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;         
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {             
                ViewBag.RestaurantId = restaurant.RestaurantId;               
                ViewBag.CateringCategories = new SelectList(
                    _context.CateringCategories
                        .Where(cc => cc.RestaurantId == restaurant.RestaurantId)
                        .ToList(),
                    "CateringCategoryId",
                    "CateringCategoryName"
                );               
                ViewBag.SubCategories = new SelectList(
                    _context.CateringItems
                        .Where(ci => ci.RestaurantId == restaurant.RestaurantId)
                        .Select(ci => new SelectListItem
                        {
                            Value = ci.MenuCategoryId.ToString(), // Assuming MenuCategoryId is an int
                            Text = ci.MenuCategoryName
                        })
                        .Distinct()
                        .ToList(),
                    "Value",
                    "Text"
                );
            }
            return View(cateringItem);
        }     
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Edit(int id, CateringItemViewModel cateringItemViewModel, IFormFile productImage)
        {
            try
            {
                string filename = "";
                if (productImage != null)
                {
                    string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Images/Menuitems");
                    filename = Guid.NewGuid().ToString() + "_" + productImage.FileName;
                    string filepath = Path.Combine(uploadFolder, filename);
                    using (var fileStream = new FileStream(filepath, FileMode.Create))
                    {
                        await productImage.CopyToAsync(fileStream);
                    }
                }
                if (ModelState.IsValid)
                {
                    var cateringItem = await _context.CateringItems
                        .Include(ci => ci.MenuCategory)
                        .FirstOrDefaultAsync(ci => ci.CateringItemId == id);
                    if (cateringItem == null)
                    {
                        return NotFound();
                    }               
                    var menuItem = await _context.MenuItems
                        .FirstOrDefaultAsync(mi => mi.MenuItemId == cateringItem.MenuItemId);
                    if (menuItem == null)
                    {
                        return NotFound("MenuItem not found for the given CateringItem");
                    }                
                    menuItem.ItemName = cateringItemViewModel.ItemName;
                    _context.Update(menuItem);                
                    UpdateCateringItemProperties(cateringItem, cateringItemViewModel, filename);               
                    menuItem.MenuCategoryId = cateringItemViewModel.MenuCategoryId;
                    _context.Update(menuItem);               
                    var menuCategory = await _context.MenuCategories
                        .FirstOrDefaultAsync(mc => mc.MenuCategoryId == cateringItemViewModel.MenuCategoryId);
                    if (menuCategory != null)
                    {
                        menuCategory.MenuCategoryName = cateringItemViewModel.MenuCategoryName;
                        _context.Update(menuCategory);
                    }
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                return Problem("An error occurred while processing your request. Please try again later.");
            }
            ViewBag.CateringCategories = new SelectList(_context.CateringCategories, "CateringCategoryId", "CateringCategoryName");
            return View(cateringItemViewModel);
        }
        //Update Catering item properties
        private void UpdateCateringItemProperties(CateringItem cateringItem, CateringItemViewModel viewModel, string filename)
        {
            cateringItem.ItemName = viewModel.ItemName;
            cateringItem.RestaurantId = viewModel.RestaurantId;
            cateringItem.MenuItemId = viewModel.MenuItemId;
            cateringItem.MenuCategoryId = viewModel.MenuCategoryId;
            cateringItem.MenuCategoryName = viewModel.MenuCategoryName;
            cateringItem.SelectedItem = viewModel.SelectedItem;
            cateringItem.CateringCategoryId = viewModel.CateringCategoryId;
            cateringItem.CateringExtrasId = viewModel.CateringExtrasId;
            cateringItem.ItemDescription = viewModel.ItemDescription;
            cateringItem.ItemPrice = viewModel.ItemPrice;
            cateringItem.DineInPrice = viewModel.DineInPrice;
            cateringItem.DiscountedPrice = viewModel.DiscountedPrice;
            cateringItem.VATPercentage = viewModel.VATPercentage;
            cateringItem.ItemAvailable = viewModel.ItemAvailable;
            cateringItem.EnableVariants = viewModel.EnableVariants;
            cateringItem.EnableAlwaysAvailable = viewModel.EnableAlwaysAvailable;
            cateringItem.Monday = viewModel.Monday;
            cateringItem.MondayStart = viewModel.MondayStart;
            cateringItem.MondayEnd = viewModel.MondayEnd;
            cateringItem.Tuesday = viewModel.Tuesday;
            cateringItem.TuesdayStart = viewModel.TuesdayStart;
            cateringItem.TuesdayEnd = viewModel.TuesdayEnd;
            cateringItem.Wednesday = viewModel.Wednesday;
            cateringItem.WednesdayStart = viewModel.WednesdayStart;
            cateringItem.WednesdayEnd = viewModel.WednesdayEnd;
            cateringItem.Thursday = viewModel.Thursday;
            cateringItem.ThursdayStart = viewModel.ThursdayStart;
            cateringItem.ThursdayEnd = viewModel.ThursdayEnd;
            cateringItem.Friday = viewModel.Friday;
            cateringItem.FridayStart = viewModel.FridayStart;
            cateringItem.FridayEnd = viewModel.FridayEnd;
            cateringItem.Saturday = viewModel.Saturday;
            cateringItem.SaturdayStart = viewModel.SaturdayStart;
            cateringItem.SaturdayEnd = viewModel.SaturdayEnd;
            cateringItem.Sunday = viewModel.Sunday;
            cateringItem.SundayStart = viewModel.SundayStart;
            cateringItem.SundayEnd = viewModel.SundayEnd;
            if (!string.IsNullOrEmpty(filename))
            {
                cateringItem.ItemImage = filename;
            }
        }

        //Delete Edit Row
        [HttpPost]
        public async Task<IActionResult> DeleteEditRow(int id)
        {
            var Recipe = await _context.CateringItems.FindAsync(id);
            if (Recipe == null)
            {
                return NotFound();
            }
            _context.CateringItems.Remove(Recipe);
            await _context.SaveChangesAsync();
            return Ok();
        }
        // GET: CateringItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CateringItems == null)
            {
                return NotFound();
            }
            var cateringItem = await _context.CateringItems
                .FirstOrDefaultAsync(m => m.CateringItemId == id);
            if (cateringItem == null)
            {
                return NotFound();
            }
            return View(cateringItem);
        }
        // POST: CateringItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CateringItems == null)
            {
                return Problem("Entity set 'ApplicationDbContext.CateringItems'  is null.");
            }
            var cateringItem = await _context.CateringItems.FindAsync(id);
            if (cateringItem != null)
            {
                _context.CateringItems.Remove(cateringItem);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        //Map view Model To Catering Item
        private CateringItem MapViewModelToCateringItem(CateringItemViewModel viewModel, string filename)
        {
            return new CateringItem
            {
                ItemName = viewModel.ItemName,
                CateringCategoryId = viewModel.CateringCategoryId,
                CateringExtrasId = viewModel.CateringExtrasId,
                ItemDescription = viewModel.ItemDescription,
                ItemPrice = viewModel.ItemPrice,
                ItemImage = filename
            };
        }
        //Map View Model To Catering Item
        private void MapViewModelToCateringItem(CateringItemViewModel viewModel, CateringItem cateringItem, string filename)
        {
            cateringItem.ItemName = viewModel.ItemName;
            cateringItem.CateringCategoryId = viewModel.CateringCategoryId;
            cateringItem.CateringExtrasId = viewModel.CateringExtrasId;
            cateringItem.ItemDescription = viewModel.ItemDescription;
            cateringItem.ItemPrice = viewModel.ItemPrice;
            if (!string.IsNullOrEmpty(filename))
            {
                cateringItem.ItemImage = filename;
            }
        }
        //Check Duplicate ItemName
        [HttpPost]
        public JsonResult CheckDuplicateItemName(string itemId)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.OwnerEmail == userEmail);

            if (restaurant != null)
            {
                bool exists = _context.CateringItems.Any(ci => ci.ItemName == itemId && ci.RestaurantId == restaurant.RestaurantId);
                return Json(new { exists = exists });
            }
            else
            {
                // Handle scenario where the logged-in user is not associated with any restaurant
                return Json(new { exists = false, error = "User is not associated with any restaurant." });
            }
        }
        private bool CateringItemExists(int id)
        {
            return _context.CateringItems.Any(e => e.CateringItemId == id);
        }
    }
}

