using Caterer.Data;
using Caterer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace Caterer.Controllers.Website
{
    public class Websites : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public Websites(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: Cat/Create
        public IActionResult WebsiteHome()
        {
            var featuredRestaurants = _context.Restaurants.Where(r => r.IsFeatured).ToList();

            return View("~/Views/WebSite/WebsiteHome.cshtml", featuredRestaurants);
        }

        //Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _httpContextAccessor.HttpContext.Session.Clear();
            Response.Cookies.Delete("CookieName");
            Response.Headers["Cache-Control"] = "no-cache, no-store";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "-1";
            return RedirectToAction("WebsiteHome", "Websites");
        }
    }
}