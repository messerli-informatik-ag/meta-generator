using System;

namespace Messerli.MetaGeneratorAbstractions
{
    public interface ITimeKeeper
    {
        void MeasureTime(Action action, string eventName);

        void Print();
    }
}