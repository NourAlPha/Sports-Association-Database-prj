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
    public class RepresentativesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RepresentativesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Representatives
        public async Task<IActionResult> Index()
        {
              return View(await _context.Representative.ToListAsync());
        }

        // GET: Representatives/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Representative == null)
            {
                return NotFound();
            }

            var representative = await _context.Representative
                .FirstOrDefaultAsync(m => m.Id == id);
            if (representative == null)
            {
                return NotFound();
            }

            return View(representative);
        }

        // GET: Representatives/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Representatives/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,Name")] Representative representative, String Password, String ClubName)
        {
            var nameSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@name", representative.Name);
            var usernameSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@representative_username", representative.Username);
            var passwordSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@password", Password);
            var clubnameSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@club_name", ClubName);
            _context.Database.ExecuteSqlRaw("exec dbo.addRepresentative @name={0}, @club_name={1}, @representative_username={2}, @password={3}",
                nameSQLParam, clubnameSQLParam, usernameSQLParam, passwordSQLParam);
            Authentication.isAuthenticated = true;
            Authentication.username = representative.Username;
            return RedirectToAction(nameof(Index));
        }

        // GET: Representatives/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Representative == null)
            {
                return NotFound();
            }

            var representative = await _context.Representative.FindAsync(id);
            if (representative == null)
            {
                return NotFound();
            }
            return View(representative);
        }

        // POST: Representatives/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Name,club_id")] Representative representative)
        {
            if (id != representative.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(representative);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RepresentativeExists(representative.Id))
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
            return View(representative);
        }

        // GET: Representatives/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Representative == null)
            {
                return NotFound();
            }

            var representative = await _context.Representative
                .FirstOrDefaultAsync(m => m.Id == id);
            if (representative == null)
            {
                return NotFound();
            }

            return View(representative);
        }

        // POST: Representatives/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Representative == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Representative'  is null.");
            }
            var representative = await _context.Representative.FindAsync(id);
            if (representative != null)
            {
                _context.Representative.Remove(representative);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RepresentativeExists(int id)
        {
          return _context.Representative.Any(e => e.Id == id);
        }
    }
}
