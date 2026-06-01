using System.Diagnostics;

namespace DungeonRoguelike.Infrastructure;

public sealed class GameClock
{
    private readonly double _targetFrameSeconds;
    private readonly Stopwatch _stopwatch = new();

    private long _previousTicks;
    private double _nextFrameSeconds;
    private double _fpsAccumulatorSeconds;
    private int _fpsFrameCounter;

    public GameClock(int targetFps = 60)
    {
        if (targetFps <= 0)
            throw new ArgumentOutOfRangeException(nameof(targetFps));

        TargetFps = targetFps;
        _targetFrameSeconds = 1.0 / targetFps;
    }

    public int TargetFps { get; }

    public double DeltaSeconds { get; private set; }

    public long TotalFrames { get; private set; }

    public double MeasuredFps { get; private set; }

    public void Start()
    {
        _stopwatch.Restart();
        _previousTicks = _stopwatch.ElapsedTicks;
        _nextFrameSeconds = TicksToSeconds(_previousTicks) + _targetFrameSeconds;
        DeltaSeconds = _targetFrameSeconds;
        TotalFrames = 0;
        _fpsAccumulatorSeconds = 0;
        _fpsFrameCounter = 0;
        MeasuredFps = TargetFps;
    }

    public void WaitForNextFrame()
    {
        while (true)
        {
            double remaining = _nextFrameSeconds - TicksToSeconds(_stopwatch.ElapsedTicks);
            if (remaining <= 0)
                break;

            // Sleep do Windows tem granularidade grosseira; cede a CPU enquanto
            // há folga e refina o instante final com spin para manter 60 FPS.
            if (remaining > 0.002)
                Thread.Sleep(1);
            else
                Thread.SpinWait(64);
        }

        long now = _stopwatch.ElapsedTicks;
        DeltaSeconds = TicksToSeconds(now - _previousTicks);
        _previousTicks = now;
        TotalFrames++;

        // Agenda absoluta (+= alvo) mantém a média de FPS exata mesmo com
        // pequenos overshoots por frame.
        _nextFrameSeconds += _targetFrameSeconds;

        // Reancora se atrasamos mais de um frame (ex.: travada do SO), evitando
        // o "spiral of death" de tentar recuperar muitos frames de uma vez.
        double nowSeconds = TicksToSeconds(now);
        if (_nextFrameSeconds < nowSeconds)
            _nextFrameSeconds = nowSeconds + _targetFrameSeconds;

        UpdateMeasuredFps();
    }

    private void UpdateMeasuredFps()
    {
        _fpsAccumulatorSeconds += DeltaSeconds;
        _fpsFrameCounter++;

        if (_fpsAccumulatorSeconds >= 1.0)
        {
            MeasuredFps = _fpsFrameCounter / _fpsAccumulatorSeconds;
            _fpsAccumulatorSeconds = 0;
            _fpsFrameCounter = 0;
        }
    }

    private static double TicksToSeconds(long ticks) => (double)ticks / Stopwatch.Frequency;
}
