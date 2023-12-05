using Npgsql;
using MTCG.Models;

namespace MTCG.DAL;

public class UserDbRepo
{
    
    private NpgsqlConnection connection;
    public UserDbRepo(NpgsqlConnection connection) {
        this.connection = connection;
    }

    
    public User GetByLogin(string username, string password) {
        using (connection) {
            
            using (var command = new NpgsqlCommand("SELECT * FROM users WHERE username = @username AND password = @password", connection))
            {
                command.Parameters.AddWithValue("username", username);
                command.Parameters.AddWithValue("password", password);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            id = reader.GetInt32(reader.GetOrdinal("id")),
                            username = reader.GetString(reader.GetOrdinal("username")),
                            coins = reader.GetInt32(reader.GetOrdinal("coins")),
                            elo = reader.GetInt32(reader.GetOrdinal("elo"))
                        };
                    }
                }
            }
        }
        return null;
    }
    
    public User GetByUsername(string username)
    {
        using (connection) {
            using (var command = new NpgsqlCommand("SELECT * FROM users WHERE username = @username", connection))
            {
                command.Parameters.AddWithValue("username", username);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            id = reader.GetInt32(reader.GetOrdinal("id")),
                            username = reader.GetString(reader.GetOrdinal("username")),
                            coins = reader.GetInt32(reader.GetOrdinal("coins")),
                            elo = reader.GetInt32(reader.GetOrdinal("elo"))
                        };
                    }
                }
            }
        }
        return null;
    }
    public bool checkLoginData(User user) {
        string query = "SELECT * FROM users WHERE username = @username AND password = @password";

        using (var cmd = new NpgsqlCommand(query, connection))
        {
            cmd.Parameters.AddWithValue("@username", user.getUsername());
            cmd.Parameters.AddWithValue("@password", user.getPassword());

            using (var reader = cmd.ExecuteReader()) {
                if (reader.HasRows)
                {
                    reader.Read();
                    user.setUserData((int)reader["coins"], (int)reader["elo"]);
                }
                // If there is a row in the result set, the login is successful
                return reader.HasRows;
            }
        }
    }
    public User setUserData(User user, bool newUser) {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            using (var cmd = new NpgsqlCommand("SELECT * FROM users WHERE username = @username AND password = @password", connection))
            {
                cmd.Parameters.AddWithValue("@username", user.getUsername());
                cmd.Parameters.AddWithValue("@password", user.getPassword());
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (newUser) {
                            user.setNewUserData();
                        }
                        else {
                            int coins = reader.GetInt32(reader.GetOrdinal("coins"));
                            int elo = reader.GetInt32(reader.GetOrdinal("elo"));
                            user.setUserData(coins, elo);
                        }
                        
                    }
                }
            }
        }
        return null;
    }

    public IEnumerable<User> GetAll()
    {
        var users = new List<User>();
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            using (var command = new NpgsqlCommand("SELECT * FROM Users", connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    users.Add(new User
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Username = reader.GetString(reader.GetOrdinal("Username")),
                        Email = reader.GetString(reader.GetOrdinal("Email"))
                    });
                }
            }
        }
        return users;
    }

    public void Add(User user)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            using (var command = new NpgsqlCommand("INSERT INTO Users (Username, Email) VALUES (@Username, @Email) RETURNING Id", connection))
            {
                command.Parameters.AddWithValue("Username", user.Username);
                command.Parameters.AddWithValue("Email", user.Email);
                user.Id = (int)command.ExecuteScalar();
            }
        }
    }

    public void UpdateUser(User user)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            using (var command = new NpgsqlCommand("UPDATE Users SET Username = @Username, Email = @Email WHERE Id = @Id", connection))
            {
                command.Parameters.AddWithValue("Username", user.getUsername());
                command.Parameters.AddWithValue("Email", user.getPassword());
                command.Parameters.AddWithValue("Id", user.Id);
                command.ExecuteNonQuery();
            }
        }
    }

    public void Delete(int userId)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            using (var command = new NpgsqlCommand("DELETE FROM Users WHERE Id = @Id", connection))
            {
                command.Parameters.AddWithValue("Id", userId);
                command.ExecuteNonQuery();
            }
        }
    }
    */
}

