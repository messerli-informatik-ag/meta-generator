using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Messerli.CommandLineAbstractions;
using Messerli.MetaGeneratorAbstractions;
using static System.Math;

namespace Messerli.MetaGenerator;

internal class TimeKeeper : ITimeKeeper
{
    private readonly IConsoleWriter _consoleWriter;
    private readonly List<TimeMeasurements> _measurements;

    public TimeKeeper(IConsoleWriter consoleWriter)
    {
        _consoleWriter = consoleWriter;
        _measurements = new List<TimeMeasurements>();
    }

    public void MeasureTime(Action action, string eventName)
    {
        var stopWatch = new Stopwatch();

        stopWatch.Start();
        action();
        stopWatch.Stop();

        _measurements.Add(new TimeMeasurements(eventName, stopWatch.ElapsedMilliseconds));
    }

    public void Print()
    {
        _consoleWriter.WriteLine();

        PrintMeasurements();

        PrintSeperatingLine();
        PrintTotal();
    }

    private void PrintSeperatingLine()
    {
        _consoleWriter.WriteLine(new string('-', EventNameColumnWidth() + ElapsedColumnWidth() + 4));
    }

    private void PrintTotal()
    {
        _consoleWriter.WriteLine($"{"Total".PadRight(EventNameColumnWidth())}: {TotalTime().ToString().PadLeft(ElapsedColumnWidth())}ms");
    }

    private void PrintMeasurements()
    {
        foreach (var measurement in _measurements)
        {
            _consoleWriter.WriteLine($"{measurement.EventName.PadRight(EventNameColumnWidth())}: {measurement.ElapsedMilliseconds.ToString().PadLeft(ElapsedColumnWidth())}ms");
        }
    }

    private long TotalTime()
        => _measurements
            .Select(m => m.ElapsedMilliseconds)
            .Sum();

    private int EventNameColumnWidth()
        => _measurements
            .Aggregate(0, (max, m) => Max(max, m.EventName.Length)) + 1;

    private int ElapsedColumnWidth()
        => _measurements
            .Aggregate(0, (max, m) => Max(max, m.ElapsedMilliseconds.ToString().Length)) + 1;
}
