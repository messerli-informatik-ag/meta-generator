using System;

namespace Messerli.FileManipulatorAbstractions
{
    public sealed class MalformedGlobalJsonException : Exception
    {
        public MalformedGlobalJsonException(string message)
            : base(message)
        {
        }
    }
}