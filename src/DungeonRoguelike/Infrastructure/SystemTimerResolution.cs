using System.Runtime.InteropServices;

namespace DungeonRoguelike.Infrastructure;

/// <summary>
/// Eleva temporariamente a resolução do timer do sistema no Windows.
///
/// Por padrão, o timer do Windows tem granularidade de ~15,6 ms, fazendo
/// <see cref="Thread.Sleep(int)"/> dormir muito além do pedido e impedindo um
/// loop estável a 60 FPS. <c>timeBeginPeriod(1)</c> reduz isso para ~1 ms.
/// Em plataformas não-Windows é um no-op.
///
/// Uso: <c>using var _ = SystemTimerResolution.Acquire(1);</c> em torno do loop.
/// </summary>
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

    /// <summary>
    /// Aplica a resolução pedida (Windows). Retorna um handle que restaura a
    /// resolução original ao ser descartado.
    /// </summary>
    public static SystemTimerResolution Acquire(uint periodMs = 1)
    {
        if (OperatingSystem.IsWindows())
        {
            bool ok = TimeBeginPeriod(periodMs) == 0; // 0 == TIMERR_NOERROR
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
