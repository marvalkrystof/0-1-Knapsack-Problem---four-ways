using Knapsack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slave
{
    public class SlaveWorker
    {

        int knapsackCapacity;

        /// <summary>
        /// Loads the configuration from App.config into fields of this object
        /// </summary>
        public void LoadConfig()
        {
            string? knapsackCapacityString = System.Configuration.ConfigurationManager.AppSettings["knapsackCapacity"];
            
            if (knapsackCapacityString == null)
            {
                Logger.Logger.LogCriticalFailure("Couldn't fetch knapsack capacity from App.config");
            }
            if (!int.TryParse(knapsackCapacityString, out this.knapsackCapacity))
            {
                Logger.Logger.LogCriticalFailure("Knapsack capacity isn't an integer");
            }
        }



        public List<Item> Solve(List<Item> items)
        {
            LoadConfig();
            Stopwatch stopWatch = new Stopwatch();
            Knapsack.Knapsack knapsack = new Knapsack.Knapsack(this.knapsackCapacity);

            Object lockObj = new Object();


            List<Thread> threads = new List<Thread>();

            List<Item> candidates = new List<Item>();

            stopWatch.Start();

            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                int y = i;
                Thread thread = new Thread(() =>
                {
                    var bestOfThread = knapsack.Solve(items.GetRange(y * (items.Count / Environment.ProcessorCount), items.Count / Environment.ProcessorCount));
                    lock (lockObj) {
                        candidates.AddRange(bestOfThread.Item2);
                    }
                });
                thread.Start();
                threads.Add(thread);
            }
            foreach (Thread thread in threads)
            {
                thread.Join();
            }


            var best = knapsack.Solve(candidates);

            stopWatch.Stop();
            Logger.Logger.Log("Total value of items: " + best.Item1 + "\nBest combination items:\n" + string.Join("\n", best.Item2));
            Logger.Logger.Log("Miliseconds elapsed: " + stopWatch.ElapsedMilliseconds.ToString());
            return best.Item2;
        }

    }
}
