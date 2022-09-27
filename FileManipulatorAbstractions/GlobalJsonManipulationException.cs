using System;

namespace Messerli.FileManipulatorAbstractions;

public sealed class GlobalJsonManipulationException : Exception
{
    private readonly string _filePath;

    public GlobalJsonManipulationException(Exception innerException, string filePath)
        : base(null, innerException)
    {
        _filePath = filePath;
    }

    public override string Message => $"Error manipulating file {_filePath} see inner exception for details";
}
