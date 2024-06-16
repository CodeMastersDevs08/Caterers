using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Caterer.Data;
using Caterer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Caterer.Data.Migrations;
using Microsoft.AspNetCore.Authorization;
namespace Caterer.Controllers.Website
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }
        //Get warehouse Details
        public IActionResult GetWarehouseDetails(string warehouseName)
        {
            var warehouse = _context.Warehouses.FirstOrDefault(w => w.WarehouseName == warehouseName);
            if (warehouse != null)
            {
                return Json(new { warehouseId = warehouse.WarehouseId, warehouseName = warehouse.WarehouseName });
            }
            return Json(null);
        }
        //Get warehouse Names
        public IActionResult GetWarehouseNames()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                var warehouseNames = _context.Warehouses
                    .Where(w => w.RestaurantId == restaurant.RestaurantId)
                    .Select(w => w.WarehouseName)
                    .ToList();
                return Json(warehouseNames);
            }
            return Json(new List<string>());
        }
        // GET: Users
        [Authorize]
        public async Task<IActionResult> SuperAdminList()
        {
            return _context.Users != null ?
                        View("~/Views/SuperAdmin/Users/SuperAdminList.cshtml", await _context.Users.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.User'  is null.");
        }
        //User list
        [Authorize]
        public async Task<IActionResult> UserList()
        {
            return _context.Users != null ?
                        View("~/Views/Website/Users/UserList.cshtml", await _context.Users.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.User'  is null.");
        }
        // GET: Users/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        // GET: Users/Create
        public IActionResult RegisterClient()
        {
            return View("~/Views/Website/Users/RegisterClient.cshtml");
        }
        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterClient([Bind("UserID,UserName,RestaurantName,Email,PhoneNumber,Password,Role")] User user)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Users.AnyAsync(u => u.UserName == user.UserName))
                {
                    ModelState.AddModelError("UserName", "This UserName is already taken.");
                    return View("~/Views/Website/Users/RegisterClient.cshtml", user);
                }
                if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "This Email is already registered.");
                    return View("~/Views/Website/Users/RegisterClient.cshtml", user);
                }
                if (await _context.Users.AnyAsync(u => u.PhoneNumber == user.PhoneNumber))
                {
                    ModelState.AddModelError("PhoneNumber", "This Phone Number is already registered.");
                    return View("~/Views/Website/Users/RegisterClient.cshtml", user);
                }
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login", "Users");
            }
            return View("~/Views/Website/Users/RegisterClient.cshtml", user);
        }
        // Get Register SuperAdmin
        public IActionResult RegisterSuperAdmin()
        {
            return View("~/Views/SuperAdmin/Users/RegisterSuperAdmin.cshtml");
        }
        //Post Register SuperAdmin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterSuperAdmin([Bind("UserID,UserName,RestaurantName,Email,PhoneNumber,Password,Role")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login", "Users");
            }
            return View("~/Views/SuperAdmin/Users/RegisterSuperAdmin.cshtml", user);
        }
        // GET: Users/Login
        public IActionResult Login()
        {
            return View("~/Views/Website/Users/Login.cshtml");
        }
        //Post Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);

                if (user != null)
                {
                    var claims = new List<Claim>
             {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString())
             };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe
                    };
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);
                    if (user.Role == "Client")
                    {
                        return RedirectToAction("WebsiteHome", "Websites");
                    }
                    else if (user.Role == "SuperAdmin")
                    {
                        return RedirectToAction("Index", "Dashboard");
                    }
                    else if (user.Role == "Restaurant")
                    {
                        return RedirectToAction("RestaurantList", "Restaurants");
                    }

                }
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
            }
            return View("~/Views/Website/Users/Login.cshtml", model);
        }
        // GET: Users/Login
        [Authorize]
        public IActionResult LoginAdmin()
        {
            return View("~/Views/Website/Users/LoginAdmin.cshtml");
        }
        // POST: Users/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAdmin(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);
                if (user != null)
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString())
            };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe
                    };
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);
                    if (user.Role == "Client")
                    {
                        return RedirectToAction("WebsiteHome", "Websites");
                    }
                    else if (user.Role == "SuperAdmin")
                    {
                        return RedirectToAction("SuperAdminlist", "Users");
                    }
                    else if (user.Role == "Restaurant")
                    {
                        return RedirectToAction("RestaurantList", "Restaurants");
                    }
                }
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
            }
            return View("~/Views/Website/Users/Login.cshtml", model);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserID,UserName,RestaurantName,Email,PhoneNumber,Password,Role")] User user)
        {
            if (id != user.UserID)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserID))
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
            return View(user);
        }
        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'ApplicationDbContext.User'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.UserID == id)).GetValueOrDefault();
        }
    }
}