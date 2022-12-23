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
    public class Association_ManagerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Association_ManagerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Association_Manager
        public async Task<IActionResult> Index()
        {
              return View(await _context.Association_Manager.ToListAsync());
        }

        // GET: Association_Manager/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Association_Manager == null)
            {
                return NotFound();
            }

            var association_Manager = await _context.Association_Manager
                .FirstOrDefaultAsync(m => m.Id == id);
            if (association_Manager == null)
            {
                return NotFound();
            }

            return View(association_Manager);
        }

        // GET: Association_Manager/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Association_Manager/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Username")] Association_Manager association_Manager, String Password)
        {
            var nameSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@name", association_Manager.Name);
            var usernameSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@username", association_Manager.Username);
            var passwordSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@password", Password);
            _context.Database.ExecuteSqlRaw("exec dbo.addAssociationManager @name={0}, @user_name={1}, @password={2}",
                nameSQLParam, usernameSQLParam, passwordSQLParam);
            Authentication.isAuthenticated = true;
            Authentication.username = association_Manager.Username;
            return RedirectToAction(nameof(Index));
        }

        // GET: Association_Manager/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Association_Manager == null)
            {
                return NotFound();
            }

            var association_Manager = await _context.Association_Manager.FindAsync(id);
            if (association_Manager == null)
            {
                return NotFound();
            }
            return View(association_Manager);
        }

        // POST: Association_Manager/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Username")] Association_Manager association_Manager)
        {
            if (id != association_Manager.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(association_Manager);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Association_ManagerExists(association_Manager.Id))
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
            return View(association_Manager);
        }

        // GET: Association_Manager/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Association_Manager == null)
            {
                return NotFound();
            }

            var association_Manager = await _context.Association_Manager
                .FirstOrDefaultAsync(m => m.Id == id);
            if (association_Manager == null)
            {
                return NotFound();
            }

            return View(association_Manager);
        }

        // POST: Association_Manager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Association_Manager == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Association_Manager'  is null.");
            }
            var association_Manager = await _context.Association_Manager.FindAsync(id);
            if (association_Manager != null)
            {
                _context.Association_Manager.Remove(association_Manager);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ViewAllUpcomingMatches()
        {
            
            
        }

        private bool Association_ManagerExists(int id)
        {
          return _context.Association_Manager.Any(e => e.Id == id);
        }
    }
}
