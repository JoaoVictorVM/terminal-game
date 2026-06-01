using DungeonRoguelike.Core;
using DungeonRoguelike.Infrastructure;

namespace DungeonRoguelike.Game;

public sealed class GameLoop
{
    private readonly GameClock _clock;
    private volatile bool _stopRequested;

    public GameLoop(int targetFps = 60)
    {
        _clock = new GameClock(targetFps);
    }

    public GameClock Clock => _clock;

    public void Run(IGameScene scene)
    {
        ArgumentNullException.ThrowIfNull(scene);

        ConsoleCancelEventHandler onCancel = (_, e) =>
        {
            e.Cancel = true; // impede término abrupto para encerrar de forma limpa
            _stopRequested = true;
        };
        Console.CancelKeyPress += onCancel;

        using var timerResolution = SystemTimerResolution.Acquire(1);

        try
        {
            _clock.Start();
            while (scene.IsRunning && !_stopRequested)
            {
                _clock.WaitForNextFrame();
                scene.Update(_clock.DeltaSeconds);
                scene.Render();
            }
        }
        finally
        {
            Console.CancelKeyPress -= onCancel;
        }
    }
}
