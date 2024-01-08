using Npgsql;

namespace MTCG.DAL;

public class CardDbRepo
{
    private NpgsqlConnection _connection;
    public CardDbRepo(NpgsqlConnection connection) {
        this._connection = connection;
    }
    
    public bool AddCard(string cardId, string cardName, double cardDamage, string cardType, string elementType, int cardSetId)
    {
        using (var command = new NpgsqlCommand(
                   "INSERT INTO cards (card_id, card_name, element_type, card_type, damage_value, card_set) " +
                   "VALUES (@CardId, @CardName, @ElementType, @CardType, @DamageValue, @CardSetId)",
                   _connection))
            {
                command.Parameters.AddWithValue("CardId", cardId);
                command.Parameters.AddWithValue("CardName", cardName);
                command.Parameters.AddWithValue("ElementType", elementType);
                command.Parameters.AddWithValue("CardType", cardType);
                command.Parameters.AddWithValue("DamageValue", cardDamage);
                command.Parameters.AddWithValue("CardSetId", cardSetId);

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
    }

    public int CardSetExists(string setName)
    {
        using (var command = new NpgsqlCommand("SELECT set_id FROM cardSets WHERE set_name = @setName", _connection))
        {
            command.Parameters.AddWithValue("setName", setName);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return reader.GetInt32(reader.GetOrdinal("set_id"));
                };
                    
            }
        }
        return -99;
    }


    public List<string> DrawPack(int setId)
    {
        List<string> cardsDrawn = new List<string>();
        
        using (NpgsqlCommand command = new NpgsqlCommand("select * from cards where card_set = @setID and card_id not in (select card_id from UserCards) order by random() limit 5", _connection))
        {
            command.Parameters.AddWithValue("setID", setId);
            using (NpgsqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Retrieve the ID column and add it to the list
                    string id = reader["card_id"].ToString();
                    cardsDrawn.Add(id);
                }
            }
        }
        return cardsDrawn;
    }


    public string GetCardInfoById(string cardId) {
        Console.WriteLine("Id:" + cardId);
        using (NpgsqlCommand command = new NpgsqlCommand("select * from cards where card_id = @cardID order by damage_value desc", _connection)){
            command.Parameters.AddWithValue("cardID", cardId);
            using (NpgsqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Retrieve the ID column and add it to the list
                    string id = reader["card_id"].ToString();
                    string name = reader["card_name"].ToString();
                    string elementType = reader["element_type"].ToString();
                    string cardType = reader["card_type"].ToString();
                    string damageValue = reader["damage_value"].ToString();

                    return " " + id + ": " + name + ", " + elementType + ", " + cardType + ", Damage: " + damageValue;
                }
            }
        }

        return "";
    }
    public int GetCardSetPrice(int setId)
    {
        using (var command = new NpgsqlCommand("SELECT set_price FROM cardSets WHERE set_id = @setID", _connection)) {
            command.Parameters.AddWithValue("setID", setId);

            using (var reader = command.ExecuteReader()) {
                if (reader.Read()) {
                    return reader.GetInt32(reader.GetOrdinal("set_price"));
                };
                    
            }
        }
        return -99;
    }
}