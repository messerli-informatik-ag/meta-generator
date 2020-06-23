namespace Messerli.CommandLineAbstractions
{
    public interface IConsoleWriter
    {
        void WriteLine(string line);

        void WriteLine();

        void Write(string value);
    }
}