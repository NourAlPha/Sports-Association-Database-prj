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
    public class Ticket_Buying_TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Ticket_Buying_TransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Ticket_Buying_Transactions
        public async Task<IActionResult> Index()
        {
            if(Super_UserController.currentUser == "")
                return View(await _context.Ticket_Buying_Transactions.ToListAsync());
            TempData["alertMessage"] = "You can not access this page.";
            return RedirectToAction("Index", "Home");
        }

        // GET: Ticket_Buying_Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (Super_UserController.currentUser == "")
            {
                if (id == null || _context.Ticket_Buying_Transactions == null)
                {
                    return NotFound();
                }

                var ticket_Buying_Transactions = await _context.Ticket_Buying_Transactions
                    .FirstOrDefaultAsync(m => m.ticket_id == id);
                if (ticket_Buying_Transactions == null)
                {
                    return NotFound();
                }

                return View(ticket_Buying_Transactions);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Ticket_Buying_Transactions/Create
        public IActionResult Create()
        {
            if(Super_UserController.currentUser == "")
                return View();
            TempData["alertMessage"] = "You can not access this page.";
            return RedirectToAction("Index", "Home");
        }

        // POST: Ticket_Buying_Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("fan_id,ticket_id")] Ticket_Buying_Transactions ticket_Buying_Transactions)
        {
            if (Super_UserController.currentUser == "")
            {
                if (ModelState.IsValid)
                {
                    _context.Add(ticket_Buying_Transactions);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(ticket_Buying_Transactions);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Ticket_Buying_Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (Super_UserController.currentUser == "")
            {
                if (id == null || _context.Ticket_Buying_Transactions == null)
                {
                    return NotFound();
                }

                var ticket_Buying_Transactions = await _context.Ticket_Buying_Transactions.FindAsync(id);
                if (ticket_Buying_Transactions == null)
                {
                    return NotFound();
                }
                return View(ticket_Buying_Transactions);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Ticket_Buying_Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("fan_id,ticket_id")] Ticket_Buying_Transactions ticket_Buying_Transactions)
        {
            if (Super_UserController.currentUser == "")
            {
                if (id != ticket_Buying_Transactions.ticket_id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(ticket_Buying_Transactions);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!Ticket_Buying_TransactionsExists(ticket_Buying_Transactions.ticket_id))
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
                return View(ticket_Buying_Transactions);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Ticket_Buying_Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (Super_UserController.currentUser == "")
            {
                if (id == null || _context.Ticket_Buying_Transactions == null)
                {
                    return NotFound();
                }

                var ticket_Buying_Transactions = await _context.Ticket_Buying_Transactions
                    .FirstOrDefaultAsync(m => m.ticket_id == id);
                if (ticket_Buying_Transactions == null)
                {
                    return NotFound();
                }

                return View(ticket_Buying_Transactions);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Ticket_Buying_Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (Super_UserController.currentUser == "")
            {
                if (_context.Ticket_Buying_Transactions == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Ticket_Buying_Transactions'  is null.");
                }
                var ticket_Buying_Transactions = await _context.Ticket_Buying_Transactions.FindAsync(id);
                if (ticket_Buying_Transactions != null)
                {
                    _context.Ticket_Buying_Transactions.Remove(ticket_Buying_Transactions);
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

        private bool Ticket_Buying_TransactionsExists(int id)
        {
          return _context.Ticket_Buying_Transactions.Any(e => e.ticket_id == id);
        }
    }
}
