using Soltys.ChangeCase;

namespace Messerli.MetaGenerator
{
    internal static class UserOptionFormat
    {
        public static string ToUserOption(string userInputVariableName)
        {
            return $"--{userInputVariableName.ParamCase()}";
        }
    }
}
