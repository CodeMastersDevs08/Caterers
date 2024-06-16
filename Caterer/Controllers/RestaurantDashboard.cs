using Microsoft.AspNetCore.Mvc;
using Caterer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Caterer.Data;
using Microsoft.EntityFrameworkCore;
using Caterer.Data.Migrations;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Security.Claims;
namespace Caterer.Controllers
{
    public class RestaurantDashboard : Controller
    {
        private readonly ApplicationDbContext _context;
        public RestaurantDashboard(ApplicationDbContext context)
        {
            _context = context;
        }
        //Index
        public async Task<IActionResult> Index()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                var orders = _context.OrderDetailWebsites
                    .Where(o => o.RestaurantId == restaurant.RestaurantId)
                    .ToList();
                var totalOrders = orders.Count();
                var acceptedOrdersCount = orders.Count(o => o.Status == "Accept");
                decimal totalGrandTotal = orders.Sum(o => o.GrandTotal);
                var products = _context.Products.ToList();  
                var lineChartData = GetLineChartData(orders);
                var barChartData = GetBarChartData(products);
                var pieChartData = GetPieChartData(orders);
                var surfaceResponseData = GetSurfaceResponseData();
                var scatterPlotData = GetScatterPlotData();
                var workingHours = _context.Restaurants
               .Where(r => r.OwnerEmail == userEmail)
               .FirstOrDefault();
                var mostSoldItemName = orders
                    .SelectMany(o => o.ItemName.Split(','))  
                    .GroupBy(itemName => itemName.Trim())
                    .OrderByDescending(group => group.Count())
                    .Select(group => group.Key)
                    .FirstOrDefault();
                var topRestaurants = _context.Restaurants.Take(5).ToList();
                var allRestaurants = _context.Restaurants.ToList();
                ViewBag.TotalOrders = totalOrders;
                ViewBag.AcceptedOrdersCount = acceptedOrdersCount;
                ViewBag.TotalGrandTotal = totalGrandTotal;
                ViewBag.LineChartData = lineChartData;
                ViewBag.BarChartData = barChartData;
                ViewBag.PieChartData = pieChartData;
                ViewBag.SurfaceResponseData = surfaceResponseData;
                ViewBag.ScatterPlotData = scatterPlotData;
                ViewBag.MostSoldItemName = mostSoldItemName;
                ViewBag.WorkingHours = workingHours;
                ViewBag.AllRestaurants = allRestaurants; 
                ViewBag.TopRestaurants = topRestaurants;  
                return View();
            }
            return View("NoRestaurantFound");
        }
        //Get Line Chart Data
        private Dictionary<DateTime, int> GetLineChartData(List<OrderDetailWebsite> orders)
        {
            var chartData = new Dictionary<DateTime, int>();
            var currentDate = DateTime.Now.Date;
            var startDate = currentDate.AddDays(-29);  
            var groupedOrders = orders
                .Where(o =>
                    !string.IsNullOrEmpty(o.SelectedDate) &&
                    DateTime.TryParseExact(o.SelectedDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var orderDate) &&
                    orderDate >= startDate &&
                    orderDate <= currentDate)
                .GroupBy(o => DateTime.ParseExact(o.SelectedDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).Date)
                .Select(group => new
                {
                    Date = group.Key,
                    OrderCount = group.Count()
                })
                .OrderBy(x => x.Date);

            foreach (var group in groupedOrders)
            {
                chartData[group.Date] = group.OrderCount;
            }
            return chartData;
        }
        //Get Bar Chart Data
        private Dictionary<string, decimal> GetBarChartData(List<Product> products)
        {
            var chartData = new Dictionary<string, decimal>();
            var lowQuantityProducts = products.OrderByDescending(p => p.Quantity).Take(5);
            foreach (var product in lowQuantityProducts)
            {
                chartData[product.ProductName] = product.Quantity ?? 0;
            }
            return chartData;
        }
        // Get Pie Chart Data
        private Dictionary<string, int> GetPieChartData(List<OrderDetailWebsite> orders)
        {
            var chartData = new Dictionary<string, int>();
            var items = orders
                .SelectMany(o => o.ItemName.Split(','));
            var groupedItems = items
                .GroupBy(item => item.Trim())
                .Select(group => new
                {
                    ItemName = group.Key,
                    OrderCount = group.Count()
                })
                .OrderByDescending(x => x.OrderCount)
                .Take(5);
            foreach (var item in groupedItems)
            {
                chartData[item.ItemName] = item.OrderCount;
            }
            return chartData;
        }
        //Get Surface Response Data
        private List<SurfaceResponseData> GetSurfaceResponseData()
        {
            var surfaceResponseData = _context.OrderDetailWebsites
                .GroupBy(o => o.ItemName)
                .OrderByDescending(group => group.Count())
                .Take(5)
                .Select(group => new SurfaceResponseData
                {
                    ItemName = group.Key,
                    OrderCount = group.Count()
                })
                .ToList();
            return surfaceResponseData;
        }
        public class SurfaceResponseData
        {
            public string ItemName { get; set; }
            public int OrderCount { get; set; }
        }
        //Get Scatter Plot Data
        private List<ScatterDataPoint1> GetScatterPlotData()
        {
            var scatterPlotData = new List<ScatterDataPoint1>
            {
                new ScatterDataPoint1 { A = 10, B = 20 },
                new ScatterDataPoint1 { A= 15, B = 25 },
                new ScatterDataPoint1 { A = 20, B = 30 },
                new ScatterDataPoint1 { A = 25, B = 35 },
                new ScatterDataPoint1 { A = 30, B = 40 }
            };
            return scatterPlotData;
        }
    }
    public class ScatterDataPoint1
    {
        public int A { get; set; }
        public int B { get; set; }
    }
}
