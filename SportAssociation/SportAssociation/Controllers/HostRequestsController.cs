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
    public class HostRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HostRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HostRequests
        public async Task<IActionResult> Index()
        {
              return View(await _context.HostRequest.ToListAsync());
        }

        // GET: HostRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.HostRequest == null)
            {
                return NotFound();
            }

            var hostRequest = await _context.HostRequest
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hostRequest == null)
            {
                return NotFound();
            }

            return View(hostRequest);
        }

        // GET: HostRequests/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HostRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RepresentativeId,ManagerId,MatchId,Status")] HostRequest hostRequest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hostRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hostRequest);
        }

        // GET: HostRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.HostRequest == null)
            {
                return NotFound();
            }

            var hostRequest = await _context.HostRequest.FindAsync(id);
            if (hostRequest == null)
            {
                return NotFound();
            }
            return View(hostRequest);
        }

        // POST: HostRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RepresentativeId,ManagerId,MatchId,Status")] HostRequest hostRequest)
        {
            if (id != hostRequest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hostRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HostRequestExists(hostRequest.Id))
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
            return View(hostRequest);
        }

        // GET: HostRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.HostRequest == null)
            {
                return NotFound();
            }

            var hostRequest = await _context.HostRequest
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hostRequest == null)
            {
                return NotFound();
            }

            return View(hostRequest);
        }

        // POST: HostRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.HostRequest == null)
            {
                return Problem("Entity set 'ApplicationDbContext.HostRequest'  is null.");
            }
            var hostRequest = await _context.HostRequest.FindAsync(id);
            if (hostRequest != null)
            {
                _context.HostRequest.Remove(hostRequest);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HostRequestExists(int id)
        {
          return _context.HostRequest.Any(e => e.Id == id);
        }
    }
}
