using System.Collections.Generic;
using Messerli.MetaGeneratorAbstractions.Json;

namespace Messerli.ManagedWrapperProjectsPlugin;

internal interface IPathProvider
{
    IEnumerable<SelectionValue> GetBranches();

    string GetTfsProjectRoot();

    string GetProjectPath();

    string GetVersionInfoPath();

    string GetSolutionDirectory();

    string GetBuildStepSignDirectory();

    string GetVisualStudioToolDirectory();
}
