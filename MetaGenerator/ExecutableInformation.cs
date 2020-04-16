using System;

namespace Messerli.MetaGenerator
{
    public static class ExecutableInformation
    {
        public static string GetExecutableDirectory()
        {
            return AppContext.BaseDirectory;
        }
    }
}
