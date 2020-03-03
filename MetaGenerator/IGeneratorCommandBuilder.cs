using System.CommandLine;

namespace Messerli.MetaGenerator
{
    internal interface IGeneratorCommandBuilder
    {
        Command Build(string name);
    }
}