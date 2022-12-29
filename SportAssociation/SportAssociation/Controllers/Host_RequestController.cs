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
    public class Host_RequestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Host_RequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Host_Request
        public async Task<IActionResult> Index()
        {
            if (Super_UserController.currentUser == "Manager")
            {
                var outputSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@out", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output };
                _context.Database.ExecuteSqlRaw("exec dbo.getManagerID3 @username='" + Authentication.username + "', @out={0} out",
                    outputSQLParam);

                int output = -1;

                if (outputSQLParam.Value != DBNull.Value)
                {
                    output = (int)outputSQLParam.Value;
                }

                ViewBag.currentManagerID = output;


                return View(await _context.Host_Request.ToListAsync());
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Host_Request/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (Super_UserController.currentUser == "Manager")
            {
                if (id == null || _context.Host_Request == null)
                {
                    return NotFound();
                }

                var host_Request = await _context.Host_Request
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (host_Request == null)
                {
                    return NotFound();
                }

                return View(host_Request);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Host_Request/Create
        public IActionResult Create()
        {
            if(Super_UserController.currentUser == "")
                return View();
            TempData["alertMessage"] = "You can not access this page.";
            return RedirectToAction("Index", "Home");
        }

        // POST: Host_Request/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,representative_id,manager_id,match_id,status")] Host_Request host_Request)
        {
            if (Super_UserController.currentUser == "")
            {
                if (ModelState.IsValid)
                {
                    _context.Add(host_Request);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(host_Request);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Host_Request/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (Super_UserController.currentUser == "Manager")
            {
                if (id == null || _context.Host_Request == null)
                {
                    return NotFound();
                }

                var host_Request = await _context.Host_Request.FindAsync(id);
                if (host_Request == null)
                {
                    return NotFound();
                }
                return View(host_Request);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Host_Request/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,representative_id,manager_id,match_id,status")] Host_Request host_Request)
        {
            if (Super_UserController.currentUser == "Manager")
            {
                if (id != host_Request.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(host_Request);
                        await _context.SaveChangesAsync();
                        if (host_Request.status == true)
                        {
                            _context.Database.ExecuteSqlRaw("delete from Host_Request where match_id = '" + host_Request.match_id + "' and status is null");
                            _context.Database.ExecuteSqlRaw("update Match set stadium_id = (SELECT stadium_id FROM Manager where id = '" + host_Request.manager_id + "') where id = '" + host_Request.match_id + "'");
                            var outputSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@out", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output };
                            _context.Database.ExecuteSqlRaw("exec dbo.getCapacity @manager_id='" + host_Request.manager_id + "', @out={0} out",
                                outputSQLParam);
                            int output = -1;
                            if (outputSQLParam.Value != DBNull.Value)
                            {
                                output = (int)outputSQLParam.Value;
                            }
                            for (int i = 0; i < output; i++)
                            {
                                _context.Database.ExecuteSqlRaw("Insert into Ticket values(1, " + host_Request.match_id + ")");
                            }
                        }
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!Host_RequestExists(host_Request.Id))
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
                return View(host_Request);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Host_Request/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (Super_UserController.currentUser == "Manager")
            {
                if (id == null || _context.Host_Request == null)
                {
                    return NotFound();
                }

                var host_Request = await _context.Host_Request
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (host_Request == null)
                {
                    return NotFound();
                }

                return View(host_Request);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Host_Request/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (Super_UserController.currentUser == "Manager")
            {
                if (_context.Host_Request == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Host_Request'  is null.");
                }
                var host_Request = await _context.Host_Request.FindAsync(id);
                if (host_Request != null)
                {
                    _context.Host_Request.Remove(host_Request);
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

        private bool Host_RequestExists(int id)
        {
          return _context.Host_Request.Any(e => e.Id == id);
        }
    }
}
