namespace Combinations
{
    /// <summary>
    /// Class used for creating combinations
    /// </summary>
    /// <typeparam name="T">Type of values to make combinations from</typeparam>
    public class Combination<T>    {


        /// <summary>
        /// Returns all combinations from list of values
        /// </summary>
        /// <typeparam name="T">Type of values</typeparam>
        /// <param name="values">List of values</param>
        /// <returns>List of Lists(Combinations)</returns>
        public static List<List<T>> GetCombinations<T>(List<T> values)
        {
            List<List<T>> result = new List<List<T>>();
            GetCombinationsHelper(values , new List<T>(), result);
            return result;
        }
        /// <summary>
        /// Helper function performing recursion for calculating the permutations.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="remainingValues"></param>
        /// <param name="currentCombination"></param>
        /// <param name="result"></param>
        private static void GetCombinationsHelper<T>(List<T> remainingValues, List<T> currentCombination, List<List<T>> result)
        {
            if (remainingValues.Count == 0)
            {
                result.Add(currentCombination);
                return;
            }

            T firstValue = remainingValues[0];
            List<T> remainingValuesCopy = new List<T>(remainingValues);
            remainingValuesCopy.RemoveAt(0);

            // Option 1: include the first value in the combination
            List<T> newCombination = new List<T>(currentCombination);
            newCombination.Add(firstValue);
            GetCombinationsHelper(remainingValuesCopy, newCombination, result);

            // Option 2: exclude the first value from the combination
            GetCombinationsHelper(remainingValuesCopy, currentCombination, result);
        }


    }
}