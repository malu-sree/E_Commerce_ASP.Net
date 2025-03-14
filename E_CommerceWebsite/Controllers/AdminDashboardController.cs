using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace E_CommerceWebsite.Controllers
{
    public class AdminDashboardController : Controller
    {

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var userName = HttpContext.Session.GetString("UserName");
            ViewData["UserName"] = userName;
            return View();
        }

    }
}
