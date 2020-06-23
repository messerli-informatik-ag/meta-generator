using System;

namespace Messerli.VsSolution.Model
{
    public static class GuidExtensions
    {
        public static string SolutionFormat(this Guid guid)
        {
            return $"{{{guid.ToString().ToUpper()}}}";
        }
    }
}
