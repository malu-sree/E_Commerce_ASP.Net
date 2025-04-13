using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using E_CommerceWebsite.Models;
using System.Diagnostics.Eventing.Reader;

namespace E_CommerceWebsite.Models.Repository
{
    /// <summary>
    /// Repository class for managing product-related database operations.
    /// </summary>
    public class ProductRepository
    {

        private readonly string _connectionString;
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductRepository"/> class with configuration.
        /// </summary>
        /// <param name="configuration">Application configuration to fetch the connection string.</param>
        public ProductRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ECommerceDBConnection");
        }
        /// <summary>
        /// Adds a new product to the database.
        /// </summary>
        /// <param name="product">Product details to be added.</param>
        /// <returns>True if product is added successfully; otherwise, false.</returns>
        public bool AddProduct(Product product)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_AddProducts", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Name", product.Name);
                        command.Parameters.AddWithValue("@Description", product.Description);
                        command.Parameters.AddWithValue("@Price", product.Price);
                        command.Parameters.AddWithValue("@Quantity", product.Quantity);
                        command.Parameters.AddWithValue("@ProductPhoto", product.Image); 

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
        /// Retrieves a list of all products from the database.
        /// </summary>
        /// <returns>List of all products.</returns>
        public List<Product> GetAllProduct()
        {
            List<Product> products = new List<Product>();

            using (SqlConnection connection = new SqlConnection(_connectionString)) 
            {
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_GetAllProducts", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                products.Add(new Product
                                {
                                    ProductId = Convert.ToInt32(reader["ProductId"]),
                                    Name = reader["Name"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    Price = Convert.ToDecimal(reader["Price"]),
                                    Quantity = Convert.ToInt32(reader["Quantity"]),
                                    Image = reader["ProductPhoto"].ToString()
                                });
                            }
                        }
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

            return products;
        }
        /// <summary>
        /// Retrieves a product by its ID.
        /// </summary>
        /// <param name="productId">ID of the product to fetch.</param>
        /// <returns>Product object if found; otherwise, null.</returns>
        public Product GetProductById(int productId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_GetProductById", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ProductId", productId);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Product
                                {
                                    ProductId = Convert.ToInt32(reader["ProductId"]),
                                    Name = reader["Name"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    Price = Convert.ToDecimal(reader["Price"]),
                                    Quantity = Convert.ToInt32(reader["Quantity"]),
                                    Image = reader["ProductPhoto"].ToString()
                                };
                            }
                        }
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
            return null;
        }
        /// <summary>
        /// Updates an existing product in the database.
        /// </summary>
        /// <param name="product">Updated product details.</param>
        /// <returns>True if the product is updated successfully; otherwise, false.</returns>
        public bool UpdateProduct(Product product)
        {
           using (SqlConnection connection=new SqlConnection(_connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_UpdateProduct", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@ProductId", product.ProductId);
                        command.Parameters.AddWithValue("@Name", product.Name);
                        command.Parameters.AddWithValue("@Description", product.Description);
                        command.Parameters.AddWithValue("@Price", product.Price);
                        command.Parameters.AddWithValue("@Quantity", product.Quantity);
                        command.Parameters.AddWithValue("@ProductPhoto", product.Image);

                        connection.Open();

                        int rowAffected = command.ExecuteNonQuery();
                        return rowAffected > 0;


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
        /// Deletes a product from the database by its ID.
        /// </summary>
        /// <param name="productId">ID of the product to delete.</param>
        /// <returns>True if the product is deleted successfully; otherwise, false.</returns>
        public bool DeleteProduct(int productId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_DeleteProduct", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ProductId", productId);
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
        /// Reduces the stock of a product after a purchase.
        /// </summary>
        /// <param name="productId">ID of the product.</param>
        /// <param name="quantity">Quantity to reduce.</param>
        /// <returns>True if stock is reduced successfully; otherwise, false.</returns>
        public bool ReduceStock(int productId, int quantity)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_ReduceStock", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ProductId", productId);
                        command.Parameters.AddWithValue("@Quantity", quantity);

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
        /// Checks if the required quantity of a product is available in stock.
        /// </summary>
        /// <param name="productId">ID of the product.</param>
        /// <param name="quantity">Quantity to check for availability.</param>
        /// <returns>True if sufficient stock is available; otherwise, false.</returns>
        public bool CheckStock(int productId, int quantity)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SELECT Quantity FROM Products WHERE productId = @ProductId", connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);
                    connection.Open();

                    int availableStock = Convert.ToInt32(command.ExecuteScalar());

                    return availableStock >= quantity; 
                }
            }
        }


    }
}
