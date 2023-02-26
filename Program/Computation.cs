
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
            var layers = Computation.GetLayersForThreads(Environment.ProcessorCount, items.Count);
            Object lockObject = new Object();

            (int, IEnumerable<Item>) best = new(-1, new List<Item>());

            List<Thread> threads = new List<Thread>();


            stopWatch.Start();

            int loops;
            if (Environment.ProcessorCount > layers.Count)
            {
                loops = layers.Count;
            }
            else
            {
                loops = Environment.ProcessorCount;
            }
            for (int i = 0; i < loops; i++)
            {
                int y = i;
                Thread thread = new Thread(() =>
                {
                    foreach (var layer in layers[y])
                    {
                        var result = FindBestOnLayer(layer, knapsack, items);

                        lock (lockObject) {
                            if (result.Item1 > best.Item1)
                            {
                                best = result;
                            }
                        }
                    }

                });
                thread.Start();
                threads.Add(thread);
            }
            foreach(Thread thread in threads)
            {
                thread.Join();
            }
            stopWatch.Stop();
            Logger.Logger.Log("Total value of items: " + best.Item1 + "\nBest combination items:\n" + string.Join("\n", best.Item2));
            Logger.Logger.Log("Miliseconds elapsed: " + stopWatch.ElapsedMilliseconds.ToString());
        }

        

    
     /// <summary>
     /// Finds best result by generating all possible combinations
     /// </summary>
     /// <param name="knapsack">knapsack to fit items into</param>
     /// <param name="items">items</param>
     /// <returns>best combination</returns>
    public (int, IEnumerable<Item>) FindBestByGeneratingAll(Knapsack.Knapsack knapsack, List<Item> items)
        {
            var combinations = Item.GenerateAllCombinations(items);
            var result = knapsack.FindBestResult(combinations);
            return result;
        }


        /// <summary>
        /// Finds best combination on layer of pascals triangle
        /// </summary>
        /// <param name="layer">layer of the triangle</param>
        /// <param name="knapsack">knapsack to fit items into</param>
        /// <param name="items">items</param>
        /// <returns>best combination on the given layer</returns>
        public (int, IEnumerable<Item>) FindBestOnLayer(int layer, Knapsack.Knapsack knapsack, List<Item> items)
        {
            if (layer > items.Count)
            {
                Logger.Logger.LogCriticalFailure("Layer has to be <= items count ");
            }

            var iterTools = new Itertools();
            var combinations = iterTools.Combinations(items, layer);
            var result = knapsack.FindBestResult(combinations);
            return result;
        }



/// <summary>
/// Calcualtes number of total combinations for each layer of pascals triangle 
/// </summary>
/// <param name="itemsCount">number of items</param>
/// <returns>list of number of combinations for each layer</returns>
        public static List<long> CalculateNumberOfCombinationsAllLayers(int itemsCount)
        {
            List<long> layerCombinationsNumber = new List<long>();
            for (int i = 1; i <= itemsCount; i++)
            {
                layerCombinationsNumber.Add(GetBinCoeff(itemsCount, i));
            }
            return layerCombinationsNumber;
        }


        /// <summary>
        /// Divides layers of pascal triangle between designated number of threads
        /// </summary>
        /// <param name="numberOfThreads">Number of threads</param>
        /// <param name="itemsCount">Item count</param>
        /// <returns>List of lists which contain the layers to be calculated for each thread</returns>
        public static List<List<int>> GetLayersForThreads(int numberOfThreads, int itemsCount)
        {
            List<long> numberOfCombinationsPerLayer = CalculateNumberOfCombinationsAllLayers(itemsCount);
            double combinationsPerThread = numberOfCombinationsPerLayer.Sum() / (double)numberOfThreads;
            List<List<int>> layersForThreads = new List<List<int>>();

            long sumOfCombinations = 0;
            List<int> combinationLayers = new List<int>();
            for (int i = 1; i < numberOfCombinationsPerLayer.Count; i++)
            {
                sumOfCombinations += numberOfCombinationsPerLayer[i];
                combinationLayers.Add(i);
                if (sumOfCombinations > combinationsPerThread)
                {
                    sumOfCombinations = 0;
                    layersForThreads.Add(combinationLayers);
                    combinationLayers = new List<int>();
                }
                else if (i == numberOfCombinationsPerLayer.Count - 1)
                {
                    layersForThreads.Add(combinationLayers);
                }
            }
            if (layersForThreads.Count < numberOfThreads)
            {
                while (SplitLayers(layersForThreads, numberOfThreads));
            }
            return layersForThreads;

        }

        /// <summary>
        /// splits layers if the number of layers is less than number of threads
        /// </summary>
        /// <param name="layersForThreads">the list of layers to be calculated for every thread</param>
        /// <param name="numberOfThreads">number of threads used</param>
        /// <returns>true if split can happen</returns>
        private static bool SplitLayers(List<List<int>> layersForThreads,int numberOfThreads)
        {
            int mid = layersForThreads.Count / 2;
            bool splittable = false;

            for (int i = mid; i < layersForThreads.Count; i++)
            {
                if (layersForThreads[i].Count > 1)
                {
                    var lists = Computation.SplitList(layersForThreads[i], layersForThreads[i].Count / 2);
                    layersForThreads[i] = lists[0];
                    layersForThreads.Add(lists[1]);
                    if (lists[0].Count > 1 || lists[1].Count > 1)
                    {
                        splittable= true;
                    }
                }
                if (layersForThreads.Count == numberOfThreads)
                {
                    return false;
                }
            }
            for (int i = mid - 1; i >= 0; i--)
            {
                if (layersForThreads[i].Count > 1)
                {
                    var lists = Computation.SplitList(layersForThreads[i], layersForThreads[i].Count / 2);
                    layersForThreads[i] = lists[0];
                    layersForThreads.Add(lists[1]);
                    if (lists[0].Count > 1 || lists[1].Count > 1)
                    {
                        splittable = true;
                    }
                }
                if (layersForThreads.Count == numberOfThreads)
                {
                    return false;
                }
            }
            return splittable;

        }

        /// <summary>
        /// Calculates binomial coefficient
        /// </summary>
        /// <param name="N">Number of items</param>
        /// <param name="K">Group size</param>
        /// <returns>Binomial coefficient</returns>
        private static long GetBinCoeff(long N, long K)
        {
            // This function gets the total number of unique combinations based upon N and K.
            // N is the total number of items.
            // K is the size of the group.
            // Total number of unique combinations = N! / ( K! (N - K)! ).
            // This function is less efficient, but is more likely to not overflow when N and K are large.
            // Taken from:  http://blog.plover.com/math/choose.html
            //
            long r = 1;
            long d;
            if (K > N) return 0;
            for (d = 1; d <= K; d++)
            {
                r *= N--;
                r /= d;
            }
            return r;
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