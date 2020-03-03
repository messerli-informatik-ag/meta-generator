using Soltys.ChangeCase;

namespace Messerli.MetaGenerator
{
    internal static class UserUptionFormat
    {
        public static string ToUserOption(string userInputVariableName)
        {
            return $"--{userInputVariableName.ParamCase()}";
        }
    }
}
