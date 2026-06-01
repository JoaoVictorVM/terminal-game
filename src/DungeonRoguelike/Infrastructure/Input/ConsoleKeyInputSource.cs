using System.Diagnostics;
using DungeonRoguelike.Core;

namespace DungeonRoguelike.Infrastructure.Input;

// Fallback portável: o console só entrega eventos, não o estado real, então a
// tecla é considerada pressionada enquanto eventos de auto-repetição chegarem
// dentro da janela de timeout. No Windows prefira WindowsRawInputSource.
public sealed class ConsoleKeyInputSource : IInputSource
{
    private static readonly long ReleaseTimeoutTicks = (long)(0.12 * Stopwatch.Frequency);

    private static readonly GameKey[] AllKeys = Enum.GetValues<GameKey>();
    private readonly long[] _lastSeenTicks = new long[AllKeys.Length];
    private readonly bool[] _state = new bool[AllKeys.Length];

    public void Poll()
    {
        long now = Stopwatch.GetTimestamp();

        if (!Console.IsInputRedirected)
        {
            while (Console.KeyAvailable)
            {
                ConsoleKey consoleKey = Console.ReadKey(intercept: true).Key;
                if (TryMap(consoleKey, out GameKey key))
                    _lastSeenTicks[(int)key] = now;
            }
        }

        foreach (GameKey key in AllKeys)
            _state[(int)key] = _lastSeenTicks[(int)key] != 0
                               && now - _lastSeenTicks[(int)key] <= ReleaseTimeoutTicks;
    }

    public bool IsDown(GameKey key) => _state[(int)key];

    private static bool TryMap(ConsoleKey consoleKey, out GameKey key)
    {
        switch (consoleKey)
        {
            case ConsoleKey.UpArrow: key = GameKey.Up; return true;
            case ConsoleKey.DownArrow: key = GameKey.Down; return true;
            case ConsoleKey.LeftArrow: key = GameKey.Left; return true;
            case ConsoleKey.RightArrow: key = GameKey.Right; return true;
            case ConsoleKey.Z: key = GameKey.Attack; return true;
            case ConsoleKey.X: key = GameKey.Defend; return true;
            case ConsoleKey.Enter: key = GameKey.Confirm; return true;
            case ConsoleKey.Escape:
            case ConsoleKey.Q: key = GameKey.Cancel; return true;
            default: key = default; return false;
        }
    }
}
