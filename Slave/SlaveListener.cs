using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text.Json;
using Knapsack;

namespace Slave
{
    public class SlaveListener
    {
        TcpClient client;

        public void ListenForConnection()
        {   
            var ipEndPoint = new IPEndPoint(IPAddress.Any, 65525);
            TcpListener listener = new(ipEndPoint);

            try
            {
                listener.Start();

                Logger.Logger.Log("Listening on " + listener.LocalEndpoint);
                Logger.Logger.Log("Waiting for a connection");
                while (!listener.Pending()) ;
                client = listener.AcceptTcpClient();
                Logger.Logger.Log(client.Client.RemoteEndPoint + " has connected");
                
            }
            catch(Exception ex)
            {
                Logger.Logger.Log(ex.Message);
                Logger.Logger.LogCriticalFailure("Listening for connections failed");
                
            }
            finally
            {
                
                SlaveWorker worker = new SlaveWorker();
                var bestItems = worker.Solve(ReceiveItems());
                SendResult(bestItems);
                listener.Stop();
            }

        }

        public List<Item> ReceiveItems()
        {
            while (client.Connected)
            {
                using (NetworkStream ns = client.GetStream())
                {
                    using (StreamReader sr = new StreamReader(ns))
                    {
                        
                        string json = sr.ReadLine();
                        return JsonSerializer.Deserialize<List<Item>>(json);
                        
                    }
                }
            }
            return new List<Item>();
        }

        public void SendResult(List<Item> result)
        {
            if(client.Connected)
            {
                using (NetworkStream ns = client.GetStream())
                {
                    using (StreamWriter sw = new StreamWriter(ns))
                    {
                        sw.WriteLine(JsonSerializer.Serialize(result));
                    }
                }
            } else
            {
                Logger.Logger.LogCriticalFailure("Client is disconnected, can't send results");
            }
            
        }

    }
}
