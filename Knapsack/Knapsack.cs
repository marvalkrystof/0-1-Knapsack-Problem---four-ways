using System.Runtime.CompilerServices;

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
        /// Finds the best item set for this knapsack using dynamic programming approach
        /// </summary>
        /// <param name="items">Items provided for the calculation</param>
        /// <returns>Total value of best items and list of the items</returns>
        public (int, List<Item>) Solve(List<Item> items)

        {
            int n = items.Count;
            int[,] K = new int[n + 1, Capacity + 1];

            for (int i = 0; i <= n; i++)
            {
                for (int w = 0; w <= Capacity; w++)
                {
                    if (i == 0 || w == 0)
                        K[i, w] = 0;
                    else if (items[i - 1].weight <= w)
                        K[i, w] = max(items[i - 1].value + K[i - 1, w - items[i - 1].weight], K[i - 1, w]);
                    else
                        K[i, w] = K[i - 1, w];
                }
            }

            List<Item> selectedItems = new List<Item>();
            int remainingWeight = Capacity;
            int totalValue = 0;
            for (int i = n; i > 0 && K[i, remainingWeight] > 0; i--)
            {
                if (K[i, remainingWeight] != K[i - 1, remainingWeight])
                {
                    Item selectedItem = items[i - 1];
                    selectedItems.Add(selectedItem);
                    remainingWeight -= selectedItem.weight;
                    totalValue += selectedItem.value;
                }
            }

            return (totalValue,selectedItems);
            }

        /// <summary>
        /// Finds the bigger number between two
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>the bigger number</returns>
        static int max(int a, int b) { return (a > b) ? a : b; }

    }
}
