using E_CommerceWebsite.Models;
using System.Data;
using Microsoft.Data.SqlClient;


namespace E_CommerceWebsite.Models.Repository
{
    /// <summary>
    /// Repository class for handling operations related to orders and order items.
    /// </summary>
    public class OrderRepository
    {
        private readonly string _connectionString;
        /// <summary>
        /// Initializes a new instance of the OrderRepository class.
        /// </summary>
        /// <param name="configuration">Application configuration to fetch the connection string.</param>
        public OrderRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ECommerceDBConnection");

        }


        /// <summary>
        /// Creates a new order and its associated order items in the database.
        /// </summary>
        /// <param name="order">Order object containing order details and a list of order items.</param>
        /// <returns>The generated OrderId of the newly created order.</returns>
        public int CreateOrder(Order order)
        {
            int orderId = 0;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("sp_AddOrders", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserId", order.UserId);
                    command.Parameters.AddWithValue("@Address", order.Address);
                    command.Parameters.AddWithValue("@PaymentMethod", order.PaymentMethod);
                    command.Parameters.AddWithValue("@Status", order.Status);

                    
                    SqlParameter outputParam = new SqlParameter("@OrderId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputParam);

                    command.ExecuteNonQuery();

                  
                    orderId = (int)outputParam.Value;
                }

              
                if (order.OrderItems != null && order.OrderItems.Any())
                {
                    foreach (var item in order.OrderItems)
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_AddOrderItem", connection))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@OrderId", orderId);
                            cmd.Parameters.AddWithValue("@ProductId", item.ProductId);
                            cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                            cmd.Parameters.AddWithValue("@Price", item.Price);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }

            return orderId;
        }



        /// <summary>
        /// Retrieves a list of orders placed by a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A list of orders for the specified user.</returns>


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
        /// Retrieves all orders from the database.
        /// </summary>
        /// <returns>A list of all orders.</returns>
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
        /// Updates the status of a specific order.
        /// </summary>
        /// <param name="orderId">The ID of the order to update.</param>
        /// <param name="status">The new status to assign to the order.</param>

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

    }
}

