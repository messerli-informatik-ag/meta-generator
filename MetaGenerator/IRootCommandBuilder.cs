using System.CommandLine;

namespace Messerli.MetaGenerator
{
    public interface IRootCommandBuilder
    {
        RootCommand Build();
    }
}