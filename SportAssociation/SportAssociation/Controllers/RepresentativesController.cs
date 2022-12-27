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
            var outputSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@out", System.Data.SqlDbType.Bit) { Direction = System.Data.ParameterDirection.Output };
            _context.Database.ExecuteSqlRaw("exec dbo.checkClubExists @club_name='" + ClubName + "', @out={0} out;", outputSQLParam);

            bool output = false;
            if (outputSQLParam.Value != DBNull.Value)
            {
                output = (bool)outputSQLParam.Value;
            }
            if (!output)
            {
                TempData["alertMessage"] = "Club does not exist!";
                return View();
            }

            _context.Database.ExecuteSqlRaw("exec dbo.checkUsernameExists @username='" + representative.Username + "', @out={0} out", outputSQLParam);

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

            _context.Database.ExecuteSqlRaw("exec dbo.clubHasRepresentative @club_name='" + ClubName + "', @out={0} out", outputSQLParam);

            output = false;
            if (outputSQLParam.Value != DBNull.Value)
            {
                output = (bool)outputSQLParam.Value;
            }

            if (output)
            {
                TempData["alertMessage"] = "This club has a representative!";
                return View();
            }

            _context.Database.ExecuteSqlRaw("exec dbo.addRepresentative @name={0}, @club_name={1}, @representative_username={2}, @password={3}",
                nameSQLParam, clubnameSQLParam, usernameSQLParam, passwordSQLParam);
            Authentication.isAuthenticated = true;
            Authentication.username = representative.Username;
            Super_UserController.currentUser = "Representative";
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

        public async Task<IActionResult> viewAllInformation()
        {
            string connectionstring = "Server=(localdb)\\mssqllocaldb;Database=Proj;Trusted_Connection=True;MultipleActiveResultSets=true";

            var result = new object();
            DataTable dataTable = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionstring))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Select c.name, c.location from dbo.Club c, dbo.Representative r where r.club_id = c.id and r.username = '" + Authentication.username + "';", conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataTable);
                conn.Close();
                da.Dispose();
            }

            return View(dataTable);

        }

        public async Task<IActionResult> viewUpcomingMatches()
        {
            string connectionstring = "Server=(localdb)\\mssqllocaldb;Database=Proj;Trusted_Connection=True;MultipleActiveResultSets=true";

            var result = new object();
            DataTable dataTable = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionstring))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("select * from dbo.upcomingMatchesOfClub(dbo.getClubNameUsername('" + Authentication.username + "'))", conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataTable);
                conn.Close();
                da.Dispose();
            }

            return View(dataTable);

        }

        public async Task<IActionResult> viewAvailableStadiums()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> viewAvailableStadiums(DateTime date)
        {
            string connectionstring = "Server=(localdb)\\mssqllocaldb;Database=Proj;Trusted_Connection=True;MultipleActiveResultSets=true";

            var result = new object();
            DataTable dataTable = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionstring))
            {
                conn.Open();
                string str = "select * from dbo.viewAvailableStadiumsOn('" + date + "');";
                SqlCommand cmd = new SqlCommand(str, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataTable);
                conn.Close();
                da.Dispose();
            }

            ViewBag.date = date;

            var outputSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@out", System.Data.SqlDbType.Bit) { Direction = System.Data.ParameterDirection.Output };
            _context.Database.ExecuteSqlRaw("exec matchExists @username='" + Authentication.username + "', @date='" + date + "', @out={0} out", outputSQLParam);

            bool output = false;
            if (outputSQLParam.Value != DBNull.Value)
            {
                output = (bool)outputSQLParam.Value;
            }
            ViewBag.validDate = output;

            Dictionary<string, bool> validRequest = new Dictionary<string, bool>();

            foreach(DataRow row in dataTable.Rows)
            {
                _context.Database.ExecuteSqlRaw("exec validRequest @username='" + Authentication.username + "', @stadium_name='" + row[dataTable.Columns[0]] + "', @date='" + date + "', @out={0} out", outputSQLParam);
                output = false;
                if (outputSQLParam.Value != DBNull.Value)
                {
                    output = (bool)outputSQLParam.Value;
                }
                validRequest.Add((string)row[dataTable.Columns[0]], output);
            }
            ViewBag.validRequest = validRequest;
            

            return View("viewAvailableStadiumsList", dataTable);
        }

        public async Task<IActionResult> RequestHost(String StadiumName, DateTime date)
        {
            var stadiumnameSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@stadium_name", StadiumName);
            var dateSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@date_time", date);
            _context.Database.ExecuteSqlRaw("exec dbo.addHostRequestUsername @username='" + Authentication.username + "', @stadium_name={0}, @date_time={1}",
                stadiumnameSQLParam, dateSQLParam);
            return View(nameof(Index));
        }

        private bool RepresentativeExists(int id)
        {
          return _context.Representative.Any(e => e.Id == id);
        }
    }
}
