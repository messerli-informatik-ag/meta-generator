using System;
using System.Runtime.Serialization;

namespace Messerli.VsSolution.Model;

[Serializable]
internal class UnknownTypeGuidException : Exception
{
    public UnknownTypeGuidException()
    {
    }

    public UnknownTypeGuidException(Guid guid)
        : this($"Unknown type guid in solution found: {guid}")
    {
    }

    public UnknownTypeGuidException(string message)
        : base(message)
    {
    }

    public UnknownTypeGuidException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected UnknownTypeGuidException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
