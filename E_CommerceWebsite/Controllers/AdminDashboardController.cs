using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using E_CommerceWebsite.Models.Repository;
using E_CommerceWebsite.Models;
using Serilog;

namespace E_CommerceWebsite.Controllers
{
    /// <summary>
    /// Controller responsible for admin dashboard functionalities such as managing products and users.
    /// Access restricted to Admin role.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {

       

        private readonly ProductRepository _productRepository;
        private readonly UserRepository _userRepository;


        /// <summary>
        /// Constructor for injecting required repositories.
        /// </summary>
        /// <param name="productRepository">Repository for product operations</param>
        /// <param name="userRepository">Repository for user operations</param>
        public AdminDashboardController(ProductRepository productRepository, UserRepository userRepository)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
        }
        /// <summary>
        /// Displays the admin dashboard with a list of all products.
        /// </summary>
        /// <returns>Product list view</returns>
        public IActionResult Index()
        {
            var userName = HttpContext.Session.GetString("UserName");
            ViewData["UserName"] = userName;

            List<Product> products = _productRepository.GetAllProduct();
            return View(products);
        }
        /// <summary>
        /// Displays the form to add a new product.
        /// </summary>
        /// <returns>Add product view</returns>
        public IActionResult AddProduct()
        {
            return View();
        }

        /// <summary>
        /// Handles POST request to add a new product.
        /// </summary>
        /// <param name="product">Product object containing details</param>
        /// <returns>Redirects to product list or displays error</returns>

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
                
                //Console.WriteLine($"Error: {ex.Message}");
                Log.Error(ex, "Error in AddProduct (POST) at {Time}", DateTime.Now);

                ViewBag.Error = "An unexpected error occurred while adding the product. Please try again.";
                return View(product);
            }
        }

        /// <summary>
        /// Displays the edit form for a selected product by ID.
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Edit product view</returns>
        public IActionResult EditProduct(int id)
        {
            var product = _productRepository.GetProductById(id);
            if (product == null)
            {
                Log.Warning("EditProduct GET: Product not found for ID {Id}", id);
                return NotFound();
            }
            return View(product);
        }


        /// <summary>
        /// Handles POST request to update a product.
        /// </summary>
        /// <param name="product">Updated product details</param>
        /// <returns>Redirects to product list or displays error</returns>
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

                //Console.WriteLine($"Error: {ex.Message}");
                Log.Error(ex, "Error occurred while updating product with ID {Id}", product.ProductId);

                ViewBag.Error = "An unexpected error occurred while updating the product. Please try again.";
                return View(product);
            }
        }


        /// <summary>
        /// Deletes a product by its ID.
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Redirects to product list</returns>
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
                //Console.WriteLine($"Error: {ex.Message}");
                Log.Error(ex, "Error occurred while deleting product with ID {Id}", id);
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the product. Please try again.";
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Displays a list of all registered users.
        /// </summary>
        /// <returns>User list view</returns>
        public IActionResult UserList()
        {
            List<User> users = _userRepository.GetAllUsers();
            return View(users);
        }

        /// <summary>
        /// Deletes a user by their ID.
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Redirects to user list</returns>
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
                //Console.WriteLine($"Error: {ex.Message}");
                Log.Error(ex, "Error occurred while deleting user with ID {Id}", id);
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the user. Please try again.";
            }
            return RedirectToAction("UserList");
        }
        /// <summary>
        /// Displays the Add Admin form.
        /// </summary>
        /// <returns>Returns the AddAdmin view.</returns>

        [HttpGet]
        public IActionResult AddAdmin()
        {
            return View();
        }

        /// <summary>
        /// Handles the POST request to add a new Admin user.
        /// </summary>
        /// <param name="user">The User object containing admin details from the form.</param>
        /// <returns>
        /// If successful, redirects to the Index action with a success message;  
        /// otherwise, redisplays the form with error message.
        /// </returns>

        [HttpPost]
        public IActionResult AddAdmin(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    user.Role = "Admin"; 
                    bool isRegistered = _userRepository.RegisterUser(user);
                    if (isRegistered)
                    {
                        TempData["SuccessMessage"] = "Admin added successfully!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to add admin. Try again.";
                    }
                }
                return View(user);
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Error: {ex.Message}");
                Log.Error(ex, "Error occurred while adding admin for email: {Email}", user.Email);
                TempData["ErrorMessage"] = "An unexpected error occurred while adding the admin. Please try again.";
                return View(user);
            }
        }

    }
}
