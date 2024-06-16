using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Caterer.Data;
using Caterer.Models;
namespace Caterer.Controllers
{
    public class ExtrasController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ExtrasController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: Extras
        public async Task<IActionResult> Index()
        {
              return View(await _context.Extras.ToListAsync());
        }
        // GET: Extras/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Extras == null)
            {
                return NotFound();
            }
            var extras = await _context.Extras
                .FirstOrDefaultAsync(m => m.ExtrasId == id);
            if (extras == null)
            {
                return NotFound();
            }
            return View(extras);
        }
        // GET: Extras/Create
        public IActionResult Create(int? menuItemId)
        {
            var model = new Extra
            {
                MenuItemId = menuItemId ?? 0 
            };
            return View(model);
        }
        // POST: Extras/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExtrasId,MenuItemId,ExtraName,ExtraPrice,AddonCategory")] Extra extras)
        {
            if (ModelState.IsValid)
            {
                _context.Add(extras);
                await _context.SaveChangesAsync();
                return RedirectToAction("Edit", "MenuItems", new { id = extras.MenuItemId });
            }
            return View(extras);
        }
        // GET: Extras/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Extras == null)
            {
                return NotFound();
            }
            var extras = await _context.Extras.FindAsync(id);
            if (extras == null)
            {
                return NotFound();
            }
            return View(extras);
        }
        // POST: Extras/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExtrasId,MenuItemId,ExtraName,ExtraPrice,AddonCategory")] Extra extras)
        {
            if (id != extras.ExtrasId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(extras);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExtrasExists(extras.ExtrasId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Edit", "MenuItems", new { id = extras.MenuItemId });
            }
            return View(extras);
        }
        // GET: Extras/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Extras == null)
            {
                return NotFound();
            }
            var extras = await _context.Extras
                .FirstOrDefaultAsync(m => m.ExtrasId == id);
            if (extras == null)
            {
                return NotFound();
            }
            return View(extras);
        }
        // POST: Extras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Extras == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Extras'  is null.");
            }
            var extras = await _context.Extras.FindAsync(id);
            if (extras != null)
            {
                _context.Extras.Remove(extras);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index),"MenuItems");
        }
        private bool ExtrasExists(int id)
        {
          return _context.Extras.Any(e => e.ExtrasId == id);
        }
    }
}
