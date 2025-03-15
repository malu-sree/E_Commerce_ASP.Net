using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using E_CommerceWebsite.Models.Repository;
using E_CommerceWebsite.Models;

namespace E_CommerceWebsite.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {

       

        private readonly ProductRepository _productRepository;
        private readonly UserRepository _userRepository;
        public AdminDashboardController(ProductRepository productRepository, UserRepository userRepository)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
        }
        public IActionResult Index()
        {
            var userName = HttpContext.Session.GetString("UserName");
            ViewData["UserName"] = userName;

            List<Product> products = _productRepository.GetAllProduct();
            return View(products);
        }

        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        
        public IActionResult AddProduct(Product product)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(product);
                }

                bool isAdded = _productRepository.AddProduct(product);
                if (isAdded)
                {
                    TempData["SuccessMessage"] = "Product added successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "Failed to add product. Try again.";
                    return View(product);
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error: {ex.Message}");

             
                ViewBag.Error = "An unexpected error occurred while adding the product. Please try again.";
                return View(product);
            }
        }

        public IActionResult EditProduct(int id)
        {
            var product = _productRepository.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        public IActionResult EditProduct(Product product)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(product);
                }
                bool isUpdated = _productRepository.UpdateProduct(product);
                if (isUpdated)
                {
                    TempData["SuccessMessage"] = "Product updated successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "Failed to update product. Try again.";
                    return View(product);
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error: {ex.Message}");

                ViewBag.Error = "An unexpected error occurred while updating the product. Please try again.";
                return View(product);
            }
        }
        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            try
            {
                bool isDeleted = _productRepository.DeleteProduct(id);
                if (isDeleted)
                {
                    TempData["SuccessMessage"] = "Product deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete product. Try again.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the product. Please try again.";
            }
            return RedirectToAction("Index");
        }
        public IActionResult UserList()
        {
            List<User> users = _userRepository.GetAllUsers();
            return View(users);
        }

        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                bool isDeleted = _userRepository.DeleteUser(id);
                if (isDeleted)
                {
                    TempData["SuccessMessage"] = "User deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete user. Try again.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the user. Please try again.";
            }
            return RedirectToAction("UserList");
        }
    }
}
