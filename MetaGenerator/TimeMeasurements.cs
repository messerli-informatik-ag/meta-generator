namespace Messerli.MetaGenerator
{
    internal class TimeMeasurements
    {
        public TimeMeasurements(string eventName, long elapsedMilliseconds)
        {
            EventName = eventName;
            ElapsedMilliseconds = elapsedMilliseconds;
        }

        public string EventName { get; }

        public long ElapsedMilliseconds { get; }
    }
}