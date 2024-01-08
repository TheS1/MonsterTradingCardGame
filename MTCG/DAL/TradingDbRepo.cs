using Npgsql;

namespace MTCG.DAL;

public class TradingDbRepo
{
    private NpgsqlConnection _connection;
    public TradingDbRepo(NpgsqlConnection connection) {
        this._connection = connection;
    }
    
    
    public List<string> GetTradedeals()
    {
        List<string> tradedeals = new List<string>();
        
        using (NpgsqlCommand command = new NpgsqlCommand("select tradings.trade_id, users.username, cards.card_name, cards.card_type, cards.element_type, cards.damage_value, tradings.desired_type, tradings.minimum_damage from tradings inner join users on users.id = tradings.user_id inner join cards on cards.card_id = tradings.card_offer", _connection))
        {
            using (NpgsqlDataReader reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    int tradeId = Convert.ToInt32(reader["trade_id"]);
                    string username = reader["username"].ToString();
                    string cardname = reader["card_name"].ToString();
                    string cardType = reader["card_type"].ToString();
                    string elementType = reader["element_type"].ToString();
                    int damageValue = Convert.ToInt32(reader["damage_value"]);
                    string desiredType = reader["desired_type"].ToString();
                    int minimumDamage = Convert.ToInt32(reader["minimum_damage"]);
                    tradedeals.Add( username + " Offer: |" + cardname + ", " + cardType + ", " + elementType + ", " + damageValue + "| for " + desiredType + " " + minimumDamage + " | Trade ID:" +tradeId);
                }
            }
        }

        return tradedeals;
    }

    public void PostTrade(string cardId, int userId, string desiredType, double minimumDamage)
    {   
        
        const string insertQuery = "insert into tradings (user_id, card_offer, desired_type, minimum_damage) values (@userId, @cardId, @desired_type, @minimum_damage)";
        using (var insertCmd = new NpgsqlCommand(insertQuery, _connection))
        {
            insertCmd.Parameters.AddWithValue("@userId", userId);
            insertCmd.Parameters.AddWithValue("@cardId", cardId);
            insertCmd.Parameters.AddWithValue("@desired_type", desiredType);
            insertCmd.Parameters.AddWithValue("@minimum_damage", minimumDamage);
            insertCmd.ExecuteNonQuery();
        }
    }

    public bool OfferIdExists(int tradeId)
    {
        using (var command = new NpgsqlCommand("select * from tradings where trade_id = @tradeId", _connection)) {
            command.Parameters.AddWithValue("tradeId", tradeId);
            using (var reader = command.ExecuteReader()) {
                if (reader.Read()) {
                    return true;
                }
            }
        }
        return false;
    }
    
    public bool ThisUserPostedTradeOffer(int userId, int tradeId)
    {
        using (var command = new NpgsqlCommand("select * from tradings where user_id = @userId and trade_id = @tradeId", _connection))
        {
            command.Parameters.AddWithValue("userId", userId);
            command.Parameters.AddWithValue("tradeId", tradeId);
            using (var reader = command.ExecuteReader())
            {
                if (!reader.Read())
                {
                    return false;
                }
            }
            
        }
        return true;
    }

    public bool TradingRequirementsMet(int tradeId, string cardId)
    {
        string desired_type = "";
        string card_type = "";
        int minimum_damage = 0;
        int card_damage = 0;
        using (var command = new NpgsqlCommand("select desired_type, minimum_damage from tradings where trade_id = @trade_id", _connection))
        {
            command.Parameters.AddWithValue("tradeId", tradeId);
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    desired_type = reader["desired_type"].ToString();
                    minimum_damage = Convert.ToInt32(reader["minimum_damage"]);
                }
            }
        }
        using (var command = new NpgsqlCommand("select card_type, damage_value from cards where card_id = @card_id", _connection))
        {
            command.Parameters.AddWithValue("card_id", cardId);
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    card_type = reader["card_type"].ToString();
                    card_damage = Convert.ToInt32(reader["damage_value"]);
                }
            }
        }
        
        Console.WriteLine(desired_type);
        Console.WriteLine(card_type);
        Console.WriteLine(card_damage);
        Console.WriteLine(minimum_damage);
        if (desired_type == card_type && card_damage >= minimum_damage) {
            return true;
        }

        return false;
    }

    public void completeTrade(int userId, int tradeId, string cardId)
    {
        int user2Id = 0;
        string card2Id = "";
        using (var command = new NpgsqlCommand("select users.id from users inner join usercards on usercards.id = users.id where card_id = @cardId", _connection)) {
            command.Parameters.AddWithValue("cardId", cardId);
            using (var reader = command.ExecuteReader()) {
                if (reader.Read()) {
                    user2Id =  Convert.ToInt32(reader["id"]);
                }
            }
        }
        using (var command = new NpgsqlCommand("select card_offer from tradings where trade_id = @tradeId", _connection)) {
            command.Parameters.AddWithValue("tradeId", tradeId);
            using (var reader = command.ExecuteReader()) {
                if (reader.Read()) {
                    card2Id = reader["card_offer"].ToString();
                }
            }
        }
        
        using (NpgsqlCommand deleteCommand = new NpgsqlCommand("delete from usercards where card_id = @card_id and id = @userId", _connection)) {
            deleteCommand.Parameters.AddWithValue("@card_id", cardId);
            deleteCommand.Parameters.AddWithValue("@userId", userId);
            deleteCommand.ExecuteNonQuery();
            deleteCommand.Parameters.Clear();
            deleteCommand.Parameters.AddWithValue("@card_id", card2Id);
            deleteCommand.Parameters.AddWithValue("@userId", user2Id);
            deleteCommand.ExecuteNonQuery();
        }
        
        
        using (NpgsqlCommand insertCommand = new NpgsqlCommand("insert into usercards values(@userId, @card_id)", _connection)) {
            insertCommand.Parameters.AddWithValue("@card_id", cardId);
            insertCommand.Parameters.AddWithValue("@userId", userId);
            insertCommand.ExecuteNonQuery();
            insertCommand.Parameters.Clear();
            insertCommand.Parameters.AddWithValue("@card_id", card2Id);
            insertCommand.Parameters.AddWithValue("@userId", user2Id);
            insertCommand.ExecuteNonQuery();
        }
        

    }

    public void DeleteOffer(int tradeId)
    {
        using (var deleteCmd = new NpgsqlCommand("delete from tradings where trade_id = @tradeId", _connection))
        {
            deleteCmd.Parameters.AddWithValue("tradeId", tradeId);
            deleteCmd.ExecuteNonQuery();

        }
    }

    public bool CardAlreadyPosted(string cardId) {
        using (var command = new NpgsqlCommand("select * from tradings where card_offer = @cardId", _connection)) {
            command.Parameters.AddWithValue("cardId", cardId);
            using (var reader = command.ExecuteReader()) {
                if (reader.Read()) {
                    return true;
                }
            }
        }
        return false;
    }
}