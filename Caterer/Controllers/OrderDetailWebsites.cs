using Caterer.Data;
using Caterer.Data.Migrations;
using Caterer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.VisualBasic;
using System;
using System.Security.Claims;
using System.Text;
using System.Web.Helpers;
namespace Caterer.Controllers
{
    public class OrderDetailWebsites : Controller
    {
        private readonly ApplicationDbContext _context;
        public OrderDetailWebsites(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult RestaurantOrderDetails(int id)
        {
            var orderDetail = _context.OrderDetailWebsites
                .Include(o => o.CateringCategory)
                .Include(o => o.CateringCategory.CateringItems)
                .FirstOrDefault(o => o.OrderDetailsWebsiteId == id);
            if (orderDetail == null)
            {
                return NotFound();
            }
            return View(nameof(RestaurantOrderDetails), orderDetail);
        }
        // GET: Restaurant Live Orders
        public IActionResult RestaurantLiveOrders()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants
                .FirstOrDefault(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                var orderDetails = _context.OrderDetailWebsites
                    .Where(od => od.RestaurantId == restaurant.RestaurantId)
                    .ToList();
                return View(orderDetails);
            }
            return View(new List<OrderDetailWebsite>());
        }
        //Order Accepted
        public IActionResult OrderAccepted(int id)
        {
            var entity = _context.OrderDetailWebsites.Find(id);
            if (entity == null)
            {
                return NotFound();
            }
            entity.Status = "Accept";
            _context.SaveChanges();
            return RedirectToAction("RestaurantLiveOrders");
        }
        //Order Rejected
        public IActionResult OrderRejected(int id)
        {
            var entity = _context.OrderDetailWebsites.Find(id);
            if (entity == null)
            {
                return NotFound();
            }
            entity.Status = "";
            _context.SaveChanges();
            return RedirectToAction("RestaurantLiveOrders");
        }
        //Accepted Items
        public IActionResult AcceptedItems()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants
                .FirstOrDefault(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                var acceptedItems = _context.OrderDetailWebsites
                    .Where(item => item.Status == "Accept" && item.RestaurantId == restaurant.RestaurantId)
                    .ToList();
                var productions = _context.Productions
                    .Where(p => acceptedItems.Select(a => a.OrderDetailsWebsiteId).Contains(p.OrderDetailsWebsiteId))
                    .ToList();
                return View("AcceptedItems", new Tuple<List<OrderDetailWebsite>, List<Production>>(acceptedItems, productions));
            }
            return View("AcceptedItems", new Tuple<List<OrderDetailWebsite>, List<Production>>(new List<OrderDetailWebsite>(), new List<Production>()));
        }
        //Accept Details
        public IActionResult AcceptDetails(int id)
        {
            var orderDetail = _context.OrderDetailWebsites
                .Include(o => o.CateringCategory)
                .Include(o => o.CateringCategory.CateringItems)  
                .FirstOrDefault(o => o.OrderDetailsWebsiteId == id);
            if (orderDetail == null)
            {
                return NotFound();
            }
            return View(orderDetail);
        }
        //Payment Method
        public IActionResult PaymentMethod(string imageUrl, string restaurantName)
        {
            var lastOrder = _context.OrderDetailWebsites
                .OrderByDescending(o => o.OrderDetailsWebsiteId)
                .FirstOrDefault();
            return View(lastOrder);
        }
        //Get SubCategories
        public IActionResult GetSubcategories(int categoryId)
        {
            var subcategories = _context.CateringItems
                .Where(item => item.CateringCategoryId == categoryId)
                .Select(item => item.MenuCategoryName)
                .Distinct()
                .ToList();
            return Json(subcategories);
        }
        //Get Items For Category And Subcategory
        public IActionResult GetItemsForCategoryAndSubcategory(int categoryId, string subcategory)
        {
            var items = _context.CateringItems
                .Where(item => item.CateringCategoryId == categoryId && item.MenuCategoryName == subcategory)
                .Select(item => new { CateringItemId = item.MenuItemId, ItemName = item.ItemName, ItemPrice = item.ItemPrice })
                .ToList();
            return Json(items);
        }
        //Get Selected List For Subcategory
        public IActionResult GetSelectedListForSubcategory(int categoryId, string subcategory)
        {
            var selectedList = _context.CateringItems
                .Where(item => item.CateringCategoryId == categoryId && item.MenuCategoryName == subcategory)
                .Select(item => item.SelectedItem)
                .FirstOrDefault();
            return Json(selectedList);
        }
        // Get Extras For Catering Item
        public IActionResult GetExtrasForCateringItem(int cateringItemId)
        {
            var extras = _context.CateringExtras
                .Where(extra => extra.CateringItemId == cateringItemId)
                .Select(extra => new { extra.ExtraName, extra.ExtraPrice, extra.CateringExtrasId })
                .ToList();

            return Json(extras);
        }
        //Generate Random Password
        private string GenerateRandomPassword()
        {
            const string symbols = "!@#$";
            const string alphabets = "abcdefghijklmnopqrstuvwxyz";
            const string numbers = "0123456789";
            Random random = new Random();
            StringBuilder password = new StringBuilder();
            // Add five numbers
            for (int i = 0; i < 5; i++)
            {
                password.Append(numbers[random.Next(numbers.Length)]);
            }
            // Add two alphabets
            for (int i = 0; i < 2; i++)
            {
                password.Append(alphabets[random.Next(alphabets.Length)]);
            }
            // Add one symbol
            password.Append(symbols[random.Next(symbols.Length)]);
            return password.ToString();
        }
        //Get Date Options
        private List<SelectListItem> GetDateOptions()
        {
            var today = DateTime.Today;
            var dateOptions = Enumerable.Range(0, 15)
                .Select(offset => today.AddDays(offset))
                .Select(date => new SelectListItem
                {
                    Value = date.ToString("yyyy-MM-dd"),
                    Text = date.ToString("MMMM dd, yyyy")
                })
                .ToList();
            return dateOptions;
        }
        // Get Time Options
        private List<SelectListItem> GetTimeOptions()
        {
            var startTime = TimeSpan.Zero;
            var endTime = TimeSpan.FromHours(23) + TimeSpan.FromMinutes(59);
            var interval = TimeSpan.FromMinutes(30);
            var timeOptions = new List<SelectListItem>();
            for (var time = startTime; time <= endTime; time += interval)
            {
                var endTimeFormatted = time.Add(interval);
                timeOptions.Add(new SelectListItem
                {
                    Value = $"{time:hh\\:mm}-{endTimeFormatted:hh\\:mm}",
                    Text = $"{time:hh\\:mm}-{endTimeFormatted:hh\\:mm}"
                });
            }
            return timeOptions;
        }
        //Order Details
        public IActionResult OrderDetails(string imageUrl, string password, int servicetax, string logourl, int minumumOrder, string restaurantName, string restaurantDescription, string restaurantAddress, int restaurantId, int userId, string username, int taxId, decimal delivery, decimal buffet, string email)
        {
            var viewModel = new OrderDetailWebsite();
            viewModel.DateOptions = GetDateOptions();
            viewModel.TimeOptions = GetTimeOptions();
            ViewBag.TaxPercentage = servicetax;
            ViewBag.ImageUrl = imageUrl;
            ViewBag.LogoUrl = logourl;
            ViewBag.Email = email;
            ViewBag.RestaurantName = restaurantName;
            ViewBag.Minimum = minumumOrder;
            ViewBag.RestaurantDescription = restaurantDescription;
            ViewBag.RestaurantAddress = restaurantAddress;
            ViewBag.RestaurantId = restaurantId;
            ViewBag.UserId = userId;
            ViewBag.UserName = username;
            ViewBag.Delivery = delivery;
            ViewBag.Buffet = buffet;
            var cateringCategories = _context.CateringCategories
                .Where(category => category.RestaurantId == restaurantId)
                .ToList();
            ViewBag.CateringCategories = cateringCategories;

            if (string.IsNullOrEmpty(username))
            {
                string generatedPassword = GenerateRandomPassword();
                ViewBag.Password = generatedPassword;
                ViewBag.GeneratedPassword = generatedPassword;
            }
            else
            {
                var userInfo = _context.Users
                    .Where(user => user.UserID == userId)
                    .Select(user => new
                    {
                        user.Password
                    })
                    .FirstOrDefault();
                ViewBag.Password = userInfo?.Password ?? "Default Restaurant";
            }
            return View(viewModel);
        }
        //Post Order Details
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrderDetails(int id, OrderDetailWebsite model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

                if (existingUser == null)
                {
                    var user = new User
                    {
                        UserName = model.Name,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        Role = model.Role,
                        Password = model.Password
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }
                model.SelfCollect = Request.Form["chkSelfCollect"] == "on" ? 1 : 0;
                model.Delivery = Request.Form["chkDelivery"] == "on" ? 1 : 0;
                model.Buffet = Request.Form["chkBuffet"] == "on" ? 1 : 0;
                model.Card = Request.Form["cardCheckbox"] == "on" ? 1 : 0;
                model.Cash = Request.Form["cashCheckbox"] == "on" ? 1 : 0;
                model.PartialPay = Request.Form["chkPartialPay"] == "on" ? 1 : 0;
                model.FullPay = Request.Form["chkFullPay"] == "on" ? 1 : 0;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return View("~/Views/OrderDetailWebsites/PaymentMethod.cshtml", model);
            }
            return View("~/Views/OrderDetailsWebsites/OrderDetailsWebsites", model);
        }

        //Get Item Details
        [HttpGet]
        public IActionResult GetItemDetails(string itemName)
        {
            var itemDetails = _context.CateringItems
                .Where(item => item.ItemName == itemName)
                .Select(item => new { itemPrice = item.ItemPrice, cateringItemId = item.MenuItemId, cateringCategoryId = item.CateringCategoryId })
                .FirstOrDefault();
            return Json(itemDetails);
        }
        [HttpGet]
        public IActionResult GetItemNames(string menuCategoryName, int restaurantId)
        {
            var itemNames = _context.CateringItems
                .Where(item => item.MenuCategoryName == menuCategoryName && item.RestaurantId == restaurantId)
                .Select(item => new { itemId = item.ItemName, itemName = item.ItemName })
                .Distinct()
                .ToList();
            return Json(itemNames);
        }


        //Get Menu Categories
        [HttpGet]
        public IActionResult GetMenuCategories(int categoryId)
        {
            var menuCategories = _context.CateringItems
                .Where(item => item.CateringCategoryId == categoryId)
                .Select(item => new { menuCategoryId = item.MenuCategoryId, menuCategoryName = item.MenuCategoryName })
                .Distinct()
                .ToList();

            return Json(menuCategories);
        }
        //Get Extra Items Edit
        [HttpGet]
        public IActionResult GetExtraItemsEdit(int id)
        {
           
            var extraItems = _context.OrderDetailWebsites
                                     .Where(item => item.OrderDetailsWebsiteId == id )
                                     .ToList();
            return Json(extraItems);
        }
        // Get Order Details Edit
        public async Task<IActionResult> OrderDetailsEdit(int id)
        {
            var order = _context.OrderDetailWebsites
                 .Include(o => o.CateringCategory)
                 .Include(o => o.CateringCategory.CateringItems)
                 .FirstOrDefault(o => o.OrderDetailsWebsiteId == id);

            if (order == null)
            {
                return NotFound();
            }
            var restaurantInfo = _context.Restaurants.FirstOrDefault(r => r.RestaurantId == order.RestaurantId);

            if (restaurantInfo != null)
            {
                ViewBag.Minimum = restaurantInfo.Minimum;
                ViewBag.Delivery = restaurantInfo.Delivery;
                ViewBag.Buffet = restaurantInfo.Buffet;
            }
            order.DateOptions = GetDateOptions();
            order.TimeOptions = GetTimeOptions();
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                var cateringCategories = await _context.MenuCategories
                    .Where(mc => mc.RestaurantId == restaurant.RestaurantId)  
                    .ToListAsync();
                ViewBag.MenuCategories = cateringCategories;
            }
            return View(order);
        }
        //Post  Order Details Edit
        [HttpPost] 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrderDetailsEdit(int id, OrderDetailWebsite model, string[] ExtraItemPriceTotal, string[] ExtraNoOfPax, string[] ExtraCateringItemPriceTotal, string[] ExtraCateringItemId, string[] ExtraItemName, string[] ExtraItemPrice, string[] ExtraSubCategory, string[] CateringItemId, string[] ItemName, string[] ItemPrice, string[] Subcategory, string[] ExtraCateringCategoryId, string[] ExtraCateringSubCategory, string[] ExtraCateringItemName, string[] ExtraCateringNoOfPax, string[] ExtraCateringItemPrice, string[] ExtrasCateringItemId)
        {
            if (id != model.OrderDetailsWebsiteId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    model.CateringItemId = string.Join(",", CateringItemId);
                    model.ItemName = string.Join(",", ItemName);
                    model.ItemPrice = string.Join(",", ItemPrice);
                    model.Subcategory = string.Join(",", Subcategory);
                    model.ExtraCateringItemId = string.Join(",", ExtraCateringItemId.Where(x => !string.IsNullOrEmpty(x)));
                    model.ExtraItemName = string.Join(",", ExtraItemName.Where(x => !string.IsNullOrEmpty(x)));
                    model.ExtraItemPrice = string.Join(",", ExtraItemPrice.Where(x => !string.IsNullOrEmpty(x)));
                    model.ExtraSubcategory = string.Join(",", ExtraSubCategory.Where(x => !string.IsNullOrEmpty(x)));
                    model.ExtraItemPriceTotal = string.Join(",", ExtraItemPriceTotal.Where(x => !string.IsNullOrEmpty(x)));
                    model.ExtraNoOfPax = string.Join(",", ExtraNoOfPax.Where(x => !string.IsNullOrEmpty(x)));
                    model.ExtraCateringCategoryId = string.Join(",", ExtraCateringCategoryId.Where(x => !string.IsNullOrEmpty(x)));
                    model.ExtraCateringSubCategory = string.Join(",", ExtraCateringSubCategory.Where(x => !string.IsNullOrEmpty(x)));
                    model.ExtraCateringItemName = string.Join(",", ExtraCateringItemName.Where(x => !string.IsNullOrEmpty(x)));
                    model.ExtraCateringNoOfPax = string.Join(",", ExtraCateringNoOfPax.Where(x => !string.IsNullOrEmpty(x)));
                    model.ExtraCateringItemPrice = string.Join(",", ExtraCateringItemPrice.Where(x => !string.IsNullOrEmpty(x)));
                    model.ExtraCateringItemPriceTotal = string.Join(",", ExtraCateringItemPriceTotal.Where(x => !string.IsNullOrEmpty(x)));
                    model.ExtrasCateringItemId = string.Join(",", ExtrasCateringItemId.Where(x => !string.IsNullOrEmpty(x)));
                    model.SelfCollect = Request.Form["chkSelfCollect"] == "on" ? 1 : 0;
                    model.Delivery = Request.Form["chkDelivery"] == "on" ? 1 : 0;
                    model.Buffet = Request.Form["chkBuffet"] == "on" ? 1 : 0;
                    model.Card = Request.Form["cardCheckbox"] == "on" ? 1 : 0;
                    model.Cash = Request.Form["cashCheckbox"] == "on" ? 1 : 0;
                    model.PartialPay = Request.Form["chkPartialPay"] == "on" ? 1 : 0;
                    model.FullPay = Request.Form["chkFullPay"] == "on" ? 1 : 0;
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(model.OrderDetailsWebsiteId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("OrderList", "Restaurants");
            }
            return View(model);
        }
     
        //Post Soft Delete Order
        [HttpPost]
        public IActionResult SoftDeleteOrder(int orderId)
        {
            var order = _context.OrderDetailWebsites.Find(orderId);
            if (order != null)
            {
                order.SoftDelete = "Soft delete";
                _context.SaveChanges();
            }
            return Json(new { success = true });
        }
        // GET: Categories/Delete/5
        public async Task<IActionResult> OrderDetailDelete(int? id)
        {
            if (id == null || _context.OrderDetailWebsites == null)
            {
                return NotFound();
            }
            var order = await _context.OrderDetailWebsites
                .FirstOrDefaultAsync(m => m.OrderDetailsWebsiteId == id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }
        //Post Delete
        [HttpPost, ActionName("OrderDetailDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.OrderDetailWebsites == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Category' is null.");
            }
            var product = await _context.OrderDetailWebsites.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            _context.OrderDetailWebsites.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("OrderList", "Restaurants");

        }
        //Get Super Admin Order Details Edit
        public async Task<IActionResult> SuperAdminOrderDetailsEdit(int id)
        {
            var order = _context.OrderDetailWebsites
                 .Include(o => o.CateringCategory)
                 .Include(o => o.CateringCategory.CateringItems)
                 .FirstOrDefault(o => o.OrderDetailsWebsiteId == id);

            if (order == null)
            {
                return NotFound();
            }
            var restaurantInfo = _context.Restaurants.FirstOrDefault(r => r.RestaurantId == order.RestaurantId);

            if (restaurantInfo != null)
            {
                ViewBag.Minimum = restaurantInfo.Minimum;
                ViewBag.Delivery = restaurantInfo.Delivery;
                ViewBag.Buffet = restaurantInfo.Buffet;
            }
            order.DateOptions = GetDateOptions();
            order.TimeOptions = GetTimeOptions();
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                var cateringCategories = await _context.MenuCategories
                    .Where(mc => mc.RestaurantId == restaurant.RestaurantId)
                    .ToListAsync();
                ViewBag.MenuCategories = cateringCategories;
            }
            return View(order);
        }
        //Post Super Admin Order Details Edit

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuperAdminOrderDetailsEdit(int id, OrderDetailWebsite model, string[] ExtraItemPriceTotal, string[] ExtraNoOfPax, string[] ExtraCateringItemPriceTotal, string[] ExtraCateringItemId, string[] ExtraItemName, string[] ExtraItemPrice, string[] ExtraSubCategory, string[] CateringItemId, string[] ItemName, string[] ItemPrice, string[] Subcategory, string[] ExtraCateringCategoryId, string[] ExtraCateringSubCategory, string[] ExtraCateringItemName, string[] ExtraCateringNoOfPax, string[] ExtraCateringItemPrice, string[] ExtrasCateringItemId)
        {
            if (id != model.OrderDetailsWebsiteId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    model.CateringItemId = string.Join(",", CateringItemId);
                    model.ItemName = string.Join(",", ItemName);
                    model.ItemPrice = string.Join(",", ItemPrice);
                    model.Subcategory = string.Join(",", Subcategory);
                    model.ExtraCateringItemId = string.Join(",", ExtraCateringItemId.Where(x => !string.IsNullOrEmpty(x)));
                    model.ExtraItemName = string.Join(",", ExtraItemName.Where(x => !string.IsNullOrEmpty(x)));
                    model.ExtraItemPrice = string.Join(",", ExtraItemPrice.Where(x => !string.IsNullOrEmpty(x)));
                    model.ExtraSubcategory = string.Join(",", ExtraSubCategory.Where(x => !string.IsNullOrEmpty(x)));
                    model.ExtraItemPriceTotal = string.Join(",", ExtraItemPriceTotal.Where(x => !string.IsNullOrEmpty(x)));
                    model.ExtraNoOfPax = string.Join(",", ExtraNoOfPax.Where(x => !string.IsNullOrEmpty(x)));
                    model.ExtraCateringCategoryId = string.Join(",", ExtraCateringCategoryId.Where(x => !string.IsNullOrEmpty(x)));
                    model.ExtraCateringSubCategory = string.Join(",", ExtraCateringSubCategory.Where(x => !string.IsNullOrEmpty(x)));
                    model.ExtraCateringItemName = string.Join(",", ExtraCateringItemName.Where(x => !string.IsNullOrEmpty(x)));
                    model.ExtraCateringNoOfPax = string.Join(",", ExtraCateringNoOfPax.Where(x => !string.IsNullOrEmpty(x)));
                    model.ExtraCateringItemPrice = string.Join(",", ExtraCateringItemPrice.Where(x => !string.IsNullOrEmpty(x)));
                    model.ExtraCateringItemPriceTotal = string.Join(",", ExtraCateringItemPriceTotal.Where(x => !string.IsNullOrEmpty(x)));
                    model.ExtrasCateringItemId = string.Join(",", ExtrasCateringItemId.Where(x => !string.IsNullOrEmpty(x)));
                    model.SelfCollect = Request.Form["chkSelfCollect"] == "on" ? 1 : 0;
                    model.Delivery = Request.Form["chkDelivery"] == "on" ? 1 : 0;
                    model.Buffet = Request.Form["chkBuffet"] == "on" ? 1 : 0;
                    model.Card = Request.Form["cardCheckbox"] == "on" ? 1 : 0;
                    model.Cash = Request.Form["cashCheckbox"] == "on" ? 1 : 0;
                    model.PartialPay = Request.Form["chkPartialPay"] == "on" ? 1 : 0;
                    model.FullPay = Request.Form["chkFullPay"] == "on" ? 1 : 0;
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(model.OrderDetailsWebsiteId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("SuperAdminOrderList", "Restaurants");
            }
            return View(model);
        }
        // GET: Categories/Delete/5
        public async Task<IActionResult> SuperAdminOrderDetailDelete(int? id)
        {
            if (id == null || _context.OrderDetailWebsites == null)
            {
                return NotFound();
            }
            var order = await _context.OrderDetailWebsites
                .FirstOrDefaultAsync(m => m.OrderDetailsWebsiteId == id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }
        //Post Delete
        [HttpPost, ActionName("SuperAdminOrderDetailDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuperAdminDeleteConfirmed(int id)
        {
            if (_context.OrderDetailWebsites == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Category' is null.");
            }
            var product = await _context.OrderDetailWebsites.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            _context.OrderDetailWebsites.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("SuperAdminOrderList", "Restaurants");
        }
        private bool OrderExists(int id)
        {
            return _context.OrderDetailWebsites.Any(e => e.OrderDetailsWebsiteId == id);
        }
    }
}