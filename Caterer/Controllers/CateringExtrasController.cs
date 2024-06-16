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
    public class CateringExtrasController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CateringExtrasController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: CateringExtras
        public async Task<IActionResult> Index()
        {
              return View(await _context.CateringExtras.ToListAsync());
        }
        // GET: CateringExtras/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CateringExtras == null)
            {
                return NotFound();
            }
            var cateringExtra = await _context.CateringExtras
                .FirstOrDefaultAsync(m => m.CateringExtrasId == id);
            if (cateringExtra == null)
            {
                return NotFound();
            }
            return View(cateringExtra);
        }

        // GET: CateringExtras/Create
        public IActionResult Create(int? cateringItemId)
        {
            var model = new CateringExtra
            {
                CateringItemId = cateringItemId ?? 0 
            };

            return View(model);
        }
        // POST: CateringExtras/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CateringExtrasId,CateringItemId,ExtraName,ExtraPrice,AddonCategory")] CateringExtra cateringExtra)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cateringExtra);
                await _context.SaveChangesAsync();
                return RedirectToAction("Edit", "CateringItems", new { id = cateringExtra.CateringItemId });
            }
            return View(cateringExtra);
        }
        // GET: CateringExtras/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CateringExtras == null)
            {
                return NotFound();
            }

            var cateringExtra = await _context.CateringExtras.FindAsync(id);
            if (cateringExtra == null)
            {
                return NotFound();
            }
            return View(cateringExtra);
        }
        // POST: CateringExtras/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CateringExtrasId,CateringItemId,ExtraName,ExtraPrice,AddonCategory")] CateringExtra cateringExtra)
        {
            if (id != cateringExtra.CateringExtrasId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cateringExtra);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CateringExtraExists(cateringExtra.CateringExtrasId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
               return RedirectToAction("Edit", "CateringItems", new { id = cateringExtra.CateringItemId });
            }
            return View(cateringExtra);
        }
        // GET: CateringExtras/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CateringExtras == null)
            {
                return NotFound();
            }
            var cateringExtra = await _context.CateringExtras
                .FirstOrDefaultAsync(m => m.CateringExtrasId == id);
            if (cateringExtra == null)
            {
                return NotFound();
            }
            return View(cateringExtra);
        }
        // POST: CateringExtras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CateringExtras == null)
            {
                return Problem("Entity set 'ApplicationDbContext.CateringExtras'  is null.");
            }
            var cateringExtra = await _context.CateringExtras.FindAsync(id);
            if (cateringExtra != null)
            {
                _context.CateringExtras.Remove(cateringExtra);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index),"CateringItems");
        }
        private bool CateringExtraExists(int id)
        {
          return _context.CateringExtras.Any(e => e.CateringExtrasId == id);
        }
    }
}
