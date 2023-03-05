namespace Master
{
    public class Program
    {
        static void Main(string[] args)
        {
            MasterServer server = new MasterServer();
   
            server.ConnectToSlaves();
        }
    }
}