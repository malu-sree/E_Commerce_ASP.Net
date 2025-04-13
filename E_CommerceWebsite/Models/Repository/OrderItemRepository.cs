using System.Data;
using Microsoft.Data.SqlClient;

namespace E_CommerceWebsite.Models.Repository
{
    /// <summary>
    /// Handles database operations related to Orders and Order Items.
    /// </summary>
    public class OrderItemRepository
    {
        private readonly string _connectionString;
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderItemRepository"/> class.
        /// </summary>
        /// <param name="configuration">Application configuration to access connection string.</param>
        public OrderItemRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ECommerceDBConnection");
        }
        /// <summary>
        /// Creates a new order in the database using the stored procedure <c>sp_CreatesOrder</c>.
        /// </summary>
        /// <param name="order">The order to create.</param>
        /// <returns>The ID of the created order.</returns>
        public int CreateOrder(Order order)
        {
            int orderId = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("sp_CreatesOrder", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserId", order.UserId);
                        command.Parameters.AddWithValue("@Address", order.Address);
                        command.Parameters.AddWithValue("@PaymentMethod", order.PaymentMethod);
                        command.Parameters.AddWithValue("@Status", order.Status);

                        SqlParameter outputId = new SqlParameter("@OrderId", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(outputId);

                        command.ExecuteNonQuery();

                        orderId = Convert.ToInt32(outputId.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
               
            }

            return orderId;
        }

        /// <summary>
        /// Retrieves all orders for a specific user using <c>sp_GetOrdersById</c>.
        /// </summary>
        /// <param name="userId">The user ID to filter orders.</param>
        /// <returns>List of orders for the given user.</returns>

        public List<Order> GetOrdersByUserId(int userId)
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("sp_GetOrdersById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orders.Add(new Order
                            {
                                OrderId = Convert.ToInt32(reader["orderId"]),
                                UserId = Convert.ToInt32(reader["userId"]),
                                Address = reader["address"].ToString(),
                                PaymentMethod = reader["paymentMethod"].ToString(),
                                Status = reader["status"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["createdAt"])
                            });
                        }
                    }
                }
            }

            return orders;
        }

        /// <summary>
        /// Retrieves all orders from the database using <c>sp_GetAllOrders</c>.
        /// </summary>
        /// <returns>List of all orders.</returns>
        public List<Order> GetAllOrders()
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("sp_GetAllOrders", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orders.Add(new Order
                            {
                                OrderId = Convert.ToInt32(reader["OrderId"]),
                                UserId = Convert.ToInt32(reader["UserId"]),
                                Address = reader["Address"].ToString(),
                                PaymentMethod = reader["PaymentMethod"].ToString(),
                                Status = reader["Status"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                UserName = reader["Name"].ToString()
                            });
                        }
                    }
                }
            }

            return orders;
        }

        /// <summary>
        /// Updates the status of an existing order using <c>sp_UpdateOrderStatus</c>.
        /// </summary>
        /// <param name="orderId">The ID of the order to update.</param>
        /// <param name="status">The new status of the order.</param>
        public void UpdateOrderStatus(int orderId, string status)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_UpdateOrderStatus", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@OrderId", orderId);
                    command.Parameters.AddWithValue("@Status", status);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Retrieves all order items for a given order ID using <c>sp_GetOrderItemsByOrderId</c>.
        /// </summary>
        /// <param name="orderId">The order ID to retrieve items for.</param>
        /// <returns>List of order items.</returns>
        public List<OrderItem> GetOrderItemsByOrderId(int orderId)
        {
            List<OrderItem> orderItems = new List<OrderItem>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("sp_GetOrderItemsByOrderId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@OrderId", orderId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orderItems.Add(new OrderItem
                            {
                                OrderItemId = Convert.ToInt32(reader["OrderItemId"]),
                                OrderId = Convert.ToInt32(reader["OrderId"]),
                                ProductId = Convert.ToInt32(reader["ProductId"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                Price = Convert.ToDecimal(reader["Price"]),
                                Name = reader["ProductName"].ToString()
                            });
                        }
                    }
                }
            }

            return orderItems;
        }
    }
}
