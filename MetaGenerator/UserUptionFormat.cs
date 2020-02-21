using System.Linq;
using System.Text.RegularExpressions;

namespace Messerli.MetaGenerator
{
    internal static class UserUptionFormat
    {
        public static string ToUserOption(string userInputVariableName)
        {
            var parts = Regex
                .Split(userInputVariableName, @"(?<!^)(?=[A-Z])")
                .Select(part => part.ToLower());

            return $"--{string.Join("-", parts)}";
        }
    }
}
