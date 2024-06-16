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
    public class StocksReport : Controller
    {
        private readonly ApplicationDbContext _context;
        public StocksReport(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> GenerateReport()
        {
            var stockAdjustments = await _context.StockAdjustments
                .Include(sa => sa.Product)
                .ToListAsync();

            return View(stockAdjustments);
        }
    }
}
