using System.Net;

namespace MTCG;

public static class RouteHandler {
    private static Dictionary<string, User> sessions = new Dictionary<string, User>();
    // root route
    public static void RootRoute(StreamWriter writer) {
        string responseString = "root";
        Server.SendResponse(writer, responseString);
    }
    
    // login route
    public static void Login(StreamWriter writer, string method, string data) {
        /*
        if (method == "POST") {
            using (var reader = new StreamReader(.InputStream)) {
                    var requestData = reader.ReadToEnd();
                    var formData = requestData.Split(' ');

                    if (formData.Length == 2) {
                        string username = Uri.UnescapeDataString(formData[0]);
                        string password = Uri.UnescapeDataString(formData[1]);

                        if (username == "admin" && password == "admin") {
                            // will be outsorced into load user once database
                            string responseString = "Login successful!";

                            Server.SendResponse(writer, responseString);
                        }
                        else {
                            string responseString = "Login failed. Invalid username or password.";
                            Server.SendResponse(writer, responseString);
                        }
                    }
                    else {
                        // Invalid form data
                        string responseString = "Invalid Data. Use curl -d \"username password\"";
                        Server.SendResponse(writer, responseString);
                    }
                
            }
            else {
                string responseString = "Didnt send cookie file -> curl ... -c cookies.txt";
                Server.SendResponse(writer, responseString);
            }
        } else {
            string responseString = "Wrong request type expecting post";
            Server.SendErrorResponse(writer, responseString);
        }
        */
    }


    public static void Register(StreamWriter writer, string method, string data) {
        /*if (request.HttpMethod == "POST") {
            if (SentCookieFile(request)) {
                using (var reader = new StreamReader(request.InputStream)) {
                    var requestData = reader.ReadToEnd();
                    var formData = requestData.Split(' ');

                    if (formData.Length == 2) {
                        string username = Uri.UnescapeDataString(formData[0]);
                        string password = Uri.UnescapeDataString(formData[1]);
                        //check if username exists, if password is good bla bla bla

                        string responseString = "user " + username + " created";

                        string sessionToken = Guid.NewGuid().ToString();
                        sessions[sessionToken] = LoadUser(username);
                        SetSessionCookie(writer, sessionToken);

                        Server.SendResponse(writer, responseString);
                    }
                    else {
                        // Invalid form data
                        string responseString = "Invalid Data. Use curl -d \"username password\"";
                        Server.SendResponse(writer, responseString);
                    }
                }
            }
            else {
                string responseString = "Didnt send cookie file -> curl ... -c cookies.txt";
                Server.SendResponse(writer, responseString);
            }
        }
        else {
            string responseString = "Wrong request type expecting post";
            Server.SendErrorResponse(writer, responseString);
        }
        */
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
        return new User(username);
    }
}