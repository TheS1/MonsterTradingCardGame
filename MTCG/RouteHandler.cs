using System.Net;
using System.Text.Json;
using Npgsql;
namespace MTCG;

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
                    Server.sessionUsers.Add(user.getUsername() + "_Key",user);
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
                User? user = JsonSerializer.Deserialize<User>(data);
                if (DbManager.registerUser(user)) {
                    Server.sessionUsers.Add(user.getUsername() + "_Key",user);
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
    
    public static void BuyPack(StreamWriter writer, string method, string authorizationToken) {
        Console.WriteLine("Token:" +authorizationToken);
        if (method == "GET") {
            if (IsLoggedIn(authorizationToken)) {
                string responseString = "Bought Pack";
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
        string responseString = "404 Not Found";
        Server.SendResponse(writer, responseString);
    }


    public static bool SentCookieFile(HttpListenerRequest request) { 
        return true;
    }
    
    private static bool IsLoggedIn(string authorizationToken) {
        if (Server.sessionUsers.ContainsKey(authorizationToken)) {
            if (!Server.sessionUsers[authorizationToken].sessionExpired()) {
                return true;
            }
        }

        return false;
    }
    
    
    
    private static void SetSessionCookie(HttpListenerResponse writer, string sessionToken) {
        var cookie = new Cookie("Session-Token", sessionToken);
        writer.AppendCookie(cookie);
    }
    
    private static User LoadUser(string username) {
        //load user data out of database based on username, database doesnt exist yet
        return new User();
    }
}