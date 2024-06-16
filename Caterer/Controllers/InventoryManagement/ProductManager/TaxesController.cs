using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Caterer.Data;
using Caterer.Data.Migrations;
using Caterer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
namespace Caterer.Controllers.InventoryManagement.ProductManager
{
    public class TaxesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TaxesController(ApplicationDbContext context)
        {
            _context = context;
        }
        private int GenerateTaxNo()
        {
            var latestTax = _context.Taxes
                .OrderByDescending(t => t.TaxNo)
                .FirstOrDefault();
            int nextCode = 1;
            if (latestTax != null)
            {
                nextCode = latestTax.TaxNo + 1;
            }
            return nextCode;
        }
        // GET: Taxes
        public async Task<IActionResult> TaxList()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                var distinctTaxNumbers = await _context.Taxes
                    .Where(t => t.RestaurantId == restaurant.RestaurantId)  
                    .Select(t => t.TaxNo)
                    .Distinct()
                    .ToListAsync();
                var taxes = new List<Tax>();
                foreach (var taxNo in distinctTaxNumbers)
                {
                    var tax = await _context.Taxes
                        .FirstOrDefaultAsync(t => t.RestaurantId == restaurant.RestaurantId && t.TaxNo == taxNo);
                    if (tax != null)
                    {
                        taxes.Add(tax);
                    }
                }
                return View("~/Views/InventoryManagement/ProductManager/Taxes/TaxList.cshtml", taxes);
            }
            return Problem("Entity set 'ApplicationDbContext.Restaurants' is null or user is not associated with any restaurant.");
        }
        // GET: Taxes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Taxes == null)
            {
                return NotFound();
            }
            var tax = await _context.Taxes
                .FirstOrDefaultAsync(m => m.TaxId == id);
            if (tax == null)
            {
                return NotFound();
            }
            return View("~/Views/Restaurant/ProductManager/Taxes/Details.cshtml", tax);
        }
        // GET: Taxes/Create
        public IActionResult TaxCreate()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants
                .FirstOrDefault(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                var tax = new Tax
                {
                };
                ViewData["RestaurantId"] = restaurant.RestaurantId;
                return View("~/Views/InventoryManagement/ProductManager/Taxes/TaxCreate.cshtml", tax);
            }
            return RedirectToAction("Index", "Home");  
        }
        // POST: Taxes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaxCreate([Bind("TaxId,TaxNo,TaxName,TaxType,TaxPercentage,RestaurantId")] Tax tax, List<string> TaxName, List<decimal> TaxPercentage)
        {
            int generatedCode = GenerateTaxNo();
            tax.TaxNo = generatedCode;
            if (ModelState.IsValid)
            {
                bool isDuplicateTaxName = _context.Taxes.Any(t => t.RestaurantId == tax.RestaurantId && t.TaxName == tax.TaxName);
                if (isDuplicateTaxName)
                {
                    ModelState.AddModelError("TaxName", "TaxName already exists.");
                    return View("~/Views/InventoryManagement/ProductManager/Taxes/TaxCreate.cshtml", tax);
                }
                _context.Add(tax);
                for (int i = 0; i < TaxName.Count; i++)
                {
                    if (i == 0)
                        continue;
                    var additionalTax = new Tax
                    {
                        RestaurantId = tax.RestaurantId,
                        TaxId = tax.TaxId,  
                        TaxNo = tax.TaxNo,
                        TaxType = tax.TaxType,
                        TaxName = TaxName[i],
                        TaxPercentage = TaxPercentage[i],
                    };
                    _context.Add(additionalTax);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(TaxList));
            }

            return View("~/Views/InventoryManagement/ProductManager/Taxes/TaxCreate.cshtml", tax);
        }
        // GET: Taxes/Edit/5
        public async Task<IActionResult> TaxEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var tax = await _context.Taxes.FindAsync(id);
            if (tax == null)
            {
                return NotFound();
            }
            var taxesWithSameTaxNo = await _context.Taxes
                .Where(t => t.TaxNo == tax.TaxNo)
                .ToListAsync();
            if (taxesWithSameTaxNo == null || !taxesWithSameTaxNo.Any())
            {
                return NotFound();
            }
            var viewModel = new TaxEditViewModel
            {
                Taxes = taxesWithSameTaxNo
            };
            return View("~/Views/InventoryManagement/ProductManager/Taxes/TaxEdit.cshtml", viewModel);
        }
        // POST: Taxes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaxEdit(List<Tax> taxes)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    foreach (var tax in taxes)
                    {
                        _context.Update(tax);
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(TaxList));
            }
            return View("~/Views/InventoryManagement/ProductManager/Taxes/TaxEdit.cshtml", taxes);
        }
        // GET: Taxes/Delete/5
        public async Task<IActionResult> TaxDelete(int? taxNo)
        {
            if (taxNo == null)
            {
                return NotFound();
            }
            var taxesWithSameTaxNo = await _context.Taxes
                .Where(m => m.TaxNo == taxNo)
                .ToListAsync();
            if (taxesWithSameTaxNo == null || !taxesWithSameTaxNo.Any())
            {
                return NotFound();
            }
            return View(taxesWithSameTaxNo);
        }
        // POST: Taxes/Delete/5
        [HttpPost, ActionName("TaxDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int taxNo)
        {
            Console.WriteLine("DeleteConfirmed action called.");
            var taxesWithSameTaxNo = await _context.Taxes
                .Where(m => m.TaxNo == taxNo)
                .ToListAsync();
            if (taxesWithSameTaxNo == null || !taxesWithSameTaxNo.Any())
            {
                return NotFound();
            }
            _context.Taxes.RemoveRange(taxesWithSameTaxNo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(TaxList));
        }
        private bool TaxExists(int id)
        {
            return _context.Taxes.Any(e => e.TaxId == id);
        }
    }
}