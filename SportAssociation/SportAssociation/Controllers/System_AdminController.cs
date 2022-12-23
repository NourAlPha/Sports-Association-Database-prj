using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportAssociation.Data;
using SportAssociation.Models;

namespace SportAssociation.Controllers
{
    public class System_AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public System_AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: System_Admin
        public async Task<IActionResult> Index()
        {
              return View(await _context.System_Admin.ToListAsync());
        }

        // GET: System_Admin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.System_Admin == null)
            {
                return NotFound();
            }

            var system_Admin = await _context.System_Admin
                .FirstOrDefaultAsync(m => m.Id == id);
            if (system_Admin == null)
            {
                return NotFound();
            }

            return View(system_Admin);
        }

        // GET: System_Admin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: System_Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Username")] System_Admin system_Admin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(system_Admin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(system_Admin);
        }

        // GET: System_Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.System_Admin == null)
            {
                return NotFound();
            }

            var system_Admin = await _context.System_Admin.FindAsync(id);
            if (system_Admin == null)
            {
                return NotFound();
            }
            return View(system_Admin);
        }

        // POST: System_Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Username")] System_Admin system_Admin)
        {
            if (id != system_Admin.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(system_Admin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!System_AdminExists(system_Admin.Id))
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
            return View(system_Admin);
        }

        // GET: System_Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.System_Admin == null)
            {
                return NotFound();
            }

            var system_Admin = await _context.System_Admin
                .FirstOrDefaultAsync(m => m.Id == id);
            if (system_Admin == null)
            {
                return NotFound();
            }

            return View(system_Admin);
        }

        // POST: System_Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.System_Admin == null)
            {
                return Problem("Entity set 'ApplicationDbContext.System_Admin'  is null.");
            }
            var system_Admin = await _context.System_Admin.FindAsync(id);
            if (system_Admin != null)
            {
                _context.System_Admin.Remove(system_Admin);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool System_AdminExists(int id)
        {
          return _context.System_Admin.Any(e => e.Id == id);
        }
    }
}
