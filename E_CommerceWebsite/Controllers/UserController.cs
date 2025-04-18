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
using Serilog;

namespace E_CommerceWebsite.Controllers
{
    /// <summary>
    /// Handles user-related actions such as registration, login, and logout.
    /// </summary>
    public class UserController : Controller
    {
       
        private readonly UserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userRepository">Instance of <see cref="UserRepository"/> to manage user data.</param>
        public UserController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Displays the registration form.
        /// </summary>
        /// <returns>The registration view.</returns>
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Handles user registration logic.
        /// </summary>
        /// <param name="user">User object containing registration information.</param>
        /// <returns>Redirects to login on success; redisplays form with error messages on failure.</returns>

        [HttpPost]
        public IActionResult Register(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    if (string.IsNullOrEmpty(user.Role))
                    {
                        user.Role = "User"; // Default role
                    }
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
                //Console.WriteLine($"Error: {ex.Message}");
                Log.Error(ex, "Error occurred during registration for email: {Email}", user.Email);
                return View(user);
            }
        }
        /// <summary>
        /// Displays the login form.
        /// </summary>
        /// <returns>The login view.</returns>
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Handles user login and sets up authentication using cookies.
        /// </summary>
        /// <param name="email">User email.</param>
        /// <param name="password">User password.</param>
        /// <param name="returnUrl">Optional return URL for redirection after login.</param>
        /// <returns>Redirects to appropriate dashboard or redisplays login on failure.</returns>
        [HttpPost]
        public IActionResult Login(string email, string password, string? returnUrl=null)
        {
            try
            {
                var user = _userRepository.AuthenticateUser(email, password);
                if (user != null) 
                {
/// This code is used for user authentication and authorization using cookies:

///Claims – Stores user-specific data like name, email, and role.
///ClaimsIdentity – Represents the user's identity based on the claims.
///ClaimsPrincipal – Represents the authenticated user(identity +claims).
///SignInAsync – Signs in the user by creating an authentication cookie, which allows the user to stay logged in across requests.

                    var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    HttpContext.Session.SetInt32("UserId", user.Id);
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
                //Console.WriteLine($"Error: {ex.Message}"); 
                Log.Error(ex, "Error occurred during login for email: {Email}", email);
                ViewData["ErrorMessage"] = "An error occurred while processing your request.";
                return View();
            }
        }


        /// <summary>
        /// Logs out the user and clears session and authentication cookies.
        /// </summary>
        /// <returns>Redirects to the home page.</returns>
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); 
            return RedirectToAction("Index", "Home");
        }
        /// <summary>
        /// Checks if the given email is already registered in the system.
        /// </summary>
        /// <param name="email">The email address to check.</param>
        /// <returns>
        /// A JSON result indicating whether the email is available (true) or already taken (false).
        /// </returns>

        [HttpGet]
        public JsonResult IsEmailAvailable(string email)
        {
            bool isTaken = _userRepository.IsEmailExists(email);
            return Json(!isTaken); 
        }


    }
}
