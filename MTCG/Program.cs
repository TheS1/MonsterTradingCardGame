// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;
using System.Text;

internal class Program
{
    public static void Main(string[] args)
    {
        var httpServer = new TcpListener(IPAddress.Loopback, 1001);
        httpServer.Start();

        Console.WriteLine("running");
       
        /*
        while (true)
        {
            using (var client = httpServer.AcceptTcpClient())
            using (var stream = client.GetStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            {
                string request = reader.ReadLine();
                Console.WriteLine("Received request: " + request);

                string response = "HTTP/1.1 200 OK\r\nContent-Type: text/html\r\n\r\nHello, World!";
                byte[] responseData = Encoding.UTF8.GetBytes(response);
                stream.Write(responseData, 0, responseData.Length);
            }
        }
        */
        
        while (true)
        {
            var clientSocket = httpServer.AcceptTcpClient();
            using var writer = new StreamWriter(clientSocket.GetStream()) {AutoFlush = true};
            using var reader = new StreamReader(clientSocket.GetStream());
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
            
                Console.WriteLine(line);
                if (line == "")
                {
                    break;
                }
            }
            writer.WriteLine("Nice");
        }
        
        
    }
}