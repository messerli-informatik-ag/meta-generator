using System;
using Messerli.CommandLineAbstractions;

namespace Messerli.CommandLine
{
    public class SystemConsoleWriter : IConsoleWriter
    {
        public void WriteLine(string line)
            => Console.WriteLine(line);

        public void WriteLine()
            => Console.WriteLine();

        public void Write(string value)
            => Console.Write(value);
    }
}
