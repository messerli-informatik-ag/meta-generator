using System;
using System.Diagnostics;
using System.IO;
using Funcky.Monads;

namespace Messerli.MetaGenerator
{
    public static class ExecutableInformation
    {
        public static Option<string> GetExecutablePath()
        {
            using (var process = Process.GetCurrentProcess())
            {
                return process.MainModule == null
                    ? Option<string>.None()
                    : Option.Some(process.MainModule.FileName);
            }
        }

        public static Option<string> GetExecutableDirectory()
        {
            return GetExecutablePath()
                .AndThen(ExtractDirectory);
        }

        private static string ExtractDirectory(string path)
        {
            var directory = Path.GetDirectoryName(path);

            return directory
                   ?? throw new Exception("Failed to get directory of executable. GetDirectoryName returned null.");
        }
    }
}
