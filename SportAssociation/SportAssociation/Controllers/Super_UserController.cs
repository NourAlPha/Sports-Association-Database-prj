using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportAssociation.Data;
using SportAssociation.Models;

namespace SportAssociation.Controllers
{
    public class Super_UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Super_UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Super_User/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Super_User/Login
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Username,Password")] Super_User super_User)
        {
            var usernameSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@username", super_User.Username);
            var passwordSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@password", super_User.Password);
            var outputSQLParam = new Microsoft.Data.SqlClient.SqlParameter("@out", System.Data.SqlDbType.VarChar, 20) { Direction = System.Data.ParameterDirection.Output };
            _context.Database.ExecuteSqlRaw("exec dbo.checkRole @username={0}, @password={1}, @out={2} out",
                usernameSQLParam, passwordSQLParam, outputSQLParam);

            string output = "Balabizo";

            if(outputSQLParam.Value != DBNull.Value)
            {
                output = outputSQLParam.Value.ToString();
            }

            if(output == "Balabizo")
            {
                return RedirectToAction("Login");
            }
            else if(output == "Fan")
            {
                //TODO blocked fan can't login.
                Authentication.isAuthenticated = true;
                Authentication.username = super_User.Username;
                return RedirectToAction("Index", "fans");
            }else if(output == "Manager")
            {
                Authentication.isAuthenticated = true;
                Authentication.username = super_User.Username;
                return RedirectToAction("Index", "managers");
            }
            else if(output == "Representative")
            {
                Authentication.isAuthenticated = true;
                Authentication.username = super_User.Username;
                return RedirectToAction("Index", "representatives");
            }
            else if(output == "System_Admin")
            {
                Authentication.isAuthenticated = true;
                Authentication.username = super_User.Username;
                return RedirectToAction("Index", "System_Admin");
            }
            else
            {
                Authentication.isAuthenticated = true;
                Authentication.username = super_User.Username;
                return RedirectToAction("Index", "Association_Manager");
            }

        }

        public async Task<IActionResult> Logout()
        {
            Authentication.isAuthenticated = false;
            return RedirectToAction("Index", "Home");
        }

        private bool Super_UserExists(string id)
        {
          return _context.Super_User.Any(e => e.Username == id);
        }
    }
}
