using E_CommerceWebsite.Models;
using E_CommerceWebsite.Models.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace E_CommerceWebsite.Controllers
{
    /// <summary>
    /// Controller for handling user dashboard-related operations.
    /// </summary>
    [Authorize(Roles = "User")]
    public class UserDashboardController : Controller
    {

        private readonly ProductRepository _productRepository;
        private readonly UserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDashboardController"/> class.
        /// </summary>
        /// <param name="productRepository">Instance of <see cref="ProductRepository"/> to manage product data.</param>
        /// <param name="userRepository">Instance of <see cref="UserRepository"/> to manage user data.</param>
        public UserDashboardController(ProductRepository productRepository, UserRepository userRepository)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
        }
        /// <summary>
        /// Displays the user dashboard with a list of products.
        /// </summary>
        /// <returns>The dashboard view.</returns>

        public IActionResult Index()
        {
            var userName = HttpContext.Session.GetString("UserName");
            ViewData["UserName"] = userName;
            var products = _productRepository.GetAllProduct();
            return View(products);
        }
        /// <summary>
        /// Displays the details of a selected product.
        /// </summary>
        /// <param name="id">Product ID.</param>
        /// <returns>The details view for the product.</returns>
        public IActionResult Details(int id)
        {
            var product = _productRepository.GetProductById(id);
            return View(product);
        }
        /// <summary>
        /// Displays the edit profile form for a user.
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <returns>The edit profile view if the user is found; otherwise, redirects to Index.</returns>
        public IActionResult EditProfile(int id)
        {
            var user = _userRepository.GetUserById(id);
            if (user != null)
            {
                return View(user);
            }
            TempData["ErrorMessage"] = "User not found!";
            return RedirectToAction("Index");
        }
        /// <summary>
        /// Handles the post request to update user profile information.
        /// </summary>
        /// <param name="user">The updated user model.</param>
        /// <returns>Redirects to Index on success, or redisplays the form on failure.</returns>
        [HttpPost]
        public IActionResult EditProfile(User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                   bool isUpdated= _userRepository.UpdateUser(user);
                    if (isUpdated)
                    {
                        TempData["SuccessMessage"] = "Profile updated successfully!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to update profile.";
                        return View(user);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error updating profile for user ID {UserId}", user.Id);
                    TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                    return View(user);
                }
            }
            Log.Warning("Invalid model state while editing profile for user ID {UserId}", user.Id);
            return View(user);
        }


    }
}
