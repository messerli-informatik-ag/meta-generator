using System;

namespace Messerli.FileManipulatorAbstractions.Project;

public sealed class ProjectManipulationException : Exception
{
    private readonly string _projectFilePath;

    public ProjectManipulationException(Exception innerException, string projectFilePath)
        : base(null, innerException)
    {
        _projectFilePath = projectFilePath;
    }

    public override string Message =>
        $"Project file manipulation of '{_projectFilePath}' failed. See inner exception for details";
}
