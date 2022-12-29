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
    public class MatchesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MatchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Matches
        public async Task<IActionResult> Index()
        {
            if(Super_UserController.currentUser == "Association_Manager")
                return View(await _context.Match.ToListAsync());
            TempData["alertMessage"] = "You can not access this page.";
            return RedirectToAction("Index", "Home");
        }

        // GET: Matches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (Super_UserController.currentUser == "Association_Manager")
            {
                if (id == null || _context.Match == null)
                {
                    return NotFound();
                }

                var match = await _context.Match
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (match == null)
                {
                    return NotFound();
                }

                return View(match);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Matches/Create
        public IActionResult Create()
        {
            if(Super_UserController.currentUser == "Association_Manager")
                return View();
            TempData["alertMessage"] = "You can not access this page.";
            return RedirectToAction("Index", "Home");
        }

        // POST: Matches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("starting_time,ending_time")] Match match, String host_club, String guest_club)
        {
            if (Super_UserController.currentUser == "Association_Manager")
            {
                var hostclubnameSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@host_name", host_club);
                var guestclubnameSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@guest_name", guest_club);
                var startingtimeSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@start_time", match.starting_time);
                var endingtimeSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@end_time", match.ending_time);
                var outputSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@out", System.Data.SqlDbType.Bit) { Direction = System.Data.ParameterDirection.Output };
                _context.Database.ExecuteSqlRaw("exec dbo.checkClubExists @club_name='" + host_club + "', @out={0} out", outputSQLParam);

                bool output = false;

                if (outputSQLParam.Value != DBNull.Value)
                {
                    output = (bool)outputSQLParam.Value;
                }

                if (!output)
                {
                    TempData["alertMessage"] = "Host Club does not exist!";
                    return View();
                }

                _context.Database.ExecuteSqlRaw("exec dbo.checkClubExists @club_name='" + guest_club + "', @out={0} out", outputSQLParam);

                output = false;
                if (outputSQLParam.Value != DBNull.Value)
                {
                    output = (bool)outputSQLParam.Value;
                }

                if (!output)
                {
                    TempData["alertMessage"] = "Guest Club does not exist!";
                    return View();
                }

                if (host_club == guest_club)
                {
                    TempData["alertMessage"] = "The club can not match himself!";
                    return View();
                }

                _context.Database.ExecuteSqlRaw("exec dbo.addNewMatch @host_name={0}, @guest_name={1}, @start_time={2}, @end_time={3}",
                    hostclubnameSQLParam, guestclubnameSQLParam, startingtimeSQLParam, endingtimeSQLParam);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Matches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (Super_UserController.currentUser == "Association_Manager")
            {
                if (id == null || _context.Match == null)
                {
                    return NotFound();
                }

                var match = await _context.Match.FindAsync(id);
                if (match == null)
                {
                    return NotFound();
                }
                return View(match);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Matches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,starting_time,ending_time,host_club,guest_club,stadium_id")] Match match)
        {
            if (Super_UserController.currentUser == "Association_Manager")
            {
                if (id != match.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(match);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!MatchExists(match.Id))
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
                return View(match);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Matches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (Super_UserController.currentUser == "Association_Manager")
            {
                if (id == null || _context.Match == null)
                {
                    return NotFound();
                }

                var match = await _context.Match
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (match == null)
                {
                    return NotFound();
                }

                return View(match);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Matches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (Super_UserController.currentUser == "Association_Manager")
            {
                _context.Database.ExecuteSqlRaw("exec dbo.deleteMatchHelper '" + id + "';");
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        private bool MatchExists(int id)
        {
          return _context.Match.Any(e => e.Id == id);
        }
    }
}