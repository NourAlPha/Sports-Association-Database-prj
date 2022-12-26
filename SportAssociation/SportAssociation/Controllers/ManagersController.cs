using System;
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
    public class ManagersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManagersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Managers
        public async Task<IActionResult> Index()
        {
              return View(await _context.Manager.ToListAsync());
        }

        // GET: Managers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Manager == null)
            {
                return NotFound();
            }

            var manager = await _context.Manager
                .FirstOrDefaultAsync(m => m.id == id);
            if (manager == null)
            {
                return NotFound();
            }

            return View(manager);
        }

        // GET: Managers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Managers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("name,username")] Manager manager, String StadiumName, String Password)
        {
            var nameSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@name", manager.name);
            var usernameSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@username", manager.username);
            var passwordSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@password", Password);
            var stadiumnameSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@stadium_name", StadiumName);
            var outputSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@out", System.Data.SqlDbType.Bit) { Direction = System.Data.ParameterDirection.Output };
            _context.Database.ExecuteSqlRaw("exec dbo.checkStadiumExists @stadium_name='" + StadiumName + "', @out={0} out", outputSQLParam);

            bool output = false;

            if (outputSQLParam.Value != DBNull.Value)
            {
                output = (bool)outputSQLParam.Value;
            }

            if (!output)
            {
                TempData["alertMessage"] = "Stadium does not exist!";
                return View();
            }

            _context.Database.ExecuteSqlRaw("exec dbo.checkUsernameExists @username='" + manager.username +"', @out={0} out", outputSQLParam);

            output = false;
            if (outputSQLParam.Value != DBNull.Value)
            {
                output = (bool)outputSQLParam.Value;
            }

            if(output)
            {
                TempData["alertMessage"] = "Username is used!";
                return View();
            }

            _context.Database.ExecuteSqlRaw("exec dbo.stadiumHasManager @stadium_name='" + StadiumName + "', @out={0} out", outputSQLParam);

            output = false;
            if (outputSQLParam.Value != DBNull.Value)
            {
                output = (bool)outputSQLParam.Value;
            }

            if (output)
            {
                TempData["alertMessage"] = "This stadium has a manager!";
                return View();
            }


            _context.Database.ExecuteSqlRaw("exec dbo.addStadiumManager @name={0}, @stadium_name={1}, @username={2}, @password={3}",
                nameSQLParam, stadiumnameSQLParam, usernameSQLParam, passwordSQLParam);
            Authentication.isAuthenticated = true;
            Authentication.username = manager.username;
            Super_UserController.currentUser = "Manager";
            return RedirectToAction(nameof(Index));
        }

        // GET: Managers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Manager == null)
            {
                return NotFound();
            }

            var manager = await _context.Manager.FindAsync(id);
            if (manager == null)
            {
                return NotFound();
            }
            return View(manager);
        }

        // POST: Managers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,name,username,stadium_id")] Manager manager)
        {
            if (id != manager.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(manager);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ManagerExists(manager.id))
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
            return View(manager);
        }

        // GET: Managers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Manager == null)
            {
                return NotFound();
            }

            var manager = await _context.Manager
                .FirstOrDefaultAsync(m => m.id == id);
            if (manager == null)
            {
                return NotFound();
            }

            return View(manager);
        }

        // POST: Managers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Manager == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Manager'  is null.");
            }
            var manager = await _context.Manager.FindAsync(id);
            if (manager != null)
            {
                _context.Manager.Remove(manager);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> viewStadiumInformation()
        {
            string connectionstring = "Server=(localdb)\\mssqllocaldb;Database=Proj;Trusted_Connection=True;MultipleActiveResultSets=true";

            var result = new object();
            DataTable dataTable = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionstring))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("select s.name, s.capacity, s.location, s.status from Stadium s, Manager m where m.stadium_id = s.id and m.username='" + Authentication.username + "';", conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataTable);
                conn.Close();
                da.Dispose();
            }

            return View(dataTable);

        }

        private bool ManagerExists(int id)
        {
          return _context.Manager.Any(e => e.id == id);
        }
    }
}
