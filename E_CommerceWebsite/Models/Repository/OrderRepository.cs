﻿using E_CommerceWebsite.Models;
using System.Data;
using Microsoft.Data.SqlClient;


namespace E_CommerceWebsite.Models.Repository
{
    public class OrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ECommerceDBConnection");

        }


        //public void CreateOrder(Order order)
        //{
        //    using (SqlConnection connection = new SqlConnection(_connectionString))
        //    {
        //        connection.Open();

        //        using (SqlCommand command = new SqlCommand("sp_CreateOrders", connection))
        //        {
        //            command.CommandType = CommandType.StoredProcedure;
        //            command.Parameters.AddWithValue("@UserId", order.UserId);
        //            command.Parameters.AddWithValue("@Address", order.Address);
        //            command.Parameters.AddWithValue("@PaymentMethod", order.PaymentMethod);
        //            command.Parameters.AddWithValue("@Status", order.Status);

        //            command.ExecuteNonQuery();
        //        }
        //    }
        //}

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

                    // ✅ Output parameter for OrderId
                    SqlParameter outputParam = new SqlParameter("@OrderId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputParam);

                    command.ExecuteNonQuery();

                    // ✅ Get generated OrderId
                    orderId = (int)outputParam.Value;
                }

                // ✅ Save Order Items
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

