
using CSharpItertools;
using Knapsack;
using System.Configuration;
using System.Diagnostics;

namespace Program
{
    /// <summary>
    /// This class is representing the object, that is needed to start the computation
    /// </summary>
    public class Computation
    {
        int knapsackCapacity;
        int numberOfItems;
        int minItemValue;
        int maxItemValue;
        int maxItemWeight;
        int minItemWeight;





        /// <summary>
        /// Starts the computation and prints the best combination and milliseconds elapsed into used logging method.
        /// </summary>
        public void Start()
        {
            LoadConfig();
            Stopwatch stopWatch = new Stopwatch();
            Knapsack.Knapsack knapsack = new Knapsack.Knapsack(this.knapsackCapacity); 
            List<Item> items = Item.GenerateItems(numberOfItems, minItemWeight, maxItemWeight, minItemValue, maxItemValue);

            stopWatch.Start();
            var best = knapsack.Solve(items);
            stopWatch.Stop();
            Logger.Logger.Log("Total value of items: " + best.Item1 + "\nBest combination items:\n" + string.Join("\n", best.Item2));
            Logger.Logger.Log("Miliseconds elapsed: " + stopWatch.ElapsedMilliseconds.ToString());
        }

   

        /// <summary>
        /// Loads the configuration from App.config into fields of this object
        /// </summary>
        private void LoadConfig() {
            string? knapsackCapacityString = System.Configuration.ConfigurationManager.AppSettings["knapsackCapacity"];
            string? numberOfItemsString = System.Configuration.ConfigurationManager.AppSettings["numberOfItems"];
            string? itemMinValueString = System.Configuration.ConfigurationManager.AppSettings["itemMinValue"];
            string? itemMaxValueString = System.Configuration.ConfigurationManager.AppSettings["itemMaxValue"];
            string? itemMinWeightString = System.Configuration.ConfigurationManager.AppSettings["itemMinWeight"];
            string? itemMaxWeightString = System.Configuration.ConfigurationManager.AppSettings["itemMaxWeight"];


            if(knapsackCapacityString == null)
            {
                Logger.Logger.LogCriticalFailure("Couldn't fetch knapsack capacity from App.config");
            }
            if(numberOfItemsString == null)
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

        }

        /// <summary>
        /// Splits list into separate lists
        /// </summary>
        /// <typeparam name="T">Type of the list to be split</typeparam>
        /// <param name="me">The list to be split</param>
        /// <param name="size">size of each list</param>
        /// <returns>List of the lists that were split</returns>
        public static List<List<T>> SplitList<T>(List<T> me, int size)
        {
            var list = new List<List<T>>();
            for (int i = 0; i < me.Count; i += size)
                list.Add(me.GetRange(i, Math.Min(size, me.Count - i)));
            return list;
        }



    }
}