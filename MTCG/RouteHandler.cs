using System.Net;

namespace MTCG;

public static class RouteHandler {
    private static Dictionary<string, User> sessions = new Dictionary<string, User>();
    // root route
    public static void RootRoute(HttpListenerRequest request, HttpListenerResponse response) {
        string responseString = "root";
        Server.SendResponse(response, responseString);
    }
    
    // login route
    public static void Login(HttpListenerRequest request, HttpListenerResponse response) {
        if (request.HttpMethod == "POST") {
            // Handle the login POST request here
            // You can access request.InputStream to read POST data
            using (var reader = new StreamReader(request.InputStream)) {
                var requestData = reader.ReadToEnd();
                var formData = requestData.Split(' ');

                if (formData.Length == 2) {
                    string username = Uri.UnescapeDataString(formData[0]);
                    string password = Uri.UnescapeDataString(formData[1]);
                    
                    if (username == "admin" && password == "admin") { // will be outsorced into load user once database
                        string responseString = "Login successful!";
                        
                        string sessionToken = Guid.NewGuid().ToString();
                        sessions[sessionToken] = LoadUser(username);
                        SetSessionCookie(response, sessionToken);
                        
                        Server.SendResponse(response, responseString);
                    }
                    else {
                        string responseString = "Login failed. Invalid username or password.";
                        Server.SendResponse(response, responseString);
                    }
                }
                else {
                    // Invalid form data
                    string responseString = "Invalid Data. Use curl -d \"username password\"";
                    Server.SendResponse(response, responseString);
                }
            }
        } else {
            string responseString = "Wrong request type";
            Server.SendErrorResponse(response, responseString);
        }

        
    }
    // not found route
    

    public static void Register(HttpListenerRequest request, HttpListenerResponse response) {
        if (request.HttpMethod == "POST") {
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
                    SetSessionCookie(response, sessionToken);
                    
                    Server.SendResponse(response, responseString);
                    
                    
                    
                }
                else {
                    // Invalid form data
                    string responseString = "Invalid Data. Use curl -d \"username password\"";
                    Server.SendResponse(response, responseString);
                }
            }
        } else {
            string responseString = "Wrong request type";
            Server.SendErrorResponse(response, responseString);
        }

    }
    
    public static void BuyPack(HttpListenerRequest request, HttpListenerResponse response) {
        if (request.HttpMethod == "GET") {
            if (IsLoggedIn(request)) {
                string userToken = request.Cookies["Session-Token"]?.Value;
                sessions[userToken].buyPack(response);
                //to do: update db when it exists
            }
            else {
                string responseString = "Log in first";
                

                for(int x=0; x< sessions.Count; x++) {
                    Console.WriteLine("{0} and {1}", sessions.Keys.ElementAt(x), 
                        sessions[ sessions.Keys.ElementAt(x)]);
                }

                Server.SendResponse(response, responseString);
            }
        } else {
            string responseString = "Wrong request type";
            Server.SendErrorResponse(response, responseString);
        }
    }
    
    public static void NotFound(HttpListenerResponse response) {
        // Handle 404 Not Found
        string responseString = "404 Not Found";
        response.StatusCode = (int)HttpStatusCode.NotFound;
        Server.SendResponse(response, responseString);
    }
    
    
    private static bool IsLoggedIn(HttpListenerRequest request) {
        // Extract the session token from the request (from cookies, headers, or other mechanisms)
        string sessionToken = request.Cookies["Session-Token"]?.Value;
        // Check if the session token is valid and associated with an authenticated user
        return !string.IsNullOrEmpty(sessionToken) && sessions.ContainsKey(sessionToken);
    }
    
    
    private static void SetSessionCookie(HttpListenerResponse response, string sessionToken) {
        var cookie = new Cookie("Session-Token", sessionToken);
        response.AppendCookie(cookie);
    }
    
    private static User LoadUser(string username) {
        //load user data out of database based on username, database doesnt exist yet
        return new User(username);
    }
}