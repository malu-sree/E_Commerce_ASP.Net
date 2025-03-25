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

        //[HttpGet]
        //public IActionResult Create(int productId, int quantity, decimal price)
        //{
        //    var userId = HttpContext.Session.GetInt32("UserId");

        //    if (userId == null)
        //    {
        //        TempData["ErrorMessage"] = "Please log in to place an order.";
        //        return RedirectToAction("Login", "User");
        //    }

        //    // Get cart items for the user and filter by productId
        //    var cartItems = _cartRepository.GetCartItems(userId.Value);
        //    var selectedItem = cartItems.FirstOrDefault(c => c.ProductId == productId);

        //    if (selectedItem == null)
        //    {
        //        TempData["ErrorMessage"] = "Invalid product selection.";
        //        return RedirectToAction("Cart", "Cart");
        //    }

        //    var order = new Order
        //    {
        //        UserId = userId.Value,
        //        Status = "Processing",
        //        OrderItems = new List<OrderItem>
        //{
        //    new OrderItem
        //    {
        //        ProductId = productId,
        //        Quantity = quantity,
        //        Price = price,
        //        Name = selectedItem.ProductName ?? "Unknown Product"// Extract product name from cart item
        //    }
        //}
        //    };

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
                return View(order);
            }
            _orderRepository.CreateOrder(order);
            TempData["SuccessMessage"] = "Order placed successfully!";
            return RedirectToAction("Create");


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


        //[HttpPost]
        //public IActionResult Create(Order order)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        int orderId = _orderItemRepository.CreateOrder(order);
        //        if (orderId > 0)
        //        {
        //            TempData["Success"] = "Order created successfully!";
        //            ModelState.Clear();
        //            return View(order);
        //        }
        //        else
        //        {
        //            TempData["Error"] = "Failed to create order.";
        //        }
        //    }
        //    return View(order);
        //}


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

        //[HttpGet]
        //public IActionResult GetOrdersByUserId(int userId)
        //{
        //    var orders = _orderItemRepository.GetOrdersByUserId(userId);
        //    return View(orders);
        //}


        public IActionResult AllOrders()
        {
            List<Order> orders = _orderRepository.GetAllOrders();
            return View(orders);
        }

        //[HttpGet]
        //public IActionResult GetAllOrders()
        //{
        //    var orders = _orderItemRepository.GetAllOrders();
        //    return View(orders);
        //}

        //[HttpGet]
        //public IActionResult GetOrderItems(int orderId)
        //{
        //    var orderItems = _orderItemRepository.GetOrderItemsByOrderId(orderId);
        //    return View(orderItems);
        //}

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

        //public IActionResult UpdateOrderStatus(int orderId, string status)
        //{
        //    _orderItemRepository.UpdateOrderStatus(orderId, status);
        //    TempData["Success"] = "Order status updated successfully!";
        //    return RedirectToAction("GetOrdersByUserId", new { userId = 1 }); 
        //}
        //public IActionResult Orders()
        //{
        //    var orders = _orderRepository.GetOrdersForAdmin();
        //    return View(orders);
        //}

    }
}
