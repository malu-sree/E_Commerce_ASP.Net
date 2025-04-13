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
        private readonly ProductRepository _productRepository;

        /// <summary>
        /// Initializes the OrderController with repository dependencies.
        /// </summary>
        public OrderController(OrderRepository orderRepository, OrderItemRepository orderItemRepository, CartRepository cartRepository, ProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }
        /// <summary>
        /// Displays the order creation form for logged-in users.
        /// </summary>
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
        //    var userId = HttpContext.Session.GetInt32("UserId");

        //    if (userId == null)
        //    {
        //        TempData["ErrorMessage"] = "Please log in to place an order.";
        //        return RedirectToAction("Login", "User");
        //    }

        //    order.UserId = userId.Value;

        //    Console.WriteLine($"UserId: {order.UserId}");
        //    Console.WriteLine($"Address: {order.Address}");
        //    Console.WriteLine($"PaymentMethod: {order.PaymentMethod}");
        //    Console.WriteLine($"Status: {order.Status}");
        //    Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");

        //    if (ModelState.IsValid)
        //    {
        //        foreach (var state in ModelState)
        //        {
        //            foreach (var error in state.Value.Errors)
        //            {
        //                Console.WriteLine($"Key: {state.Key}, Error: {error.ErrorMessage}");
        //            }
        //        }
        //        return View(order);
        //    }
        //    _orderRepository.CreateOrder(order);
        //    TempData["SuccessMessage"] = "Order placed successfully!";
        //    return RedirectToAction("Create");


        //}


        //[HttpPost]
        //public IActionResult Create(Order order)
        //{
        //    var userId = HttpContext.Session.GetInt32("UserId");

        //    if (userId == null)
        //    {
        //        TempData["ErrorMessage"] = "Please log in to place an order.";
        //        return RedirectToAction("Login", "User");
        //    }

        //    order.UserId = userId.Value;

        //    int orderId = _orderRepository.CreateOrder(order);

        //    if (orderId > 0)
        //    {
        //        TempData["SuccessMessage"] = "Order placed successfully!";
        //        return RedirectToAction("Create");
        //    }
        //    else
        //    {
        //        TempData["ErrorMessage"] = "Failed to place order.";
        //        return View(order);
        //    }
        //}

        //[HttpPost]
        //public IActionResult Create(Order order)
        //{
        //    var userId = HttpContext.Session.GetInt32("UserId");

        //    if (userId == null)
        //    {
        //        TempData["ErrorMessage"] = "Please log in to place an order.";
        //        return RedirectToAction("Login", "User");
        //    }

        //    order.UserId = userId.Value;

        //    int orderId = _orderRepository.CreateOrder(order);

        //    if (orderId > 0)
        //    {
        //        TempData["SuccessMessage"] = "Order placed successfully!";
        //        return RedirectToAction("Create");
        //    }
        //    else
        //    {
        //        TempData["ErrorMessage"] = "Failed to place order.";
        //        return View(order);
        //    }
        //}


        /// <summary>
        /// Handles order creation with stock validation.
        /// </summary>
        /// <param name="order">Order data submitted from the form.</param>
        /// <returns>Redirects to confirmation or re-displays form on failure.</returns>

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

           
            foreach (var item in order.OrderItems)
            {
                bool stockAvailable = _productRepository.CheckStock(item.ProductId, item.Quantity);

                if (!stockAvailable)
                {
                    TempData["ErrorMessage"] = "Some products are out of stock.";
                    return View(order); 
                }
            }

    
            int orderId = _orderRepository.CreateOrder(order);

            if (orderId > 0)
            {
                
                foreach (var item in order.OrderItems)
                {
                    _productRepository.ReduceStock(item.ProductId, item.Quantity);
                }

                TempData["SuccessMessage"] = "Order placed successfully!";
                return RedirectToAction("Create"); 
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to place order.";
                return View(order);
            }
        }



        //[HttpPost]
        //public IActionResult Create(Order order)
        //{
        //    var userId = HttpContext.Session.GetInt32("UserId");

        //    if (userId == null)
        //    {
        //        TempData["ErrorMessage"] = "Please log in to place an order.";
        //        return RedirectToAction("Login", "User");
        //    }

        //  
        //    order.UserId = userId.Value;

        //    
        //    var cartId = HttpContext.Session.GetInt32("CartId");
        //    if (cartId == null)
        //    {
        //        TempData["ErrorMessage"] = "Cart not found.";
        //        return RedirectToAction("Index","Cart");
        //    }
        //    order.CartId = cartId.Value;

        //    
        //    Console.WriteLine($"UserId: {order.UserId}");
        //    Console.WriteLine($"CartId: {order.CartId}");
        //    Console.WriteLine($"Address: {order.Address}");
        //    Console.WriteLine($"PaymentMethod: {order.PaymentMethod}");
        //    Console.WriteLine($"Status: {order.Status}");
        //    Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");

        //    if (!ModelState.IsValid)
        //    {
        //        foreach (var state in ModelState)
        //        {
        //            foreach (var error in state.Value.Errors)
        //            {
        //                Console.WriteLine($"Key: {state.Key}, Error: {error.ErrorMessage}");
        //            }
        //        }
        //        return View(order);
        //    }

        //   
        //    _orderRepository.CreateOrder(order);
        //    TempData["SuccessMessage"] = "Order placed successfully!";
        //    return RedirectToAction("Create");
        //}



        /// <summary>
        /// Displays all orders placed by the currently logged-in user.
        /// </summary>
        /// <returns>View with list of user's orders.</returns>

        public IActionResult UserOrder()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["ErrorMessage"] = "Please log in to view your orders.";
                return RedirectToAction("Login", "User");
            }

            var orders = _orderRepository.GetOrdersByUserId(userId.Value);
            return View(orders);
        }




        /// <summary>
        /// Displays a list of all orders (Admin only).
        /// </summary>
        /// <returns>View with all orders.</returns>
        public IActionResult AllOrders()
        {
            List<Order> orders = _orderRepository.GetAllOrders();
            return View(orders);
        }



        /// <summary>
        /// Updates the status of a specific order.
        /// </summary>
        /// <param name="orderId">The ID of the order to update.</param>
        /// <param name="status">The new status to apply.</param>
        /// <returns>Redirects to all orders list.</returns>

        [HttpPost]
        public IActionResult UpdateOrderStatus(int orderId, string status)
        {
            if (!string.IsNullOrEmpty(status))
            {
                _orderRepository.UpdateOrderStatus(orderId, status);
                TempData["SuccessMessage"] = "Order status updated successfully!";
            }

            return RedirectToAction("AllOrders");
        }

        
        //public IActionResult Orders()
        //{
        //    var orders = _orderRepository.GetOrdersForAdmin();
        //    return View(orders);
        //}
      




    }
}
