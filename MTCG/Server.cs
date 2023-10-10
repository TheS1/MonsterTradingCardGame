using System.Net;

namespace MTCG;

public static class Server
{
    static readonly int Port = 8080;

    private static HttpListener _listener = null!;

    public static void Start()
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add("http://localhost:" + Port.ToString() + "/");
        _listener.Start();
        Console.WriteLine("Server running at http://localhost:" + Port.ToString() + "/");
        
        //fill card pool out of database
        CardPool.addCardsToPool(new List<Card>{new MonsterCard("blob", 10, "grass"), new MonsterCard("blub", 10, "water")});
        while (_listener.IsListening) {
            var context = _listener.GetContext();
            ThreadPool.QueueUserWorkItem(ProcessRequest, context);
        }
    }

    public static void Stop()
    {
        _listener.Stop();
    }
    

    private static void ProcessRequest(object state)
    {
        var context = (HttpListenerContext)state;
        var request = context.Request;
        var response = context.Response;

        Console.WriteLine($"Received request: {request.HttpMethod} {request.Url}");
        
        if (!IsRequestValid(request)) {
            SendErrorResponse(response, "Malformed request. Please check your request format.");
            return;
        }
        
        string route = request.Url.AbsolutePath;

        if (route == "/") {
            RouteHandler.RootRoute(request, response);
        }
        else if (route == "/login") {
            RouteHandler.Login(request, response);

        } 
        else if (route == "/register") {
            RouteHandler.Register(request, response);
        }
        else if (route == "/buyPack") {
            RouteHandler.BuyPack(request, response);
        }
        else {
            RouteHandler.NotFound(response);
        }
        
    }
    

    
    //to do: implement more checks later
    private static bool IsRequestValid(HttpListenerRequest request) {
        string url = request.Url.ToString();

        // Check for scheme (protocol)
        if (!url.StartsWith("http://") && !url.StartsWith("https://")) {
            return false;
        }
        
        // Check for path traversal
        if (url.Contains("..")) {
            return false;
        }

        return true;
    }
    
    public static void SendResponse(HttpListenerResponse response, string responseString) {
        response.ContentType = "text/plain";
        byte[] responseBytes = System.Text.Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = responseBytes.Length;
        response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
        response.Close();
    }
    
    public static void SendErrorResponse(HttpListenerResponse response, string message) {
        response.StatusCode = (int)HttpStatusCode.BadRequest;
        response.ContentType = "text/plain";
        byte[] errorBytes = System.Text.Encoding.UTF8.GetBytes(message);
        response.ContentLength64 = errorBytes.Length;
        response.OutputStream.Write(errorBytes, 0, errorBytes.Length);
        response.Close();
    }

 

    
    
    
}