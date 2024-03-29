﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SportAssociation.Data;
using SportAssociation.Models;

namespace SportAssociation.Controllers
{
    public class FansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FansController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Fans
        public async Task<IActionResult> Index()
        {
            if(Super_UserController.currentUser == "Fan")
                return View(await _context.Fan.ToListAsync());
            TempData["alertMessage"] = "You can not access this page.";
            return RedirectToAction("Index", "Home");
        }

        // GET: Fans/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (Super_UserController.currentUser == "System_Admin")
            {
                if (id == null || _context.Fan == null)
                {
                    return NotFound();
                }

                var fan = await _context.Fan
                    .FirstOrDefaultAsync(m => m.national_id == id);
                if (fan == null)
                {
                    return NotFound();
                }

                return View(fan);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Fans/Create
        public IActionResult Create()
        {
            if(Super_UserController.currentUser == "Balabizo")
                return View();
            TempData["alertMessage"] = "You can not access this page.";
            return RedirectToAction("Index", "Home");
        }

        // POST: Fans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("national_id,Name,birth_date,Address,phone_number,Username")] Fan fan, String Password)
        {
            if (Super_UserController.currentUser == "Balabizo")
            {
                var nameSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@name", fan.Name);
                var usernameSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@username", fan.Username);
                var passwordSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@password", Password);
                var nationalidSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@national_id", fan.national_id);
                var phonenumberSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@phone_num", fan.phone_number);
                var birthdateSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@birth_date", fan.birth_date);
                var addressSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@address", fan.Address);
                var outputSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@out", System.Data.SqlDbType.Bit) { Direction = System.Data.ParameterDirection.Output };
                _context.Database.ExecuteSqlRaw("exec dbo.fanAlreadyExists @national_id='" + fan.national_id + "', @out={0} out;", outputSQLParam);

                bool output = false;
                if (outputSQLParam.Value != DBNull.Value)
                {
                    output = (bool)outputSQLParam.Value;
                }
                if (output)
                {
                    TempData["alertMessage"] = "Fan already exists!";
                    return View();
                }

                _context.Database.ExecuteSqlRaw("exec dbo.checkUsernameExists @username='" + fan.Username + "', @out={0} out", outputSQLParam);

                output = false;
                if (outputSQLParam.Value != DBNull.Value)
                {
                    output = (bool)outputSQLParam.Value;
                }

                if (output)
                {
                    TempData["alertMessage"] = "Username is used!";
                    return View();
                }

                _context.Database.ExecuteSqlRaw("exec dbo.addFan @name={0}, @username={1}, @password={2}, @national_id={3}, @birth_date={4}, @address={5}, @phone_num={6}",
                    nameSQLParam, usernameSQLParam, passwordSQLParam, nationalidSQLParam, birthdateSQLParam, addressSQLParam, phonenumberSQLParam);
                Authentication.isAuthenticated = true;
                Authentication.username = fan.Username;
                Super_UserController.currentUser = "Fan";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Fans/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (Super_UserController.currentUser == "System_Admin")
            {
                if (id == null || _context.Fan == null)
                {
                    return NotFound();
                }

                var fan = await _context.Fan.FindAsync(id);
                if (fan == null)
                {
                    return NotFound();
                }
                return View(fan);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Fans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("national_id,Name,birth_date,Address,phone_number,Status,Username")] Fan fan)
        {
            if (Super_UserController.currentUser == "System_Admin")
            {
                if (id != fan.national_id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(fan);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!FanExists(fan.national_id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(ManageFan));
                }
                return View(fan);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Fans/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (Super_UserController.currentUser == "System_Admin")
            {
                if (id == null || _context.Fan == null)
                {
                    return NotFound();
                }

                var fan = await _context.Fan
                    .FirstOrDefaultAsync(m => m.national_id == id);
                if (fan == null)
                {
                    return NotFound();
                }

                return View(fan);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Fans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (Super_UserController.currentUser == "System_Admin")
            {
                if (_context.Fan == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Fan'  is null.");
                }
                var fan = await _context.Fan.FindAsync(id);
                if (fan != null)
                {
                    _context.Fan.Remove(fan);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageFan));
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> viewAvailableMatches()
        {
            if(Super_UserController.currentUser == "Fan")
                return View();
            TempData["alertMessage"] = "You can not access this page.";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> viewAvailableMatches(DateTime date)
        {
            if (Super_UserController.currentUser == "Fan")
            {
                string connectionstring = "Server=(localdb)\\mssqllocaldb;Database=Proj;Trusted_Connection=True;MultipleActiveResultSets=true";

                var result = new object();
                DataTable dataTable = new DataTable();

                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    conn.Open();
                    string str = "select * from dbo.availableMatchesToAttend('" + date + "');";
                    SqlCommand cmd = new SqlCommand(str, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    // this will query your database and return the result to your datatable
                    da.Fill(dataTable);
                    conn.Close();
                    da.Dispose();
                }

                ViewBag.date = date;

                return View("viewAvailableMatchesList", dataTable);
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> PurchaseTicket(String HostClub, String GuestClub)
        {
            if (Super_UserController.currentUser == "Fan")
            {
                _context.Database.ExecuteSqlRaw("exec dbo.buyTicket @host_club='" + HostClub + "', @guest_club='" + GuestClub + "', @username='" + Authentication.username + "';");
                return View(nameof(Index));
            }
            else
            {
                TempData["alertMessage"] = "You can not access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> ManageFan()
        {
            if(Super_UserController.currentUser == "System_Admin")
                return View(await _context.Fan.ToListAsync());
            TempData["alertMessage"] = "You can not access this page.";
            return RedirectToAction("Index", "Home");
        }

        private bool FanExists(string id)
        {
          return _context.Fan.Any(e => e.national_id == id);
        }
    }
}
