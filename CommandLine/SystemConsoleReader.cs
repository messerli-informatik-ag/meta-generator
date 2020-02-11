using Funcky.Extensions;
using Funcky.Monads;
using Messerli.CommandLineAbstractions;

namespace Messerli.CommandLine
{
    public class SystemConsoleReader : IConsoleReader
    {
        public Option<int> ReadInt()
        {
            return ReadLine().TryParseInt();
        }

        public string ReadLine()
        {
            return System.Console.ReadLine();
        }
    }
}