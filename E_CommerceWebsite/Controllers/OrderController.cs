using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using E_CommerceWebsite.Models;
using E_CommerceWebsite.Models.Repository;


namespace E_CommerceWebsite.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderRepository _orderRepository;
        private readonly OrderItemRepository _orderItemRepository;
        private readonly CartRepository _cartRepository;
        public OrderController(OrderRepository orderRepository, OrderItemRepository orderItemRepository, CartRepository cartRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _cartRepository = cartRepository;
        }
        [HttpGet]
        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["ErrorMessage"] = "Please log in to place an order.";
                return RedirectToAction("Login", "User");
            }

            return View(new Order());
        }

        //[HttpPost]
        //public IActionResult Create(Order order)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _orderRepository.CreateOrder(order);
        //        TempData["SuccessMessage"] = "Order placed successfully!";
        //        return RedirectToAction("Create");
        //    }

        //    return View(order);
        //}

        [HttpPost]
        public IActionResult Create(Order order)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["ErrorMessage"] = "Please log in to place an order.";
                return RedirectToAction("Login", "User");
            }

            order.UserId = userId.Value;

            Console.WriteLine($"UserId: {order.UserId}");
            Console.WriteLine($"Address: {order.Address}");
            Console.WriteLine($"PaymentMethod: {order.PaymentMethod}");
            Console.WriteLine($"Status: {order.Status}");
            Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");

            if (ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Key: {state.Key}, Error: {error.ErrorMessage}");
                    }
                }
                _orderRepository.CreateOrder(order);
                TempData["SuccessMessage"] = "Order placed successfully!";
                return RedirectToAction("Create");
            }

            return View(order);
        }

    }
}
