using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using E_CommerceWebsite.Models;
using E_CommerceWebsite.Models.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Humanizer;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;

namespace E_CommerceWebsite.Controllers
{
    public class UserController : Controller
    {
       
        private readonly UserRepository _userRepository;

        public UserController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isRegistered = _userRepository.RegisterUser(user);
                    if (isRegistered)
                    {
                        TempData["SuccessMessage"] = "Registration successful!";
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Registration failed. Try again.";
                    }
                }
                return View(user);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                // Log the exception details (e.g., using a logger)
                Console.WriteLine($"Error: {ex.Message}");
                return View(user);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password, string? returnUrl=null)
        {
            try
            {
                var user = _userRepository.AuthenticateUser(email, password);
                if (user != null) 
                {
//  This code is used for user authentication and authorization using cookies:

//Claims – Stores user-specific data like name, email, and role.
//ClaimsIdentity – Represents the user's identity based on the claims.
//ClaimsPrincipal – Represents the authenticated user(identity +claims).
//SignInAsync – Signs in the user by creating an authentication cookie, which allows the user to stay logged in across requests.

                    var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    HttpContext.Session.SetString("UserName", user.Name);
                    HttpContext.Session.SetString("UserEmail", user.Email);
                    HttpContext.Session.SetString("UserRole", user.Role);

                    /// Redirect to the returnUrl if it's not null, otherwise redirect based on role
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    if (user.Role == "Admin")
                    {
                        return RedirectToAction("Index", "AdminDashboard");
                    }

                    else
                    {
                        return RedirectToAction("Index", "UserDashboard");
                    }
                }
                else
                {
                    ViewData["ErrorMessage"] = "Invalid email or password.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}"); 
                ViewData["ErrorMessage"] = "An error occurred while processing your request.";
                return View();
            }
        }

        //public IActionResult Logout()
        //{
        //    HttpContext.Session.Clear(); 
        //    return RedirectToAction("Index", "Home");
        //}

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear(); // Clear session data
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); // Sign out user
            return RedirectToAction("Index", "Home");
        }




    }
}
