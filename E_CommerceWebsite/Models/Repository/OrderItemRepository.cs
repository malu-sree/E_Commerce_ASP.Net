using System.Data;
using Microsoft.Data.SqlClient;

namespace E_CommerceWebsite.Models.Repository
{
    public class OrderItemRepository
    {
        private readonly string _connectionString;

        public OrderItemRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ECommerceDBConnection");
        }

        public void AddOrderItems(int orderId, List<OrderItem> orderItems)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    foreach (var item in orderItems)
                    {
                        using (SqlCommand command = new SqlCommand("sp_AddOrderItem", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure; 
                            command.Parameters.AddWithValue("@OrderId", orderId);
                            command.Parameters.AddWithValue("@ProductId", item.ProductId);
                            command.Parameters.AddWithValue("@Quantity", item.Quantity);
                            command.Parameters.AddWithValue("@Price", item.Price);

                            command.ExecuteNonQuery();
                        }
                    }
                }
                
                finally
                {
                    connection.Close(); // Ensure the connection is closed
                }
            }
        }
    }
}
