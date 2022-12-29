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
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            if(Super_UserController.currentUser == "")
                return View(await _context.Ticket.ToListAsync());
            TempData["alertMessage"] = "You can not access this page.";
            return RedirectToAction("Index", "Home");
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (Super_UserController.currentUser == "")
            {
                if (id == null || _context.Ticket == null)
                {
                    return NotFound();
                }

                var ticket = await _context.Ticket
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (ticket == null)
                {
                    return NotFound();
                }

                return View(ticket);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            if(Super_UserController.currentUser == "")
                return View();
            TempData["alertMessage"] = "You can not access this page.";
            return RedirectToAction("Index", "Home");
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Status,match_id")] Ticket ticket)
        {
            if (Super_UserController.currentUser == "")
            {
                if (ModelState.IsValid)
                {
                    _context.Add(ticket);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(ticket);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (Super_UserController.currentUser == "")
            {
                if (id == null || _context.Ticket == null)
                {
                    return NotFound();
                }

                var ticket = await _context.Ticket.FindAsync(id);
                if (ticket == null)
                {
                    return NotFound();
                }
                return View(ticket);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Status,match_id")] Ticket ticket)
        {
            if (Super_UserController.currentUser == "")
            {
                if (id != ticket.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(ticket);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!TicketExists(ticket.Id))
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
                return View(ticket);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (Super_UserController.currentUser == "")
            {
                if (id == null || _context.Ticket == null)
                {
                    return NotFound();
                }

                var ticket = await _context.Ticket
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (ticket == null)
                {
                    return NotFound();
                }

                return View(ticket);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (Super_UserController.currentUser == "")
            {
                if (_context.Ticket == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Ticket'  is null.");
                }
                var ticket = await _context.Ticket.FindAsync(id);
                if (ticket != null)
                {
                    _context.Ticket.Remove(ticket);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        private bool TicketExists(int id)
        {
          return _context.Ticket.Any(e => e.Id == id);
        }
    }
}
