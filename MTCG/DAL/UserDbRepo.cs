using System.Data;
using Npgsql;
using MTCG.Models;

namespace MTCG.DAL;

public class UserDbRepo
{
    
    private NpgsqlConnection _connection;
    public UserDbRepo(NpgsqlConnection connection) {
        this._connection = connection;
    }

    
    public bool LoginUser(string username, string password) {
        using (var command = new NpgsqlCommand("SELECT * FROM users WHERE username = @username AND password = @password", _connection))
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
        using (var command = new NpgsqlCommand("SELECT * FROM users WHERE username = @username", _connection))
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
    
    
    public bool UsernameExistsBesidesSelf(int userId, string username) {
        using (var command = new NpgsqlCommand("SELECT * FROM users WHERE username = @username and id != @userID", _connection))
        {
            command.Parameters.AddWithValue("username", username);
            command.Parameters.AddWithValue("userID", userId);
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
    
    public bool IsAdmin(string username) {
        using (var command = new NpgsqlCommand("SELECT admin FROM users WHERE username = @username", _connection))
        {
            command.Parameters.AddWithValue("username", username);
            var result = command.ExecuteScalar();
            return result != null && (bool)result;
        }
    }

    public void SetUserData(User user) {
        
        using (var command = new NpgsqlCommand("SELECT * FROM users WHERE username = @username", _connection)) {
            command.Parameters.AddWithValue("username", user.GetUsername());
            using (var reader = command.ExecuteReader()) {
                if (reader.Read()) {
                    user.Id = Convert.ToInt32(reader["id"]);
                    user.Username = reader["username"].ToString();
                    user.Coins = Convert.ToInt32(reader["coins"]);
                    user.Elo = Convert.ToInt32(reader["elo"]);
                    user.IsAdmin = Convert.ToBoolean(reader["admin"]);
                    user.TimeActive = DateTime.Now;
                }
            }
        }

        List<Card> cardDeck = new List<Card>();
        using (NpgsqlCommand command = new NpgsqlCommand("select cards.card_id, cards.card_name, cards.card_type, cards.damage_value, cards.element_type from userCards inner join cards on cards.card_id = userCards.card_id where userCards.id = @userID and userCards.inDeck = true", _connection)) {
            command.Parameters.AddWithValue("userID", user.Id);
            using (NpgsqlDataReader reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    Card card = new Card {
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
        user.UpdateDeck(cardDeck);
    }

    public void Add(User user) {
        const string insertQuery = "INSERT INTO users (username, password) VALUES (@username, @password)";
        using (var insertCmd = new NpgsqlCommand(insertQuery, _connection))
        {
            insertCmd.Parameters.AddWithValue("@username", user.GetUsername());
            insertCmd.Parameters.AddWithValue("@password", user.GetPassword());
            insertCmd.ExecuteNonQuery();
        }
    }


    public void AddCardsToUser(int userId, string cardId) {
        const string insertQuery = "INSERT INTO userCards (id, card_id) VALUES (@userID, @cardID)";
        Console.WriteLine(userId);
        Console.WriteLine(cardId);
        using (var insertCmd = new NpgsqlCommand(insertQuery, _connection))
        {
            insertCmd.Parameters.AddWithValue("@userID", userId);
            insertCmd.Parameters.AddWithValue("@cardID", cardId);
            insertCmd.ExecuteNonQuery();
        }
    }


    public void SubtractCoins(int userId, int price) {
        string updateQuery = "UPDATE users SET coins = coins - @coinsToSubtract WHERE id = @userId";
        // Specify the number of coins to subtract
        using (NpgsqlCommand updateCommand = new NpgsqlCommand(updateQuery, _connection))
        {
            // Add parameters to the update command
            updateCommand.Parameters.AddWithValue("@coinsToSubtract", price);
            updateCommand.Parameters.AddWithValue("@userId", userId);
            updateCommand.ExecuteNonQuery();
        }
    }

    public bool UserOwnsCard(int userId, string cardId) {
        using (var command = new NpgsqlCommand("SELECT * FROM userCards WHERE id = @userID AND card_ID = @cardID", _connection))
        {
            command.Parameters.AddWithValue("userID", userId);
            command.Parameters.AddWithValue("cardID", cardId);
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
    
    
    public List<string> GetCardsOfUser(int userId) {
        List<string> cards = new List<string>();
        
        using (NpgsqlCommand command = new NpgsqlCommand("select userCards.card_id from userCards inner join cards on cards.card_id = userCards.card_id where userCards.id = @userID order by damage_value desc;", _connection))
        {
            command.Parameters.AddWithValue("userID", userId);
            using (NpgsqlDataReader reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    string cardId = reader["card_id"].ToString() ?? "";
                    cards.Add(cardId);
                }
            }
        }
        return cards;
    }
    
    
    public List<Card> UpdateDeck(int userId, List<string> deck) {
        List<Card> cardDeck = new List<Card>();
        using (NpgsqlCommand command = new NpgsqlCommand("update userCards set indeck = false where id = @userID", _connection)) {
            command.Parameters.AddWithValue("userID", userId);
            command.ExecuteNonQuery();
        }
        
        using (NpgsqlCommand command = new NpgsqlCommand("update userCards set indeck = true where card_id in (@card1, @card2, @card3, @card4)", _connection)) {
            command.Parameters.AddWithValue("card1", deck[0]);
            command.Parameters.AddWithValue("card2", deck[1]);
            command.Parameters.AddWithValue("card3", deck[2]);
            command.Parameters.AddWithValue("card4", deck[3]);
            command.ExecuteNonQuery();
        }

        using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM cards WHERE card_id in (@card1, @card2, @card3, @card4)", _connection)) {
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

    public string GetUserProfile(int userId) {
        using (var command = new NpgsqlCommand("SELECT * FROM users WHERE id = @userID", _connection)) {
            command.Parameters.AddWithValue("userID", userId);
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
    public List<string> GetDeckOfUser(int userId)
    {
        List<string> cards = new List<string>();
            
        using (NpgsqlCommand command = new NpgsqlCommand("select userCards.card_id from userCards where id = @userID and indeck = true;", _connection))
        {
            command.Parameters.AddWithValue("userID", userId);
            using (NpgsqlDataReader reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    string cardId = reader["card_id"].ToString() ?? "";
                    cards.Add(cardId);
                }
            }
        }
        return cards;
    }

    public void UpdateProfile(int userId, string username, string bio, string image)
    {
        using (NpgsqlCommand command = new NpgsqlCommand("update users set username = @username, bio = @bio, image = @image  where id = @userID", _connection)) {
            command.Parameters.AddWithValue("username", username);
            command.Parameters.AddWithValue("bio", bio);
            command.Parameters.AddWithValue("image", image);
            command.Parameters.AddWithValue("userID", userId);
            command.ExecuteNonQuery();
        }

    }

    public List<string> GetScoreboard()
    {
        List<string> users = new List<string>();
        
        using (NpgsqlCommand command = new NpgsqlCommand("select image, username, elo, wins, losses from users where admin = false order by elo desc limit 5", _connection))
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


    public void AddBattle(int player1Id, int player2Id, float result, DateTime date, int turns)
    {
        const string insertQuery = "INSERT INTO battles (player1, player2, result, date, turns) VALUES (@user1ID, @user2ID, @result, @date, @turns)";
        using (var insertCmd = new NpgsqlCommand(insertQuery, _connection))
        {
            insertCmd.Parameters.AddWithValue("@user1ID", player1Id);
            insertCmd.Parameters.AddWithValue("@user2ID", player2Id);
            insertCmd.Parameters.AddWithValue("@result", result);
            insertCmd.Parameters.AddWithValue("@date", date);
            insertCmd.Parameters.AddWithValue("@turns", turns);
            insertCmd.ExecuteNonQuery();
        }

    }

    public void UpdateElo(int userId, int eloChange, int win, int loss) {
        string updateQuery = "UPDATE users SET elo = elo + @eloChange, wins = wins + @win, losses = losses + @loss WHERE id = @userID";
        using (NpgsqlCommand updateCommand = new NpgsqlCommand(updateQuery, _connection))
        {
            // Add parameters to the update command
            updateCommand.Parameters.AddWithValue("@eloChange", eloChange);
            updateCommand.Parameters.AddWithValue("@userID", userId);
            updateCommand.Parameters.AddWithValue("@win", win);
            updateCommand.Parameters.AddWithValue("@loss", loss);
            updateCommand.ExecuteNonQuery();
        }
    }
}

