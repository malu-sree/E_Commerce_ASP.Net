using System;
using System.Data;
using Microsoft.Data.SqlClient;
namespace E_CommerceWebsite.Models.Repository
{
    /// <summary>
    /// Repository class for managing cart-related operations.
    /// </summary>
    public class CartRepository
    {
        private readonly string _connectionString;
        /// <summary>
        /// Initializes a new instance of the <see cref="CartRepository"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration to retrieve connection string.</param>
        public CartRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ECommerceDBConnection");
        }
        /// <summary>
        /// Adds a product to the user's cart.
        /// </summary>
        /// <param name="cart">The cart item to be added.</param>
        /// <returns><c>true</c> if the item was added successfully; otherwise, <c>false</c>.</returns>
        public bool AddToCart(Cart cart)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_AddToCartItems", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Quantity", cart.Quantity);
                        command.Parameters.AddWithValue("@UserId", cart.UserId);
                        command.Parameters.AddWithValue("@ProductId", cart.ProductId);
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves all cart items for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A list of <see cref="Cart"/> items.</returns>
        public List<Cart> GetCartItems(int userId)
        {
            List<Cart> cartItems = new List<Cart>();
            SqlConnection connection = new SqlConnection(_connectionString);

            try
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("sp_GetCartsItem", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cartItems.Add(new Cart
                            {
                                CartId = Convert.ToInt32(reader["cartId"]),
                                UserId = Convert.ToInt32(reader["userId"]),
                                ProductId = Convert.ToInt32(reader["productId"]),
                                Quantity = Convert.ToInt32(reader["quantity"]),
                                Product = new Product
                                {
                                    Name = reader["name"].ToString(),
                                    Price = Convert.ToDecimal(reader["price"]),
                                    Image = reader["productPhoto"].ToString()
                                },
                                TotalPrice = Convert.ToDecimal(reader["totalPrice"])
                            });
                        }
                    }
                }
            }
            
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close(); 
            }

            return cartItems;
        }
        /// <summary>
        /// Removes a cart item based on its ID.
        /// </summary>
        /// <param name="cartId">The ID of the cart item to be removed.</param>

        public void RemoveCartItem(int cartId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("sp_DeleteCartItem", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CartId", cartId);
                    command.ExecuteNonQuery();
                }
            }
        }

    }
}
