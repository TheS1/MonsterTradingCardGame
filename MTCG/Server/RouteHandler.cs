using System.Net;
using System.Text.Json;
using MTCG.DAL;
using MTCG.Models;
using Npgsql;

namespace MTCG.Server;

public static class RouteHandler
{
    // root route
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
                User? user = JsonSerializer.Deserialize<User>(data);
                if (DbManager.loginUser(user)) {
                    Server.sessionUsers[user.getUsername() + "-mtcgToken"] = user;
                    
                    string responseString = "Logged in!";
                    Server.SendResponse(writer, responseString);
                }
                else {
                    string responseString = "Wrong username or password";
                    Server.SendResponse(writer, responseString);
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
                User user = JsonSerializer.Deserialize<User>(data);
                if (DbManager.registerUser(user)) {
                    Server.sessionUsers.Add(user.getUsername() + "-mtcgToken",user);
                    string responseString = "Registered successfully!";
                    Server.SendResponse(writer, responseString);
                }
                else {
                    
                    string responseString = "Username already exists!";
                    Server.SendResponse(writer, responseString);
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



    
    public static void Logout(StreamWriter writer, string method) {
        /*if (method == "GET") {
            if (IsLoggedIn())
                {
                    
                    string responseString = "Logged out successfully!";
                    Server.SendResponse(writer, responseString);
                }
                else
                {
                    string responseString = "Not Logged in";
                    Server.SendResponse(writer, responseString);
                }
            }
            else {
                string responseString = "Didnt send cookie file -> curl ... -b cookies.txt";
                Server.SendResponse(writer, responseString);
            }
        }
        else {
            string responseString = "Wrong request type expecting get";
            Server.SendErrorResponse(writer, responseString);
        }
        */
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

                    if (!DbManager.addCards(id, name, damage, type, elementType, set)) {
                        responseString = "Error adding card \"" + name + "\"";
                        Server.SendErrorResponse(writer, responseString, 400);
                        return;
                    } 
                    
                }
            responseString = "Added all Cards successfully ";
            Server.SendResponse(writer, responseString);

        } else {
            string responseString = "Wrong request type";
            Server.SendErrorResponse(writer, responseString, 400);
        }
        
    }
    


    
    public static void BuyPack(StreamWriter writer, string method, string authorizationToken, string data) {
        Console.WriteLine("Token:" +authorizationToken);
        if (method == "POST") {
            if (Server.IsLoggedIn(authorizationToken)) {
                string responseString;
                JsonDocument jsonDocument = JsonDocument.Parse(data);
                int setID = DbManager.checkSet(jsonDocument.RootElement.GetProperty("Set").GetString());
                if (setID == -99) {
                    responseString = "Invalid Set, check available sets with /sets";
                    Server.SendErrorResponse(writer, responseString, 400);
                    return;
                }

                if (!DbManager.checkUserEnoughCoins(Server.sessionUsers[authorizationToken].coins, setID)) {
                    responseString = "Not enough coins, check sets with /sets or coins with /profile";
                    Server.SendErrorResponse(writer, responseString, 400);
                    return;
                }

                List<string> CardsDrawn = DbManager.buyPack(Server.sessionUsers[authorizationToken].id, setID);
                Server.sessionUsers[authorizationToken].coins -= 5;
                responseString = "Cards drawn: \n" + DbManager.getAllCards(CardsDrawn);
                Server.SendResponse(writer, responseString);
            }
            else {
                string responseString = "Not logged in";
                Server.SendResponse(writer, responseString);
            }
            
        } else {
            string responseString = "Wrong request type";
            Server.SendErrorResponse(writer, responseString, 400);
        }
        
    }

    
    
    public static void NotFound(StreamWriter writer) {
        // Handle 404 Not Found
        string responseString = "Not Found";
        Server.SendErrorResponse(writer, responseString, 404);
    }


    public static bool SentCookieFile(HttpListenerRequest request) { 
        return true;
    }
    



    
    
    
    
    private static User LoadUser(string username) {
        //load user data out of database based on username, database doesnt exist yet
        return new User();
    }

    public static void showCards(StreamWriter writer, string method, string authorizationToken){
        if (method == "GET") {
            if (Server.IsLoggedIn(authorizationToken)) {
                string responseString = "Your Cards: \n" + DbManager.getAllUserCards(Server.sessionUsers[authorizationToken].id);
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

    public static void deckManagement(StreamWriter writer, string method, string authorizationToken, string data) {
        if (Server.IsLoggedIn(authorizationToken)) {
            if (method == "GET") {
                string responseString = "Your Deck: \n" +
                                        DbManager.getUserDeck(Server.sessionUsers[authorizationToken].id);
                Server.SendResponse(writer, responseString);

            }
            else if (method == "PUT") {
                List<string> deck = JsonSerializer.Deserialize<List<string>>(data);
                
                if (DbManager.updateDeck(Server.sessionUsers[authorizationToken], deck)) {
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
        } else {
            string responseString = "Not Logged in";
            Server.SendErrorResponse(writer, responseString, 401);
        }
    }

    public static void userProfile(StreamWriter writer, string method, string authorizationToken, string data)
    {
        if (Server.IsLoggedIn(authorizationToken)) {
            if (method == "GET") {
                string responseString = DbManager.getUserProfile(Server.sessionUsers[authorizationToken].id);
                Server.SendResponse(writer, responseString);

            }
            else if (method == "PUT") {
                JsonDocument jsonDocument = JsonDocument.Parse(data);
                string username = jsonDocument.RootElement.GetProperty("Name").GetString();
                string bio = jsonDocument.RootElement.GetProperty("Bio").GetString();
                string image = jsonDocument.RootElement.GetProperty("Image").GetString();

                if (DbManager.updateProfile(Server.sessionUsers[authorizationToken].id, username, bio, image)) {
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
        } else {
            string responseString = "Not Logged in";
            Server.SendErrorResponse(writer, responseString, 401);
        }
    }

    public static void Scoreboard(StreamWriter writer, string method) {
        if (method == "GET") {
            string responseString = DbManager.getScoreboard();
            Server.SendResponse(writer, responseString);
            Server.SendResponse(writer, responseString);
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
                string deckConf = DbManager.getUserDeck(Server.sessionUsers[authorizationToken].id);
                if (deckConf == "Deck not configured") {
                    Server.SendResponse(writer, deckConf);
                    return;
                }
                
                Console.WriteLine(Server.sessionUsers[authorizationToken]);
                string battleLog = Server.BattleManager.enterMatchmaking(Server.sessionUsers[authorizationToken]);
                Server.SendResponse(writer, battleLog);
                
            }
        }
        else
        {
            string responseString = "Not logged in";
            Server.SendErrorResponse(writer, responseString, 401);
        }
    }
}