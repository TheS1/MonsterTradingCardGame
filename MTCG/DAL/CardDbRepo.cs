using Npgsql;

namespace MTCG.DAL;

public class CardDbRepo
{
    private NpgsqlConnection connection;
    public CardDbRepo(NpgsqlConnection connection) {
        this.connection = connection;
    }
    
    public bool AddCard(string cardId, string cardName, double cardDamage, string cardType, string elementType, int cardSetId)
    {
        using (var command = new NpgsqlCommand(
                   "INSERT INTO cards (card_id, card_name, element_type, card_type, damage_value, card_set) " +
                   "VALUES (@CardId, @CardName, @ElementType, @CardType, @DamageValue, @CardSetId)",
                   connection))
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
        using (var command = new NpgsqlCommand("SELECT set_id FROM cardSets WHERE set_name = @setName", connection))
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


    public List<string> drawPack(int setID)
    {
        List<string> CardsDrawn = new List<string>();
        
        using (NpgsqlCommand command = new NpgsqlCommand("select * from cards where card_set = @setID and card_id not in (select card_id from UserCards) order by random() limit 5", connection))
        {
            command.Parameters.AddWithValue("setID", setID);
            using (NpgsqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Retrieve the ID column and add it to the list
                    string id = reader["card_id"].ToString();
                    CardsDrawn.Add(id);
                }
            }
        }
        return CardsDrawn;
    }


    public string getCardInfoByID(string cardID) {
        Console.WriteLine("Id:" + cardID);
        using (NpgsqlCommand command = new NpgsqlCommand("select * from cards where card_id = @cardID order by damage_value desc", connection)){
            command.Parameters.AddWithValue("cardID", cardID);
            using (NpgsqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Retrieve the ID column and add it to the list
                    string id = reader["card_id"].ToString();
                    string name = reader["card_name"].ToString();
                    string element_type = reader["element_type"].ToString();
                    string card_type = reader["card_type"].ToString();
                    string damage_value = reader["damage_value"].ToString();

                    return " " + id + ": " + name + ", " + element_type + ", " + card_type + ", Damage: " + damage_value;
                }
            }
        }

        return "";
    }
    public int GetCardSetPrice(int setID)
    {
        using (var command = new NpgsqlCommand("SELECT set_price FROM cardSets WHERE set_id = @setID", connection)) {
            command.Parameters.AddWithValue("setID", setID);

            using (var reader = command.ExecuteReader()) {
                if (reader.Read()) {
                    return reader.GetInt32(reader.GetOrdinal("set_price"));
                };
                    
            }
        }
        return -99;
    }
}