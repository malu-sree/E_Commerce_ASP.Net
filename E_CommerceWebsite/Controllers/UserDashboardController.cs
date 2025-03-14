using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceWebsite.Controllers
{
    public class UserDashboardController : Controller
    {
        [Authorize(Roles = "User")]
        public IActionResult Index()
        {
            var userName = HttpContext.Session.GetString("UserName");
            ViewData["UserName"] = userName;
            return View();
        }
    }
}
