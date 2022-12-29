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
    public class ClubsController : Controller
    {
        private readonly ApplicationDbContext _context;


        public ClubsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Clubs

        public async Task<IActionResult> Index()
        {
            if(Super_UserController.currentUser == "System_Admin")
                return View(await _context.Club.ToListAsync());
            TempData["alertMessage"] = "You can not access this page.";
            return RedirectToAction("Index", "Home");
        }

        // GET: Clubs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (Super_UserController.currentUser == "System_Admin")
            {
                if (id == null || _context.Club == null)
                {
                    return NotFound();
                }

                var club = await _context.Club
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (club == null)
                {
                    return NotFound();
                }

                return View(club);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Clubs/Create
        public IActionResult Create()
        {
            if(Super_UserController.currentUser == "System_Admin")
                return View();
            TempData["alertMessage"] = "You can not access this page.";
            return RedirectToAction("Index", "Home");
        }

        // POST: Clubs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,name,location")] Club club)
        {
            if (Super_UserController.currentUser == "System_Admin")
            {
                if (ModelState.IsValid)
                {
                    var outputSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@out", System.Data.SqlDbType.Bit) { Direction = System.Data.ParameterDirection.Output };
                    _context.Database.ExecuteSqlRaw("exec dbo.checkClubExists @club_name='" + club.name + "', @out={0} out", outputSQLParam);

                    bool output = false;

                    if (outputSQLParam.Value != DBNull.Value)
                    {
                        output = (bool)outputSQLParam.Value;
                    }

                    if (output)
                    {
                        TempData["alertMessage"] = "Club already exists!";
                        return View();
                    }
                    _context.Add(club);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(club);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Clubs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (Super_UserController.currentUser == "System_Admin")
            {
                if (id == null || _context.Club == null)
                {
                    return NotFound();
                }

                var club = await _context.Club.FindAsync(id);
                if (club == null)
                {
                    return NotFound();
                }
                return View(club);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Clubs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,location")] Club club)
        {
            if (Super_UserController.currentUser == "System_Admin")
            {
                if (id != club.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(club);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ClubExists(club.Id))
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
                return View(club);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Clubs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (Super_UserController.currentUser == "System_Admin")
            {
                if (id == null || _context.Club == null)
                {
                    return NotFound();
                }

                var club = await _context.Club
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (club == null)
                {
                    return NotFound();
                }

                return View(club);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Clubs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (Super_UserController.currentUser == "System_Admin")
            {
                _context.Database.ExecuteSqlRaw("exec dbo.deleteClubHelper '" + id + "';");
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        private bool ClubExists(int id)
        {
          return _context.Club.Any(e => e.Id == id);
        }
    }
}
