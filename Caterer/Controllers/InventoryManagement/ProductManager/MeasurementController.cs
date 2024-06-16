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
using Caterer.Data.Migrations;

namespace Caterer.Controllers.InventoryManagement.ProductManager
{
    public class MeasurementController : Controller
    {
        private readonly ApplicationDbContext _context;
        public MeasurementController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: Measurement
        public async Task<IActionResult> MeasurementList()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                var measurements = await _context.Measurements
                    .Where(m => m.RestaurantId == restaurant.RestaurantId)  
                           .OrderByDescending(s => s.MeasurementId)
                    .ToListAsync();
                return View("~/Views/InventoryManagement/ProductManager/Measurement/MeasurementList.cshtml", measurements);
            }
            return Problem("Entity set 'ApplicationDbContext.Restaurants' is null or user is not associated with any restaurant.");
        }
        // GET: Measurement/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Measurements == null)
            {
                return NotFound();
            }
            var measurement = await _context.Measurements
                .FirstOrDefaultAsync(m => m.MeasurementId == id);
            if (measurement == null)
            {
                return NotFound();
            }
            return View(measurement);
        }
        //Post Store
        [HttpPost]
        public JsonResult store(Models.Measurement measurement)
        {
            var pro = new Models.Measurement()
            {
                MeasurementName = measurement.MeasurementName,
                RestaurantId  = measurement.RestaurantId,
            };
            _context.Measurements.Add(pro);
            _context.SaveChanges();
            return new JsonResult("Product Saved Successfully....!");
        }
        //Get Measurement Create
        public IActionResult MeasurementCreate()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants
                .FirstOrDefault(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                ViewData["RestaurantId"] = restaurant.RestaurantId;
                if (TempData.ContainsKey("ErrorMessage"))
                {
                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];
                }
                return View("~/Views/InventoryManagement/ProductManager/Measurement/MeasurementCreate.cshtml");
            }
            return RedirectToAction("Index", "Home");  
        }

        // POST: Measurement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MeasurementCreate([Bind("MeasurementId,MeasurementName")] Models.Measurement measurement)
        {
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var restaurant = await _context.Restaurants
                    .FirstOrDefaultAsync(r => r.OwnerEmail == userEmail);
                if (restaurant == null)
                {
                    return RedirectToAction("Index", "Home");  
                }
                measurement.RestaurantId = restaurant.RestaurantId;
                if (_context.Measurements.Any(m => m.MeasurementName == measurement.MeasurementName))
                {
                    ModelState.AddModelError("MeasurementName", "Measurement name already exists.");
                    return View("~/Views/InventoryManagement/ProductManager/Measurement/MeasurementCreate.cshtml", measurement);
                }
                if (ModelState.IsValid)
                {
                    _context.Add(measurement);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("MeasurementList","Measurement");
                }
                return View("~/Views/InventoryManagement/ProductManager/Measurement/MeasurementCreate.cshtml", measurement);
            }
            catch (Exception ex)
            {
                return Problem("An error occurred while processing your request. Please try again later.");
            }
        }
        // GET: Measurement/Edit/5
        public async Task<IActionResult> MeasurementEdit(int? id)
        {
            if (id == null || _context.Measurements == null)
            {
                return NotFound();
            }
            var measurement = await _context.Measurements.FindAsync(id);
            if (measurement == null)
            {
                return NotFound();
            }
            return View("~/Views/InventoryManagement/ProductManager/Measurement/MeasurementEdit.cshtml", measurement);
        }
        // POST: Measurement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MeasurementEdit(int id, [Bind("MeasurementId,MeasurementName,RestaurantId")] Models.Measurement measurement)
        {
            if (id != measurement.MeasurementId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(measurement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MeasurementExists(measurement.MeasurementId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(MeasurementList));
            }
            return View("~/Views/InventoryManagement/ProductManager/Measurement/MeasurementEdit.cshtml", measurement);
        }
        // GET: Measurement/Delete/5
        public async Task<IActionResult> MeasurementDelete(int? id)
        {
            if (id == null || _context.Measurements == null)
            {
                return NotFound();
            }
            var measurement = await _context.Measurements
                .FirstOrDefaultAsync(m => m.MeasurementId == id);
            if (measurement == null)
            {
                return NotFound();
            }
            return View("~/Views/InventoryManagement/ProductManager/Measurement/MeasurementDelete.cshtml", measurement);
        }
        // POST: Measurement/Delete/5
        [HttpPost, ActionName("MeasurementDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Measurements == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Measurement'  is null.");
            }
            var measurement = await _context.Measurements.FindAsync(id);
            if (measurement != null)
            {
                _context.Measurements.Remove(measurement);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MeasurementList));
        }
        private bool MeasurementExists(int id)
        {
            return (_context.Measurements?.Any(e => e.MeasurementId == id)).GetValueOrDefault();
        }
    }
}
