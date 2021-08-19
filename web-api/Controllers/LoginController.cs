using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using a2.admin.Models;


namespace a2.admin.Controllers
{

    [Route("/admin/SecureLogin")]
    public class LoginController : Controller
    {

        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string loginID, string password)
        {
            
            if (loginID != "admin" && password != "admin")
            {
                ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
                return View(new Login { LoginName = loginID });
            }

            HttpContext.Session.SetString("Name", "admin");
            return RedirectToAction("Index", "Customer");
        }

        [Route("LogoutNow")]
        public IActionResult Logout()
        {
            // Logout customer.
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }
    }
}
