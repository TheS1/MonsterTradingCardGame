using Npgsql;
using MTCG.Models;

namespace MTCG.DAL;

public class UserDbRepo
{
    
    private NpgsqlConnection connection;
    public UserDbRepo(NpgsqlConnection connection) {
        this.connection = connection;
    }

    
    public bool LoginUser(string username, string password) {
        using (var command = new NpgsqlCommand("SELECT * FROM users WHERE username = @username AND password = @password", connection))
        {
            command.Parameters.AddWithValue("username", username);
            command.Parameters.AddWithValue("password", password);
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    public bool UsernameExists(string username) {
        using (var command = new NpgsqlCommand("SELECT * FROM users WHERE username = @username", connection))
        {
            command.Parameters.AddWithValue("username", username);
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    
    public bool UsernameExistsBesidesSelf(int userID, string username) {
        using (var command = new NpgsqlCommand("SELECT * FROM users WHERE username = @username and id != @userID", connection))
        {
            command.Parameters.AddWithValue("username", username);
            command.Parameters.AddWithValue("userID", userID);
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    public bool isAdmin(string username) {
        using (var command = new NpgsqlCommand("SELECT admin FROM users WHERE username = @username", connection))
        {
            command.Parameters.AddWithValue("username", username);
            var result = command.ExecuteScalar();
            return result != null && (bool)result;
        }
    }

    public void SetUserData(User user) {
        
        using (var command = new NpgsqlCommand("SELECT * FROM users WHERE username = @username", connection)) {
            command.Parameters.AddWithValue("username", user.getUsername());
            using (var reader = command.ExecuteReader()) {
                if (reader.Read()) {
                    user.id = Convert.ToInt32(reader["id"]);
                    user.username = reader["username"].ToString();
                    user.coins = Convert.ToInt32(reader["coins"]);
                    user.elo = Convert.ToInt32(reader["elo"]);
                    user.isAdmin = Convert.ToBoolean(reader["admin"]);
                    user.timeActive = DateTime.Now;
                }
            }
        }
    }

    public void Add(User user) {
        const string insertQuery = "INSERT INTO users (username, password) VALUES (@username, @password)";
        using (var insertCmd = new NpgsqlCommand(insertQuery, connection))
        {
            insertCmd.Parameters.AddWithValue("@username", user.getUsername());
            insertCmd.Parameters.AddWithValue("@password", user.getPassword());
            insertCmd.ExecuteNonQuery();
        }
    }


    public void AddCardsToUser(int userID, string cardID) {
        const string insertQuery = "INSERT INTO userCards (id, card_id) VALUES (@userID, @cardID)";
        Console.WriteLine(userID);
        Console.WriteLine(cardID);
        using (var insertCmd = new NpgsqlCommand(insertQuery, connection))
        {
            insertCmd.Parameters.AddWithValue("@userID", userID);
            insertCmd.Parameters.AddWithValue("@cardID", cardID);
            insertCmd.ExecuteNonQuery();
        }
    }


    public void SubtractCoins(int userID, int price) {
        string updateQuery = "UPDATE users SET coins = coins - @coinsToSubtract WHERE id = @userId";
        // Specify the number of coins to subtract
        using (NpgsqlCommand updateCommand = new NpgsqlCommand(updateQuery, connection))
        {
            // Add parameters to the update command
            updateCommand.Parameters.AddWithValue("@coinsToSubtract", price);
            updateCommand.Parameters.AddWithValue("@userId", userID);
            updateCommand.ExecuteNonQuery();
        }
    }

    public bool UserOwnsCard(int userID, string cardID) {
        using (var command = new NpgsqlCommand("SELECT * FROM userCards WHERE id = @userID AND card_ID = @cardID", connection))
        {
            command.Parameters.AddWithValue("userID", userID);
            command.Parameters.AddWithValue("cardID", cardID);
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    
    public List<string> getCardsOfUser(int userID) {
        List<string> cards = new List<string>();
        
        using (NpgsqlCommand command = new NpgsqlCommand("select userCards.card_id from userCards inner join cards on cards.card_id = userCards.card_id where userCards.id = @userID order by damage_value desc;", connection))
        {
            command.Parameters.AddWithValue("userID", userID);
            using (NpgsqlDataReader reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    string cardId = reader["card_id"].ToString() ?? "";
                    cards.Add(cardId);
                }
            }
        }
        return cards;
    }
    
    
    public List<Card> updateDeck(int userID, List<string> deck) {
        List<Card> cardDeck = new List<Card>();
        using (NpgsqlCommand command = new NpgsqlCommand("update userCards set indeck = false where id = @userID", connection)) {
            command.Parameters.AddWithValue("userID", userID);
            command.ExecuteNonQuery();
        }
        
        using (NpgsqlCommand command = new NpgsqlCommand("update userCards set indeck = true where card_id in (@card1, @card2, @card3, @card4)", connection)) {
            command.Parameters.AddWithValue("card1", deck[0]);
            command.Parameters.AddWithValue("card2", deck[1]);
            command.Parameters.AddWithValue("card3", deck[2]);
            command.Parameters.AddWithValue("card4", deck[3]);
            command.ExecuteNonQuery();
        }

        using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM cards WHERE card_id in (@card1, @card2, @card3, @card4)", connection)) {
            command.Parameters.AddWithValue("card1", deck[0]);
            command.Parameters.AddWithValue("card2", deck[1]);
            command.Parameters.AddWithValue("card3", deck[2]);
            command.Parameters.AddWithValue("card4", deck[3]);

            using (NpgsqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Card card = new Card
                    {
                        Id = reader["card_id"].ToString(),
                        Name = reader["card_name"].ToString(),
                        Type = reader["card_type"].ToString(),
                        Damage = Convert.ToDouble(reader["damage_value"]),
                        Element = (ElementType)Enum.Parse(typeof(ElementType), reader["element_type"].ToString())
                    };

                    cardDeck.Add(card);
                }
            }
        }

        return cardDeck;
    }


/*
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

    public string getUserProfile(int userID) {
        using (var command = new NpgsqlCommand("SELECT * FROM users WHERE id = @userID", connection)) {
            command.Parameters.AddWithValue("userID", userID);
            using (var reader = command.ExecuteReader()) {
                if (reader.Read()) {
                    string username = reader["username"].ToString();
                    int coins = Convert.ToInt32(reader["coins"]);
                    int elo = Convert.ToInt32(reader["elo"]);
                    string bio = reader["bio"].ToString();
                    string image = reader["image"].ToString();
                    int wins = Convert.ToInt32(reader["wins"]);
                    int losses = Convert.ToInt32(reader["losses"]);
                    float winrate = losses == 0 ? 100 : (float)wins / losses;
                    return username + "\n " + bio + " " + image + "\n Coins: " + coins + "\nElo: " + elo + " | Winrate: " + wins + "/" + losses + " (" + winrate + "%)\n";
                }
            }
        }

        return "";
    }
    public List<string> getDeckOfUser(int userID)
    {
        List<string> cards = new List<string>();
            
        using (NpgsqlCommand command = new NpgsqlCommand("select userCards.card_id from userCards where id = @userID and indeck = true;", connection))
        {
            command.Parameters.AddWithValue("userID", userID);
            using (NpgsqlDataReader reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    string cardId = reader["card_id"].ToString() ?? "";
                    cards.Add(cardId);
                }
            }
        }
        return cards;
    }

    public void updateProfile(int userID, string username, string bio, string image)
    {
        using (NpgsqlCommand command = new NpgsqlCommand("update users set username = @username, bio = @bio, image = @image  where id = @userID", connection)) {
            command.Parameters.AddWithValue("username", username);
            command.Parameters.AddWithValue("bio", bio);
            command.Parameters.AddWithValue("image", image);
            command.Parameters.AddWithValue("userID", userID);
            command.ExecuteNonQuery();
        }

    }

    public List<string> getScoreboard()
    {
        List<string> users = new List<string>();
        
        using (NpgsqlCommand command = new NpgsqlCommand("select image, username, elo, wins, losses from users where admin = false order by elo limit 5", connection))
        {
            using (NpgsqlDataReader reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    string image = reader["image"].ToString();
                    string username = reader["username"].ToString();
                    int elo = Convert.ToInt32(reader["elo"]);
                    int wins = Convert.ToInt32(reader["wins"]);
                    int losses = Convert.ToInt32(reader["losses"]);
                    float winrate = losses == 0 ? 100 : (float)wins / losses;
                    users.Add(image + " " + username + " Elo: " + elo + " | Winrate: " + wins + "/" + losses + " (" + winrate + "%)");
                }
            }
        }

        return users;
    }
}

