using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using E_CommerceWebsite.Models;
using E_CommerceWebsite.Models.Repository;

namespace E_CommerceWebsite.Controllers
{
    public class CartController : Controller
    {
        private readonly CartRepository _cartRepository;
        public CartController(CartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }
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
                Console.WriteLine($"Error: {ex.Message}");
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Index()
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
            var cartItems = _cartRepository.GetCartItems(userId);
            decimal totalCartPrice = cartItems.Sum(item => item.TotalPrice);

            ViewBag.TotalCartPrice = totalCartPrice;
            return View(cartItems);
        }

        public IActionResult RemoveFromCart(int cartId)
        {
            _cartRepository.RemoveCartItem(cartId);
            return RedirectToAction("Index");
        }

    }
}
