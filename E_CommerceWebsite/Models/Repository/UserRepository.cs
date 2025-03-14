using System.Data;
using Microsoft.Data.SqlClient;

using E_CommerceWebsite.Models;

namespace E_CommerceWebsite.Models.Repository
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ECommerceDBConnection");
        }

        public bool RegisterUser(User user)
        {
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(_connectionString);
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_RegisterUser", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Name", user.Name);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
                    cmd.Parameters.AddWithValue("@Address", user.Address);
                    cmd.Parameters.AddWithValue("@Password", PasswordHelper.HashPassword(user.Password)); // Hashing password
                    cmd.Parameters.AddWithValue("@Role", user.Role);

                    int result = cmd.ExecuteNonQuery();
                    return result > 0;
                }
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }


        public User AuthenticateUser(string email, string password)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            try
            {
                connection.Open();
                string hashPassword = PasswordHelper.HashPassword(password);
                string query = "SELECT * FROM Users WHERE Email = @Email AND Password = @Password";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", hashPassword);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read()) // If user found in database
                        {
                            return new User
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString(),
                                Email = reader["Email"].ToString(),
                                Role = reader["Role"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}"); // Log the exception
                return null;
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close(); // Ensure the connection is closed
                }
            }
            return null; // No user found
        }


    }
}
