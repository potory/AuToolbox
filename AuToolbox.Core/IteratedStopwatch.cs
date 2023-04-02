using System.Diagnostics;

namespace AuToolbox.Core;

public class IteratedStopwatch
{
    private readonly Stopwatch _sw = new();

    private int _totalIterations;
    private int _currentIteration;
    
    private TimeSpan _averageTime;
    private TimeSpan _elapsedTime;

    public TimeSpan AverageTime => _averageTime;
    public TimeSpan ElapsedTime => _elapsedTime;

    public TimeSpan RemainingTime => TimeSpan.FromTicks(
        _averageTime.Ticks * (_totalIterations - _currentIteration)
    );

    public void Start(int count)
    {
        _currentIteration = 0;
        _totalIterations = count;

        _averageTime = TimeSpan.Zero;
        _elapsedTime = TimeSpan.Zero;

        _sw.Reset();
        _sw.Start();
    }

    public void NextIteration()
    {
        _sw.Stop();
        _currentIteration++;
        _averageTime = TimeSpan.FromTicks(
            (_averageTime.Ticks * (_currentIteration - 1) + _sw.Elapsed.Ticks) / _currentIteration
        );
        _elapsedTime += _sw.Elapsed;
        _sw.Restart();
    }

    public void Stop()
    {
        _sw.Stop();
        _elapsedTime += _sw.Elapsed;
    }
}