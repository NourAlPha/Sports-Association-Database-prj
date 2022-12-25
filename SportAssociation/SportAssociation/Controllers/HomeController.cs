using Microsoft.AspNetCore.Mvc;
using SportAssociation.Models;
using System.Diagnostics;

namespace SportAssociation.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if(Super_UserController.currentUser == "Balabizo")
                return View();
            else if(Super_UserController.currentUser == "Fan")
                return RedirectToAction("Index", "fans");
            else if(Super_UserController.currentUser == "Manager")
                return RedirectToAction("Index", "managers");
            else if(Super_UserController.currentUser == "Representative")
                return RedirectToAction("Index", "representatives");
            else if(Super_UserController.currentUser == "System_Admin")
                return RedirectToAction("Index", "System_Admin");
            else
                return RedirectToAction("Index", "Association_Manager");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}