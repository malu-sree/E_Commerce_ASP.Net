using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using E_CommerceWebsite.Models;
using E_CommerceWebsite.Models.Repository;
using Serilog;

namespace E_CommerceWebsite.Controllers
{
    /// <summary>
    /// Controller responsible for managing cart operations.
    /// </summary>
    public class CartController : Controller
    {
        private readonly CartRepository _cartRepository;

        /// <summary>
        /// Constructor to initialize CartController with dependency injection.
        /// </summary>
        /// <param name="cartRepository">Repository for cart operations</param>
        public CartController(CartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        /// <summary>
        /// Adds a product to the user's cart.
        /// </summary>
        /// <param name="productId">ID of the product to be added</param>
        /// <param name="quantity">Quantity of the product</param>
        /// <returns>Redirects to Home page with success or error message</returns>
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            try
            {
               
                int userId = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));

                Cart cart = new Cart
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity
                };

                bool isAdded = _cartRepository.AddToCart(cart);

                if (isAdded)
                {
                    TempData["SuccessMessage"] = "Product added to cart successfully!";
                    return RedirectToAction("Index", "Home");
                }

                TempData["ErrorMessage"] = "Failed to add product to cart.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
               
                TempData["ErrorMessage"] = "An error occurred while adding the product to the cart.";
                //Console.WriteLine($"Error: {ex.Message}");
                Log.Error(ex, "Error occurred while adding product ID {ProductId} to cart for user ID {UserId}", productId, HttpContext.Session.GetInt32("UserId"));
                return RedirectToAction("Index", "Home");
            }
        }
        /// <summary>
        /// Displays all items in the current user's cart.
        /// </summary>
        /// <returns>View showing list of cart items and total price</returns>
        public IActionResult Index()
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
            var cartItems = _cartRepository.GetCartItems(userId);
            decimal totalCartPrice = cartItems.Sum(item => item.TotalPrice);

            ViewBag.TotalCartPrice = totalCartPrice;
            return View(cartItems);
        }

        /// <summary>
        /// Removes an item from the user's cart by cart ID.
        /// </summary>
        /// <param name="cartId">ID of the cart item to be removed</param>
        /// <returns>Redirects to the cart index page</returns>
        public IActionResult RemoveFromCart(int cartId)
        {
            _cartRepository.RemoveCartItem(cartId);
            return RedirectToAction("Index");
        }
       

    }
}
