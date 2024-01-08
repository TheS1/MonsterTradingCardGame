using System.Text.Json;
using MTCG.DAL;
using MTCG.Models;

namespace MTCG.Server;

public static class RouteHandler
{
    private static readonly object DbLocker = new object();
    public static void RootRoute(StreamWriter writer)
    {
        string responseString = "root";
        Server.SendResponse(writer, responseString);
    }

    // login route
    public static void Login(StreamWriter writer, string method, string contentType, string data) {
        Console.WriteLine(contentType);
        if (method == "POST") {
            if (contentType == " application/json") {
                User user = JsonSerializer.Deserialize<User>(data) ?? new User();
                lock (DbLocker) {
                    if (DbManager.LoginUser(user))
                    {
                        Server.SessionUsers[user.GetUsername() + "-mtcgToken"] = user;

                        string responseString = "Logged in!";
                        Server.SendResponse(writer, responseString);
                    }
                    else
                    {
                        string responseString = "Wrong username or password";
                        Server.SendResponse(writer, responseString);
                    }
                }
            }
        }
        else {
            string responseString = "Wrong request type expected POST";
            Server.SendResponse(writer, responseString);
        }
    }


    public static void Register(StreamWriter writer, string method, string contentType, string data) {
        if (method == "POST") {
            if (contentType == " application/json") {
                User user = JsonSerializer.Deserialize<User>(data) ?? new User();
                lock (DbLocker)
                {
                    if (DbManager.RegisterUser(user))
                    {
                        Server.SessionUsers.Add(user.GetUsername() + "-mtcgToken", user);
                        string responseString = "Registered successfully!";
                        Server.SendResponse(writer, responseString);
                    }
                    else
                    {

                        string responseString = "Username already exists!";
                        Server.SendResponse(writer, responseString);
                    }
                }
            }
            else {
                string responseString = "Data must be in json format";
                Server.SendResponse(writer, responseString);
            }
        }else {
            string responseString = "Wrong request type expected POST";
            Server.SendResponse(writer, responseString);
        }
    }
    
    
    public static void AddCards(StreamWriter writer, string method, string data) {
        
        if (method == "POST") {
            string responseString;

            JsonDocument jsonDocument = JsonDocument.Parse(data);

                // Iterate through the JSON array and print the values
                foreach (JsonElement element in jsonDocument.RootElement.EnumerateArray())
                {
                    string id = element.GetProperty("Id").GetString();
                    string name = element.GetProperty("Name").GetString();
                    double damage = element.GetProperty("Damage").GetDouble();
                    string set = element.TryGetProperty("Set", out _) ? element.GetProperty("Set").GetString() : "Basic";
                    string type = "Monster";
                    if (name.Contains("Spell")) {
                        type = "Spell";
                    }
                    string elementType = "Regular";
                    if (name.Contains("Water", StringComparison.OrdinalIgnoreCase))
                        elementType = "Water";
                    else if (name.Contains("Fire", StringComparison.OrdinalIgnoreCase))
                        elementType = "Fire";
                    lock (DbLocker)
                    {
                        if (!DbManager.AddCards(id, name, damage, type, elementType, set))
                        {
                            responseString = "Error adding card \"" + name + "\"";
                            Server.SendErrorResponse(writer, responseString, 400);
                            return;
                        }
                    }
                }
            responseString = "Added all Cards successfully ";
            Server.SendResponse(writer, responseString);

        } else {
            string responseString = "Wrong request type";
            Server.SendErrorResponse(writer, responseString, 400);
        }
        
    }




    public static void BuyPack(StreamWriter writer, string method, string authorizationToken, string data)
    {
        Console.WriteLine("Token:" + authorizationToken);
        if (method == "POST")
        {
            if (Server.IsLoggedIn(authorizationToken))
            {
                string responseString;
                JsonDocument jsonDocument = JsonDocument.Parse(data);
                lock (DbLocker)
                {
                    int setId = DbManager.CheckSet(jsonDocument.RootElement.GetProperty("Set").GetString());
                    if (setId == -99)
                    {
                        responseString = "Invalid Set, check available sets with /sets";
                        Server.SendErrorResponse(writer, responseString, 400);
                        return;
                    }

                    if (!DbManager.CheckUserEnoughCoins(Server.SessionUsers[authorizationToken].Coins, setId))
                    {
                        responseString = "Not enough coins, check sets with /sets or coins with /profile";
                        Server.SendErrorResponse(writer, responseString, 400);
                        return;
                    }

                    List<string> cardsDrawn = DbManager.BuyPack(Server.SessionUsers[authorizationToken].Id, setId);
                    if (cardsDrawn.Count == 5) {
                        Server.SessionUsers[authorizationToken].Coins -= 5;
                        responseString = "Cards drawn: \n" + DbManager.GetAllCards(cardsDrawn);
                    }
                    else
                    {
                        responseString = "Not enough cards left in this set";
                    }
                    Server.SendResponse(writer, responseString);
                }
            }
            else
            {
                string responseString = "Not logged in";
                Server.SendResponse(writer, responseString);
            }

        }
        else
        {
            string responseString = "Wrong request type";
            Server.SendErrorResponse(writer, responseString, 400);
        }

    }


    public static void ShowCards(StreamWriter writer, string method, string authorizationToken){
        if (method == "GET") {
            if (Server.IsLoggedIn(authorizationToken)) {
                string responseString = "Your Cards: \n" + DbManager.GetAllUserCards(Server.SessionUsers[authorizationToken].Id);
                Server.SendResponse(writer, responseString);
            }
            else
            {
                string responseString = "Not Logged in";
                Server.SendErrorResponse(writer, responseString, 401);
            }
        } else {
            string responseString = "Wrong request type";
            Server.SendErrorResponse(writer, responseString, 400);
        }

    }

    public static void DeckManagement(StreamWriter writer, string method, string authorizationToken, string data) {
        if (Server.IsLoggedIn(authorizationToken)) {
            lock (DbLocker)
            {
                if (method == "GET")
                {
                    string responseString = "Your Deck: \n" +
                                            DbManager.GetUserDeck(Server.SessionUsers[authorizationToken].Id);
                    Server.SendResponse(writer, responseString);

                }
                else if (method == "PUT")
                {
                    List<string> deck = JsonSerializer.Deserialize<List<string>>(data);

                    if (DbManager.UpdateDeck(Server.SessionUsers[authorizationToken], deck))
                    {
                        string responseString = "Updated Deck!";
                        Server.SendResponse(writer, responseString);
                    }
                    else
                    {
                        string responseString = "You dont own one or more of these cards";
                        Server.SendResponse(writer, responseString);
                    }

                }
                else
                {
                    string responseString = "Wrong request type";
                    Server.SendErrorResponse(writer, responseString, 400);
                }
            }
        } else {
            string responseString = "Not Logged in";
            Server.SendErrorResponse(writer, responseString, 401);
        }
    }

    public static void UserProfile(StreamWriter writer, string method, string authorizationToken, string data)
    {
        if (Server.IsLoggedIn(authorizationToken)) {
            lock (DbLocker)
            {
                if (method == "GET")
                {
                    string responseString = DbManager.GetUserProfile(Server.SessionUsers[authorizationToken].Id);
                    Server.SendResponse(writer, responseString);

                }
                else if (method == "PUT")
                {
                    JsonDocument jsonDocument = JsonDocument.Parse(data);
                    string username = jsonDocument.RootElement.GetProperty("Name").GetString();
                    string bio = jsonDocument.RootElement.GetProperty("Bio").GetString();
                    string image = jsonDocument.RootElement.GetProperty("Image").GetString();

                    if (DbManager.UpdateProfile(Server.SessionUsers[authorizationToken].Id, username, bio, image))
                    {
                        Server.SessionUsers[username + "-mtcgToken"] = Server.SessionUsers[authorizationToken];
                        Server.SessionUsers.Remove(authorizationToken);
                        string responseString = "Updated Profile!";
                        Server.SendResponse(writer, responseString);
                    }
                    else
                    {
                        string responseString = "New Username is taken!";
                        Server.SendResponse(writer, responseString);
                    }
                }
                else
                {
                    string responseString = "Wrong request type";
                    Server.SendErrorResponse(writer, responseString, 400);
                }
            }
        } else {
            string responseString = "Not Logged in";
            Server.SendErrorResponse(writer, responseString, 401);
        }
    }

    public static void Scoreboard(StreamWriter writer, string method) {
        if (method == "GET") {
            lock (DbLocker)
            {
                string responseString = DbManager.GetScoreboard();
                Server.SendResponse(writer, responseString);
            }
        }
        else
        {
            string responseString = "Wrong request type";
            Server.SendErrorResponse(writer, responseString, 400);
        }
    }

    public static void Battle(StreamWriter writer, string method, string authorizationToken)
    {
        if (Server.IsLoggedIn(authorizationToken))
        {
            if (method == "POST")
            {
                string deckConf;
                lock (DbLocker)
                {
                    deckConf = DbManager.GetUserDeck(Server.SessionUsers[authorizationToken].Id);
                }

                if (deckConf == "Deck not configured")
                {
                    Server.SendResponse(writer, deckConf);
                    return;
                }

                Server.BattleManager.EnterMatchmaking(Server.SessionUsers[authorizationToken]);
                string battleLog = Server.BattleManager.WaitForResult();
                Server.SendResponse(writer, battleLog);
                

            }
            else {
                string responseString = "Wrong request type";
                Server.SendErrorResponse(writer, responseString, 400);
            }
        }
        else
        {
            string responseString = "Not logged in";
            Server.SendErrorResponse(writer, responseString, 401);
        }
    }

    public static void Tradings(StreamWriter writer, string method, string authorizationToken, string data)
    {
        if (Server.IsLoggedIn(authorizationToken)) {
            if (method == "GET") {
                string responseString = DbManager.GetTradedeals();
                Server.SendResponse(writer, responseString);

            }
            else if (method == "POST")
            {
                JsonDocument jsonDocument = JsonDocument.Parse(data);


                string id = jsonDocument.RootElement.GetProperty("Id").GetString();
                string type = jsonDocument.RootElement.GetProperty("Type").GetString();
                double damage = jsonDocument.RootElement.GetProperty("MinimumDamage").GetDouble();

                string responseString;
                lock (DbLocker)
                {


                    if (DbManager.PostTradeOffer(id, Server.SessionUsers[authorizationToken].Id, type, damage))
                    {
                        responseString = "Posted Trade Offer!";
                    }
                    else
                    {
                        responseString = "You dont own this card";
                    }

                    Server.SendResponse(writer, responseString);
                }
            }
            else
            {
                string responseString = "Wrong request type";
                Server.SendErrorResponse(writer, responseString, 400);
            }
        } else {
            string responseString = "Not Logged in";
            Server.SendErrorResponse(writer, responseString, 401);
        }
    }

    public static void Trade(StreamWriter writer, string method, string path, string authorizationToken, string data)
    {
        if (Server.IsLoggedIn(authorizationToken)) {
            if (method == "POST") {
                int tradeId = Convert.ToInt32(path.Substring(path.LastIndexOf('/') + 1));
                
                Console.WriteLine("ok:" + tradeId);
                JsonDocument jsonDocument = JsonDocument.Parse(data);

                string cardId = jsonDocument.RootElement.GetProperty("Id").GetString();

                lock (DbLocker)
                {
                    string responseString =
                        DbManager.TryTrading(Server.SessionUsers[authorizationToken].Id, tradeId, cardId);
                    Server.SendResponse(writer, responseString);
                }
            }
            else
            {
                string responseString = "Wrong request type";
                Server.SendErrorResponse(writer, responseString, 400);
            }
        } else {
            string responseString = "Not Logged in";
            Server.SendErrorResponse(writer, responseString, 401);
        }
    }
    
    public static void NotFound(StreamWriter writer) {
        // Handle 404 Not Found
        string responseString = "Not Found";
        Server.SendErrorResponse(writer, responseString, 404);
    }
}