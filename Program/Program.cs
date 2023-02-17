using Knapsack;
using Combinations;
using System.Diagnostics;

namespace Program {
    class Program
    {

        static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            Knapsack.Knapsack knapsack = new Knapsack.Knapsack(50);
            List<Item> items = Item.GenerateItems(10,1,10,1,100);
            List<List<Item>> combinations = Combinations.Combination<Item>.GetCombinations(items);
            
            var result = knapsack.FindBestResult(combinations);
            Console.WriteLine("Total value: " + result.Item1 + " " + string.Join(",",result.Item2));
            stopWatch.Stop();
            Console.WriteLine("Miliseconds elapsed:" + stopWatch.ElapsedMilliseconds.ToString());
        }

    }
}