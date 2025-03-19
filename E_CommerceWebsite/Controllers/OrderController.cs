using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using E_CommerceWebsite.Models;
using E_CommerceWebsite.Models.Repository;


namespace E_CommerceWebsite.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderRepository _orderRepository;

        public OrderController(OrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                // If user is not logged in, redirect to login
                return RedirectToAction("Login", "User");
            }

            var order = new Order
            {
                UserId = userId.Value
            };

            return View(order);
        }

        // Step 2: POST Place Order
        [HttpPost]
        public IActionResult PlaceOrder(Order order)
        {
            if (ModelState.IsValid)
            {
                var placedOrder = _orderRepository.PlaceOrder(order);

                if (placedOrder != null && placedOrder.OrderId > 0)
                {
                    TempData["SuccessMessage"] = $"Order placed successfully! Order ID: {placedOrder.OrderId}";
                    return RedirectToAction("OrderSuccess", new { id = placedOrder.OrderId });
                }
                else
                {
                    ModelState.AddModelError("", "Failed to place order. Please try again.");
                }
            }

            return View("Checkout", order);
        }

        // Step 3: GET Order Success Page
        [HttpGet]
        public IActionResult OrderSuccess(int id)
        {
            var order = new Order
            {
                OrderId = id
            };

            return View(order);
        }

    }
}
