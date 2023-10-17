using Npgsql;
namespace MTCG;


public static class DbManager
{

    private static NpgsqlConnection conn;
    public static void connect() {
        string connString = "Host=localhost;Username=admin ;Password=admin;Database=mtcgdb";
        // Create a new connection
        conn = new NpgsqlConnection(connString);
        conn.Open();
    }

    public static bool loginUser(User user) {
       
        string query = "SELECT * FROM users WHERE username = @username AND password = @password";

        using (var cmd = new NpgsqlCommand(query, conn))
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
    
    public static bool registerUser(User user) {
        string checkUsernameQuery = "SELECT * FROM users WHERE username = @username";

        using (var cmd = new NpgsqlCommand(checkUsernameQuery, conn))
        {
            cmd.Parameters.AddWithValue("@username", user.getUsername());

            using (var reader = cmd.ExecuteReader()) {
                if (reader.HasRows) { 
                    return false;
                }
            }
        }
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
