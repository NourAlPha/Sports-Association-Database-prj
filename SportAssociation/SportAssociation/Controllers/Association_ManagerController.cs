﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
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
            var outputSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@out", System.Data.SqlDbType.Bit) { Direction = System.Data.ParameterDirection.Output };

            bool output = false;

            _context.Database.ExecuteSqlRaw("exec dbo.checkUsernameExists @username='" + association_Manager.Username + "', @out={0} out", outputSQLParam);

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

            _context.Database.ExecuteSqlRaw("exec dbo.addAssociationManager @name={0}, @user_name={1}, @password={2}",
                nameSQLParam, usernameSQLParam, passwordSQLParam);
            Authentication.isAuthenticated = true;
            Authentication.username = association_Manager.Username;
            Super_UserController.currentUser = "Association_Manager";
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
            string connectionstring = "Server=(localdb)\\mssqllocaldb;Database=Proj;Trusted_Connection=True;MultipleActiveResultSets=true";

            var result = new object();
            DataTable dataTable = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionstring))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Select dbo.getClubName(host_club) as Host_Club, dbo.getClubName(guest_club) as Guest_Club, starting_time, ending_time from Match where starting_time > current_timestamp", conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataTable);
                conn.Close();
                da.Dispose();
            }

            return View(dataTable);

        }
        public async Task<IActionResult> ViewAlreadyPlayedMatches()
        {
            string connectionstring = "Server=(localdb)\\mssqllocaldb;Database=Proj;Trusted_Connection=True;MultipleActiveResultSets=true";

            var result = new object();
            DataTable dataTable = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionstring))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Select dbo.getClubName(host_club) as Host_Club, dbo.getClubName(guest_club) as Guest_Club, starting_time, ending_time from Match where starting_time < current_timestamp", conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataTable);
                conn.Close();
                da.Dispose();
            }

            return View(dataTable);

        }

        public async Task<IActionResult> neverMatched()
        {
            string connectionstring = "Server=(localdb)\\mssqllocaldb;Database=Proj;Trusted_Connection=True;MultipleActiveResultSets=true";

            var result = new object();
            DataTable dataTable = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionstring))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Select * from dbo.clubsNeverMatched", conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataTable);
                conn.Close();
                da.Dispose();
            }

            return View(dataTable);

        }

        private bool Association_ManagerExists(int id)
        {
          return _context.Association_Manager.Any(e => e.Id == id);
        }
    }
}
