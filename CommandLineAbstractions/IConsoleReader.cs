using Funcky.Monads;

namespace Messerli.CommandLineAbstractions
{
    public interface IConsoleReader
    {
        Option<int> ReadInt();

        string ReadLine();
    }
}