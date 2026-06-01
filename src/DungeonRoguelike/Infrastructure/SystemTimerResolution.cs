using System.Runtime.InteropServices;

namespace DungeonRoguelike.Infrastructure;

// Por padrão o timer do Windows tem granularidade de ~15,6 ms, fazendo
// Thread.Sleep(1) dormir muito além do pedido e impedindo um loop estável a
// 60 FPS. timeBeginPeriod(1) reduz isso para ~1 ms enquanto o handle viver.
public sealed class SystemTimerResolution : IDisposable
{
    [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
    private static extern uint TimeBeginPeriod(uint milliseconds);

    [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
    private static extern uint TimeEndPeriod(uint milliseconds);

    private readonly uint _periodMs;
    private readonly bool _active;
    private bool _disposed;

    private SystemTimerResolution(uint periodMs, bool active)
    {
        _periodMs = periodMs;
        _active = active;
    }

    public static SystemTimerResolution Acquire(uint periodMs = 1)
    {
        if (OperatingSystem.IsWindows())
        {
            bool ok = TimeBeginPeriod(periodMs) == 0;
            return new SystemTimerResolution(periodMs, ok);
        }

        return new SystemTimerResolution(periodMs, active: false);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        if (_active && OperatingSystem.IsWindows())
            TimeEndPeriod(_periodMs);
    }
}
