using Npgsql;
using MTCG.Models;
namespace MTCG.DAL;


public static class DbManager
{

    private static NpgsqlConnection conn;
    private static UserDbRepo userRepository;
    public static void connect() {
        string connString = "Host=localhost;Username=admin ;Password=admin;Database=mtcgdb";
        // Create a new connection
        conn = new NpgsqlConnection(connString);
        conn.Open();
        userRepository = new UserDbRepo(conn);
    }

    public static bool loginUser(User user)
    {
        user = userRepository.GetByLogin(user.getUsername(), user.getPassword());
        if (user is not null) {
            return true;
        }
        else {
            return false;
        }
    }
    
    public static bool registerUser(User user) {
        user = userRepository.GetByLogin(user.getUsername(), user.getPassword());
        
        user.setNewUserData();

        // If the username is not already in use, proceed with registration
        string insertQuery = "INSERT INTO users (username, password) VALUES (@username, @password)";

        using (var insertCmd = new NpgsqlCommand(insertQuery, conn))
        {
            insertCmd.Parameters.AddWithValue("@username", user.getUsername());
            insertCmd.Parameters.AddWithValue("@password", user.getPassword());

            int rowsAffected = insertCmd.ExecuteNonQuery();

            // Registration is successful if at least one row was inserted
            return rowsAffected > 0;
            
        }
    }
    
    
}
