using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using a2.admin.Models;

namespace a2.admin.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index() => View();
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