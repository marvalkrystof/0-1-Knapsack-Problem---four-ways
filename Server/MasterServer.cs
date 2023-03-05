using Knapsack;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Master
{
    public class MasterServer
    {

        public List<IPEndPoint> slaveAddresses = new List<IPEndPoint>();
        public List<TcpClient> slaveConnections = new List<TcpClient>();

        int knapsackCapacity;
        int numberOfItems;
        int minItemValue;
        int maxItemValue;
        int maxItemWeight;
        int minItemWeight;

        Knapsack.Knapsack knapsack;


        public void LoadConfig()
        {
            string[] slaveIps = ConfigurationManager.AppSettings.AllKeys
                             .Where(key => key.StartsWith("ip"))
                             .Select(key => ConfigurationManager.AppSettings[key])
                             .ToArray();

            foreach (var ip in slaveIps)
            {
                IPEndPoint result = null;
                if (!IPEndPoint.TryParse(ip, out result)) 
                {
                    Logger.Logger.Log(ip + " couldn't be parsed and wont be used");
                }
                else
                {
                    slaveAddresses.Add(result);
                }                

            }

            string? knapsackCapacityString = System.Configuration.ConfigurationManager.AppSettings["knapsackCapacity"];
            string? numberOfItemsString = System.Configuration.ConfigurationManager.AppSettings["numberOfItems"];
            string? itemMinValueString = System.Configuration.ConfigurationManager.AppSettings["itemMinValue"];
            string? itemMaxValueString = System.Configuration.ConfigurationManager.AppSettings["itemMaxValue"];
            string? itemMinWeightString = System.Configuration.ConfigurationManager.AppSettings["itemMinWeight"];
            string? itemMaxWeightString = System.Configuration.ConfigurationManager.AppSettings["itemMaxWeight"];


            if (knapsackCapacityString == null)
            {
                Logger.Logger.LogCriticalFailure("Couldn't fetch knapsack capacity from App.config");
            }
            if (numberOfItemsString == null)
            {
                Logger.Logger.LogCriticalFailure("Couldn't fetch number of items from App.config");
            }
            if (itemMinValueString == null)
            {
                Logger.Logger.LogCriticalFailure("Couldn't fetch minimum value of items from App.config");
            }
            if (itemMaxValueString == null)
            {
                Logger.Logger.LogCriticalFailure("Couldn't fetch maximum value of items from App.config");
            }
            if (itemMinWeightString == null)
            {
                Logger.Logger.LogCriticalFailure("Couldn't fetch minimum weight of items from App.config");
            }
            if (itemMaxWeightString == null)
            {
                Logger.Logger.LogCriticalFailure("Couldn't fetch maximum weight of items from App.config");
            }

            if (!int.TryParse(knapsackCapacityString, out this.knapsackCapacity))
            {
                Logger.Logger.LogCriticalFailure("Knapsack capacity isn't an integer");
            }
            if (!int.TryParse(itemMaxWeightString, out this.maxItemWeight))
            {
                Logger.Logger.LogCriticalFailure("Max weight isn't an integer");
            }
            if (!int.TryParse(itemMinWeightString, out this.minItemWeight))
            {
                Logger.Logger.LogCriticalFailure("Min weight isn't an integer");
            }
            if (!int.TryParse(itemMaxValueString, out this.maxItemValue))
            {
                Logger.Logger.LogCriticalFailure("Max value isn't an integer");
            }
            if (!int.TryParse(itemMinValueString, out this.minItemValue))
            {
                Logger.Logger.LogCriticalFailure("Min value isn't an integer");
            }
            if (!int.TryParse(numberOfItemsString, out this.numberOfItems))
            {
                Logger.Logger.LogCriticalFailure("Number of items isn't an integer");
            }

            knapsack = new Knapsack.Knapsack(knapsackCapacity);
        }
    

        public void ConnectToSlaves()
        {

            this.LoadConfig();

            TcpClient client = new TcpClient();
            
            foreach (var ip in this.slaveAddresses.ToList())
            {
                try
                {
                    client.Connect(ip);
                    slaveConnections.Add(client);
                }
                catch (SocketException)
                {
                    Logger.Logger.Log("Couldn't connect to " + ip + " dropping this address");
                    slaveAddresses.Remove(ip);
                }
            }

            DistributeWork();
            var results = ReceiveResults();
            var solution = knapsack.Solve(results);
            Logger.Logger.Log("Total value of items: " + solution.Item1 + "\nBest combination items:\n" + string.Join("\n", solution.Item2));
            //Logger.Logger.Log("Miliseconds elapsed: " + stopWatch.ElapsedMilliseconds.ToString());
        }

        public void DistributeWork()
        {
            if(slaveConnections.Count == 0)
            {
                Logger.Logger.LogCriticalFailure("There are no active connections, can't perform the computation");
            }

            List<Item> items = Item.GenerateItems(numberOfItems, minItemWeight, maxItemWeight, minItemValue, maxItemValue);

            for (int i = 1; i <= slaveConnections.Count; i++)
            {
                int y = i-1;
                Thread thread = new Thread(()=> SendWork(y,items));
                thread.Start();
            }
        }

        public void SendWork(int index, List<Item> items)
        {
            
            using (NetworkStream ns = slaveConnections[index].GetStream())
            {
                using (StreamWriter sw = new StreamWriter(ns))
                { 
                    sw.WriteLine(JsonSerializer.Serialize(items.GetRange((index * items.Count) / slaveConnections.Count, items.Count / slaveConnections.Count)));
                }
                
            }

        }


        public List<Item> ReceiveResults()
        {
            List<Item> results = new List<Item>();
            while (slaveConnections.Count > 0)
            {
                foreach (var connection in slaveConnections.ToList())
                {
                    try {
                        if (connection.Available > 0)
                        {
                            using (NetworkStream ns = connection.GetStream())
                            {
                                using (StreamReader sr = new StreamReader(ns))
                                {

                                    string json = sr.ReadLine();

                                    results.AddRange(JsonSerializer.Deserialize<List<Item>>(json));
                                    slaveConnections.Remove(connection);
                                }
                            }
                        }
                    } catch (ObjectDisposedException)
                    {
                        slaveConnections.Remove(connection);
                    }
               }
            }
            return results;
        }

        

    }
}
