using System.CommandLine.Invocation;
using Messerli.MetaGeneratorAbstractions;

namespace Messerli.MetaGenerator
{
    internal interface IGenerationSteps
    {
        int Execute(IMetaGenerator metaTypeGenerator, InvocationContext arguments);
    }
}