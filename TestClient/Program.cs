using ClientServerLib; 
namespace TestClient
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Client.StartClient();
            Console.ReadLine(); 
            return 0; 
        }
    }
    public class Client : ASyncClient
    {

    }
}