using System;
using Funcky.Extensions;
using Funcky.Monads;
using Messerli.CommandLineAbstractions;

namespace Messerli.CommandLine
{
    public class SystemConsoleReader : IConsoleReader
    {
        public Option<int> ReadInt()
            => ReadLine().TryParseInt();

        public string ReadLine()
            => Option.FromNullable(System.Console.ReadLine()).GetOrElse(() => throw new Exception("unreachable"));
    }
}
