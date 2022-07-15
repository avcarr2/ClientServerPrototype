using ClientServerLib;
using System; 
namespace ClientServer
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Server.BeginListening();
            Console.ReadLine(); 
            return 1; 
        }
    }
    public class Server : AsyncSocketListener
    {

    }
}