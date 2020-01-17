using System;

namespace Messerli.ProjectAbstractions
{
    public interface ITimeKeeper
    {
        void MeasureTime(Action action, string eventName);

        void Print();
    }
}