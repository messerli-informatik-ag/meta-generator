using System;

namespace Messerli.FileManipulatorAbstractions
{
    public sealed class GlobalJsonManipulationException : Exception
    {
        public GlobalJsonManipulationException(string message)
            : base(message)
        {
        }
    }
}