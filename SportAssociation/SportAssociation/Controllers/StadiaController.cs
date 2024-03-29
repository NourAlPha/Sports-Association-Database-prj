﻿using System;
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
    public class StadiaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StadiaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Stadia
        public async Task<IActionResult> Index()
        {
            if(Super_UserController.currentUser == "System_Admin")
                return View(await _context.Stadium.ToListAsync());
            TempData["alertMessage"] = "You can not access this page.";
            return RedirectToAction("Index", "Home");
        }

        // GET: Stadia/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (Super_UserController.currentUser == "System_Admin")
            {
                if (id == null || _context.Stadium == null)
                {
                    return NotFound();
                }

                var stadium = await _context.Stadium
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (stadium == null)
                {
                    return NotFound();
                }

                return View(stadium);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Stadia/Create
        public IActionResult Create()
        {
            if(Super_UserController.currentUser == "System_Admin")
                return View();
            TempData["alertMessage"] = "You can not access this page.";
            return RedirectToAction("Index", "Home");
        }

        // POST: Stadia/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Capacity,Location,Status")] Stadium stadium)
        {
            if (Super_UserController.currentUser == "System_Admin")
            {
                if (ModelState.IsValid)
                {
                    var outputSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@out", System.Data.SqlDbType.Bit) { Direction = System.Data.ParameterDirection.Output };
                    _context.Database.ExecuteSqlRaw("exec dbo.checkStadiumExists @stadium_name='" + stadium.Name + "', @out={0} out", outputSQLParam);

                    bool output = false;

                    if (outputSQLParam.Value != DBNull.Value)
                    {
                        output = (bool)outputSQLParam.Value;
                    }

                    if (output)
                    {
                        TempData["alertMessage"] = "Stadium already exists!";
                        return View();
                    }

                    _context.Add(stadium);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(stadium);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Stadia/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (Super_UserController.currentUser == "System_Admin")
            {
                if (id == null || _context.Stadium == null)
                {
                    return NotFound();
                }

                var stadium = await _context.Stadium.FindAsync(id);
                if (stadium == null)
                {
                    return NotFound();
                }
                return View(stadium);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Stadia/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Capacity,Location,Status")] Stadium stadium)
        {
            if (Super_UserController.currentUser == "System_Admin")
            {
                if (id != stadium.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(stadium);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!StadiumExists(stadium.Id))
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
                return View(stadium);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Stadia/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (Super_UserController.currentUser == "System_Admin")
            {
                if (id == null || _context.Stadium == null)
                {
                    return NotFound();
                }

                var stadium = await _context.Stadium
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (stadium == null)
                {
                    return NotFound();
                }

                return View(stadium);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Stadia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (Super_UserController.currentUser == "System_Admin")
            {
                _context.Database.ExecuteSqlRaw("exec dbo.deleteStadiumHelper '" + id + "';");
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        private bool StadiumExists(int id)
        {
          return _context.Stadium.Any(e => e.Id == id);
        }
    }
}
