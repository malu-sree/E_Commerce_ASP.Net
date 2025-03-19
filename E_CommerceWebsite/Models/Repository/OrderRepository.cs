using E_CommerceWebsite.Models;
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
        public Order PlaceOrder(Order order)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("sp_PlaceOrder", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserId", order.UserId);
                        command.Parameters.AddWithValue("@FullName", order.FullName);
                        command.Parameters.AddWithValue("@PhoneNumber", order.PhoneNumber);
                        command.Parameters.AddWithValue("@Address", order.Address);
                        command.Parameters.AddWithValue("@PaymentMethod", order.PaymentMethod);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                order.OrderId = Convert.ToInt32(reader["OrderId"]);
                                order.TotalAmount = Convert.ToDecimal(reader["TotalAmount"]);
                                order.FullName = reader["FullName"].ToString();
                                order.PhoneNumber = reader["PhoneNumber"].ToString();
                                order.Address = reader["Address"].ToString();
                                order.PaymentMethod = reader["PaymentMethod"].ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("An error occurred while placing the order: " + ex.Message);
                }
            }

            return order;
        }

    }
}
