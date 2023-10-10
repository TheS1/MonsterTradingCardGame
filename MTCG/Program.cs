// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;
using System.Text;
using MTCG;

internal class Program
{
    public static void Main(string[] args)
    {
        
        Server.Start();
        
        while (true){}
        Server.Stop();
        
        
        
    }
}