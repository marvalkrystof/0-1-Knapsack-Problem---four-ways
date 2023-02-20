namespace Knapsack
{
/// <summary>
/// Class representing a knapsack from the 0/1 Knapsack problem
/// </summary>
    public class Knapsack
    {
        int capacity;


        public Knapsack(int capacity)
        {
            this.Capacity = capacity;
        }

        public int Capacity
        {

            get => capacity;

            set
            {
                if (capacity < 0)
                {
                    throw new ArgumentOutOfRangeException("Knapsack capacity should be non negative");
                }
                capacity = value;
            }
        }
        
        /// <summary>
        /// Tries to fit provided combination into a knapsack
        /// </summary>
        /// <param name="items">Provided combination</param>
        /// <returns>Value of the combination and the combination itself. Returns (-1, null) if the combination doesn't fit./returns>
        public (int,List<Item>) TryCombination(List<Item> items)
        {
            int totalWeight = 0;
            int totalValue = 0;
            foreach (Item item in items)
            {
                totalValue += item.value;
                totalWeight += item.weight;
                if (totalWeight > Capacity)
                {
                    return (-1, null);
                }
            }
                return (totalValue,items);
        }
        /// <summary>
        /// Finds the best result for the knapsack problem from provided combinations.
        /// </summary>
        /// <param name="combinations">The combinations of values</param>
        /// <returns>The total value of the best combination and the combination itself</returns>
        public (int, List<Item>) FindBestResult(List<List<Item>> combinations) {

            (int, List<Item>) bestResult = (-1, new List<Item>());

            foreach (List<Item> combination in combinations) {
            (int,List<Item>) result = TryCombination(combination);
                if (result.Item1 > bestResult.Item1)
                {
                    bestResult = result;
                }
            }
            return bestResult;
        }


    }
}
