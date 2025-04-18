using System.Data;
using Microsoft.Data.SqlClient;
using E_CommerceWebsite.Models;

namespace E_CommerceWebsite.Models.Repository
{
    /// <summary>
    /// Repository class for handling user-related database operations.
    /// </summary>
    public class UserRepository
    {
        private readonly string _connectionString;
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="configuration">Configuration object to retrieve connection string.</param>
        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ECommerceDBConnection");
        }
        /// <summary>
        /// Registers a new user in the database.
        /// </summary>
        /// <param name="user">The user object containing user details.</param>
        /// <returns>True if registration is successful, otherwise false.</returns>
        public bool RegisterUser(User user)
        {
            
            SqlConnection connection = new SqlConnection(_connectionString);
            try
            {
               
                connection.Open();
                using (SqlCommand command = new SqlCommand("sp_RegisterUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Name", user.Name);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
                    command.Parameters.AddWithValue("@Address", user.Address);
                    command.Parameters.AddWithValue("@Password", PasswordHelper.HashPassword(user.Password)); 
                    command.Parameters.AddWithValue("@Role", user.Role);

                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        /// <summary>
        /// Authenticates a user with email and password.
        /// </summary>
        /// <param name="email">User email address.</param>
        /// <param name="password">User password.</param>
        /// <returns>User object if authenticated, otherwise null.</returns>

        public User AuthenticateUser(string email, string password)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            try
            {
                connection.Open();
                string hashPassword = PasswordHelper.HashPassword(password);
                using (SqlCommand command = new SqlCommand("AuthenticateUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", hashPassword);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read()) 
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
            
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close(); 
                }
            }
            return null; 
        }
        /// <summary>
        /// Retrieves all users from the database.
        /// </summary>
        /// <returns>List of users.</returns>
        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("sp_GetAllUsers", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(new User
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Name = reader["Name"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    PhoneNumber = reader["Phone"].ToString(),
                                    Address = reader["Address"].ToString(),

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
            return users;
        }
        /// <summary>
        /// Deletes a user from the database based on the ID.
        /// </summary>
        /// <param name="id">User ID to delete.</param>
        /// <returns>True if deleted successfully, otherwise false.</returns>
        public bool DeleteUser(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand("spDeleteUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Id", id);
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
        /// Retrieves a user by ID from the database.
        /// </summary>
        /// <param name="id">User ID to retrieve.</param>
        /// <returns>User object if found, otherwise null.</returns>
        public User GetUserById(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_GetUserById", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Id", id);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new User
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Name = reader["Name"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    PhoneNumber = reader["Phone"].ToString(),
                                    Address = reader["Address"].ToString(),
                                    
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
        /// Updates user details in the database.
        /// </summary>
        /// <param name="user">The user object with updated details.</param>
        /// <returns>True if update is successful, otherwise false.</returns>
        public bool UpdateUser(User user)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand("sp_UpdateUsers", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Id", user.Id);
                        command.Parameters.AddWithValue("@Name", user.Name);
                        command.Parameters.AddWithValue("@Email", user.Email);
                        command.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
                        command.Parameters.AddWithValue("@Address", user.Address);
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

        public bool IsEmailExists(string email)
        {
            bool exists = false;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_CheckEmailExists", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Email", email);

                    connection.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    exists = count > 0;
                }
            }
            return exists;
        }



    }
}
