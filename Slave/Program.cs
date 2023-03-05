using Slave;

namespace Master
{
    public class Program
    {
        static void Main(string[] args)
        {
            SlaveListener slave = new SlaveListener();
            slave.ListenForConnection();

        }
    }
}