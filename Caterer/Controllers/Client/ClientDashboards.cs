using Caterer.Data;
using Caterer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace Caterer.Controllers.Clients
{
    public class ClientDashboards : Controller
    {
        private readonly ApplicationDbContext _context;
        public ClientDashboards(ApplicationDbContext context)
        {
            _context = context;
        }
        //Client Dashboard
        public async Task<IActionResult> ClientDashboard()
        {
            string userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Problem("User email not found.");
            }
            var orderDetails = await _context.OrderDetailWebsites
                .Where(od => od.Email == userEmail)
                .Select(od => new OrderDetailWebsite
                {
                    OrderDetailsWebsiteId = od.OrderDetailsWebsiteId,
                    RestaurantName = od.RestaurantName,
                    DeliveryStatus=od.DeliveryStatus,
                    Status=od.Status,
                    GrandTotal = od.GrandTotal,
                })
                .ToListAsync();
            return View("~/Views/Client/ClientDashboards/ClientDashboard.cshtml", orderDetails);
        }
        //Client Order Details
        public IActionResult ClientOrderDetails(int id)
        {
            var orderDetail = _context.OrderDetailWebsites
                .Include(o => o.CateringCategory)
                .Include(o => o.CateringCategory.CateringItems)  
                .FirstOrDefault(o => o.OrderDetailsWebsiteId == id);
            if (orderDetail == null)
            {
                return NotFound();
            }
            return View("~/Views/Client/ClientDashboards/ClientOrderDetails.Cshtml",orderDetail);
        }
    }
}