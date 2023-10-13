using Npgsql;
namespace MTCG;


public static class DbManager {
    public static void connect()
    {
        string connString = "Host=localhost;Username=admin ;Password=admin;Database=mtcgdb";

        // Create a new connection
        using (var conn = new NpgsqlConnection(connString))
        {
            conn.Open();

            // Define a SQL query
            string query = "SELECT * FROM mytable";

            // Create a command object
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                // Execute the query and obtain a data reader
                using (var reader = cmd.ExecuteReader())
                {
                    // Iterate through the rows and print the values
                    while (reader.Read())
                    {
                        Console.WriteLine("ID: {0}, Name: {1}", reader["id"], reader["name"]);
                    }
                }
            }
    }
    
}