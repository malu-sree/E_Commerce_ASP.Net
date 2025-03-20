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


        public void CreateOrder(Order order)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("sp_CreateOrders", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserId", order.UserId);
                    command.Parameters.AddWithValue("@Address", order.Address);
                    command.Parameters.AddWithValue("@PaymentMethod", order.PaymentMethod);
                    command.Parameters.AddWithValue("@Status", order.Status);

                    command.ExecuteNonQuery(); // Execute without returning anything
                }
            }
        }
    }
}
