using System;

namespace Messerli.MetaGeneratorAbstractions
{
    public interface IExceptionFormatter
    {
        void FormatException(Exception exception);
    }
}
