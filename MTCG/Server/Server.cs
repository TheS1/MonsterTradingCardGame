using System.Net;
using System.Net.Sockets;
using System.Text;
using MTCG.DAL;
using MTCG.Models;
using MTCG.Logic;
namespace MTCG.Server;

public static class Server
{
    static readonly int Port = 8080;

    private static TcpListener _listener = null!;
    public static Dictionary<string, User> sessionUsers = new Dictionary<string, User>();
    public static BattleManager BattleManager = new BattleManager();
    public static void Start()
    {

        DbManager.connect();
        _listener = new TcpListener(IPAddress.Loopback, 8080);
        
        _listener.Start();
        Console.WriteLine("Server running at http://localhost:" + Port.ToString() + "/");
        
        //fill card pool out of database
        while (true) {
            var clientSocket = _listener.AcceptTcpClient();
            ThreadPool.QueueUserWorkItem(ProcessRequest, clientSocket);
        }
    }

    public static void Stop()
    {
        _listener.Stop();
    }
    

    private static void ProcessRequest(object? socket) {
        TcpClient? clientSocket = (TcpClient?)socket;
        using var writer = new StreamWriter(clientSocket.GetStream()) { AutoFlush = true };
        using var reader = new StreamReader(clientSocket.GetStream());
        string? line;

        // 1.1 first line in HTTP contains the method, path and HTTP version
        line = reader.ReadLine();
        if ( line != null )
            Console.WriteLine(line);

        string method = line.Split(' ')[0];
        string path = line.Split(' ')[1];
        
        Dictionary<string, string> httpHeaders = new Dictionary<string, string>();
        
        
        // 1.2 read the HTTP-headers (in HTTP after the first line, until the empy line)
        int content_length = 0; // we need the content_length later, to be able to read the HTTP-content
        while ((line = reader.ReadLine()) != null)
        {
            Console.WriteLine(line);
            if (line == "")
            {
                break;  // empty line indicates the end of the HTTP-headers
            }

            // Parse the header
            var parts = line.Split(':');
            httpHeaders.Add(parts[0], parts[1]);
            if (parts.Length == 2 && parts[0] == "Content-Length")
            {
                content_length = int.Parse(parts[1].Trim());
            }
        }
        // 1.3 read the body if existing
        var data = new StringBuilder(200);
        
        if ( content_length>0 )
        {
            char[] chars = new char[1024];
            int bytesReadTotal = 0;
            while ( bytesReadTotal < content_length)
            {
                var bytesRead = reader.Read(chars, 0, chars.Length);
                bytesReadTotal += bytesRead;
                if (bytesRead == 0)
                    break;
                data.Append(chars, 0, bytesRead);
            }
            Console.WriteLine( data.ToString() );
        }

        string contentType = httpHeaders.ContainsKey("Content-Type") ? httpHeaders["Content-Type"] : "";
        string authorizationToken = httpHeaders.ContainsKey("Authorization") ? httpHeaders["Authorization"].Split(' ')[2] : "";
        
        Console.WriteLine($"Received request: {method} {path}");
        
        if (!IsRequestValid(path)) {
            SendErrorResponse(writer, "Malformed request. Please check your request format.", 500);
            return;
        }

        
        if (path == "/") {
            RouteHandler.RootRoute(writer);
        }
        else if (path == "/login") {
            RouteHandler.Login(writer, method, contentType, data.ToString());
        } 
        else if (path == "/register") {
            RouteHandler.Register(writer, method, contentType, data.ToString());
        }
        else if (path == "/logout") {
            RouteHandler.Logout(writer, method);
        }
        else if (path == "/addCards" && IsAdmin(authorizationToken))
        {
            RouteHandler.AddCards(writer, method, data.ToString());
        }
        else if (path == "/buyPack") {
            RouteHandler.BuyPack(writer, method, authorizationToken, data.ToString());
        }
        else if (path == "/cards") {
            RouteHandler.showCards(writer, method, authorizationToken);
        }
        else if (path == "/deck") {
            RouteHandler.deckManagement(writer, method, authorizationToken, data.ToString());
        }
        else if (path == "/myProfile") {
            RouteHandler.userProfile(writer, method, authorizationToken, data.ToString());
        }
        else if (path == "/scoreboard") {
            RouteHandler.Scoreboard(writer, method);
        }
        else if (path == "/battle") {
            RouteHandler.Battle(writer, method, authorizationToken);
        }
        
        else {
            RouteHandler.NotFound(writer);
        }
    }
    
    public static bool IsLoggedIn(string authorizationToken) {
        if (sessionUsers.ContainsKey(authorizationToken)) {
            if (!sessionUsers[authorizationToken].sessionExpired()) {
                return true;
            }
        }
        return false;
    }
    
    private static bool IsAdmin(string authorizationToken) {
        if (IsLoggedIn(authorizationToken)) {
            if (sessionUsers[authorizationToken].isAdmin) {
                return true;
            }
        }
        return false;
    }
    
    //to do: implement more checks later
    private static bool IsRequestValid(string url) {

        // Check for path traversal
        if (url.Contains("..")) {
            return false;
        }

        return true;
    }
    
    public static void SendResponse(StreamWriter writer, string responseString) {
        writer.WriteLine("HTTP/1.0 200 OK");    // first line in HTTP-Response contains the HTTP-Version and the status code
        writer.WriteLine("Content-Type: text/plain; charset=utf-8");     // the HTTP-headers (in HTTP after the first line, until the empy line)
        writer.WriteLine("Content-Length: " + responseString.Length);
        writer.WriteLine();
        writer.Write(responseString);
    }
    
    public static void SendErrorResponse(StreamWriter writer, string responseString, int errCode) {
        writer.WriteLine("HTTP/1.0 " + errCode);    // first line in HTTP-Response contains the HTTP-Version and the status code
        writer.WriteLine("Content-Type: text/plain; charset=utf-8");     // the HTTP-headers (in HTTP after the first line, until the empy line)
        writer.WriteLine("Content-Length: " + responseString.Length);
        writer.WriteLine();
        writer.Write(responseString);

    }

 

    
    
    
}