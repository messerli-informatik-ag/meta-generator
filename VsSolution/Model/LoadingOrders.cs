using System.Collections.Generic;

namespace Messerli.VsSolution.Model
{
    internal class LoadingOrders
    {
        private static readonly HashSet<string> ValidSolutionLoadingOrders = new HashSet<string> { "preSolution", "postSolution" };
        private static readonly HashSet<string> ValidProjectLoadingOrders = new HashSet<string> { "preProject", "postProject" };

        public static bool IsValidLoadingOrder(string loadingOrder)
        {
            return IsValidProjectLoadingOrder(loadingOrder) || IsValidSolutionLoadingOrder(loadingOrder);
        }

        public static bool IsValidProjectLoadingOrder(string loadingOrder)
        {
            return ValidProjectLoadingOrders.Contains(loadingOrder);
        }

        public static bool IsValidSolutionLoadingOrder(string loadingOrder)
        {
            return ValidSolutionLoadingOrders.Contains(loadingOrder);
        }
    }
}