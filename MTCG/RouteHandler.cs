using System.Net;
using System.Text.Json;
using Npgsql;
namespace MTCG;

public static class RouteHandler
{
    private static Dictionary<string, User> sessions = new Dictionary<string, User>();

    // root route
    public static void RootRoute(StreamWriter writer)
    {
        string responseString = "root";
        Server.SendResponse(writer, responseString);
    }

    // login route
    public static void Login(StreamWriter writer, string method, string contentType, string data)
    {
        Console.WriteLine(contentType);
        if (method == "POST") {
            if (contentType == " application/json") {
                User? user = JsonSerializer.Deserialize<User>(data);
                if (DbManager.loginUser(user.getUsername(), user.getPassword())) {
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
    }


    public static void Register(StreamWriter writer, string method, string contentType, string data)
    {
        
        if (method == "POST")
        {
            if (contentType == " application/json")
            {
                User? user = JsonSerializer.Deserialize<User>(data);
                if (DbManager.registerUser(user.getUsername(), user.getPassword())) {
                    Server.sessionUsers.Add(user.getUsername() + "_Key",user);
                    string responseString = "Registered successfully!";
                    Server.SendResponse(writer, responseString);
                }
                else {
                    
                    string responseString = "Username already exists!";
                    Server.SendResponse(writer, responseString);
                }
            }

        }

        else
        {
            string responseString = "Didnt send cookie file -> curl ... -c cookies.txt";
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
    
    public static void BuyPack(StreamWriter writer, string method) {
        /*
        if (request.HttpMethod == "GET") {
            if (SentCookieFile(request)) {
                if (IsLoggedIn(request)) {
                    string userToken = request.Cookies["Session-Token"]?.Value;
                    sessions[userToken].buyPack(writer);
                    //to do: update db when it exists
                }
                else {
                    string responseString = "Log in first!";
                    Server.SendResponse(writer, responseString);
                }
            }
            else {
                string responseString = "Didnt send cookie file -> curl ... -b cookies.txt";
                Server.SendResponse(writer, responseString);
            }
            
        } else {
            string responseString = "Wrong request type";
            Server.SendErrorResponse(writer, responseString);
        }
        */
    }

    
    
    public static void NotFound(StreamWriter writer) {
        // Handle 404 Not Found
        string responseString = "404 Not Found";
        Server.SendResponse(writer, responseString);
    }


    public static bool SentCookieFile(HttpListenerRequest request) { 
        return true;
    }
    
    private static bool IsLoggedIn(HttpListenerRequest request) {
        // Extract the session token from the request (from cookies, headers, or other mechanisms)
        string sessionToken = request.Cookies["Session-Token"]?.Value;
        // Check if the session token is valid and associated with an authenticated user
        return !string.IsNullOrEmpty(sessionToken) && sessions.ContainsKey(sessionToken);
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