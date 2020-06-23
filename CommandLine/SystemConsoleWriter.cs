using Messerli.CommandLineAbstractions;

namespace Messerli.CommandLine
{
    public class SystemConsoleWriter : IConsoleWriter
    {
        public void WriteLine(string line)
        {
            System.Console.WriteLine(line);
        }

        public void WriteLine()
        {
            System.Console.WriteLine();
        }

        public void Write(string value)
        {
            System.Console.Write(value);
        }
    }
}