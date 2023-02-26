using CSharpItertools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knapsack
{
    /// <summary>
    /// This class is representing the item from the 0/1 Knapsack problem.
    /// </summary>
    public class Item
    {
        public int weight;
        public int value;

        public Item(int weight, int value)
        {
            this.weight = weight;
            this.value = value;
        }

        /// <summary>
        /// Generates an item according to parameters.
        /// </summary>
        /// <param name="minWeight">Minimum weight of the item</param>
        /// <param name="maxWeight">Maximum weight of the item</param>
        /// <param name="minValue">Minimum value of the item</param>
        /// <param name="maxValue">Maximum value of the item</param>
        /// <returns>The generated item</returns>
        /// <exception cref="ArgumentOutOfRangeException">If one of the minimum parameters is lower than the maximum 
        /// or is equal to zero exception is raised.
        /// </exception>
        public static Item Generate(int minWeight, int maxWeight, int minValue, int maxValue)
        {
            if(minWeight <= 0)
            {
                throw new ArgumentOutOfRangeException("The minimal weight of item should be greater than 0");
            }
            if(minWeight > maxWeight)
            {
                throw new ArgumentOutOfRangeException("The minimal weight of item should be less than maximum weight");
            }
            if (minValue <= 0)
            {
                throw new ArgumentOutOfRangeException("The minimal value of item should be greater than 0");
            }
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException("The minimal value of item should be less than maximum value");
            }

            Random rnd = new Random();
            int weight = rnd.Next(minWeight,maxWeight);
            int value = rnd.Next(minValue, maxValue);
            return new Item(weight,value);
        }

        /// <summary>
        /// Generate list of items according to parameters specified.
        /// </summary>
        /// <param name="numberOfItems">Number of items generated</param>
        /// <param name="minWeight">Minimum weight of item</param>
        /// <param name="maxWeight">Maximum weight of item</param>
        /// <param name="minValue">Minimum value of item</param>
        /// <param name="maxValue">Maximum value of item</param>
        /// <returns>List of generated items</returns>
        public static List<Item> GenerateItems(int numberOfItems, int minWeight, int maxWeight, int minValue, int maxValue)
        {
            List<Item> items = new List<Item>();
            for (int i = 0; i < numberOfItems; i++)
            {
                items.Add(Item.Generate(minWeight,maxWeight,minValue,maxValue));
            }
            return items;
        }


        public override string ToString()
        {
            return "Weight: " + weight + " Value: " + value;
        }


    }
}
