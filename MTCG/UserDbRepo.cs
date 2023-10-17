using Npgsql;

namespace MTCG;

public class UserDbRepo
{
    private readonly string _connectionString;

    public UserDbRepo(string connectionString)
    {
        _connectionString = connectionString;
    }

    public User GetById(int userId)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            using (var command = new NpgsqlCommand("SELECT * FROM Users WHERE Id = @Id", connection))
            {
                command.Parameters.AddWithValue("Id", userId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Username = reader.GetString(reader.GetOrdinal("Username")),
                            Email = reader.GetString(reader.GetOrdinal("Email"))
                        };
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

    public void Update(User user)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            using (var command = new NpgsqlCommand("UPDATE Users SET Username = @Username, Email = @Email WHERE Id = @Id", connection))
            {
                command.Parameters.AddWithValue("Username", user.Username);
                command.Parameters.AddWithValue("Email", user.Email);
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
}

