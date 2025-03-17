using E_CommerceWebsite.Models;
using E_CommerceWebsite.Models.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceWebsite.Controllers
{
    [Authorize(Roles = "User")]
    public class UserDashboardController : Controller
    {
        private readonly ProductRepository _productRepository;
        private readonly UserRepository _userRepository;

        public UserDashboardController(ProductRepository productRepository, UserRepository userRepository)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
        }

        public IActionResult Index()
        {
            var userName = HttpContext.Session.GetString("UserName");
            ViewData["UserName"] = userName;
            var products = _productRepository.GetAllProduct();
            return View(products);
        }

        public IActionResult Details(int id)
        {
            var product = _productRepository.GetProductById(id);
            return View(product);
        }

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
                    TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                    return View(user);
                }
            }
            return View(user);
        }


    }
}
