using DungeonRoguelike.Core;
using DungeonRoguelike.Infrastructure;

namespace DungeonRoguelike.Game;

/// <summary>
/// Orquestra o loop principal em tempo real (PRD §16.3).
///
/// Ordem por frame nesta etapa: Update → Render, com paçamento de FPS fixo
/// feito pelo <see cref="GameClock"/>. Input dedicado entra na Etapa 1.4.
/// O loop encerra quando a cena pede parada ou ao receber Ctrl+C.
/// </summary>
public sealed class GameLoop
{
    private readonly GameClock _clock;
    private volatile bool _stopRequested;

    public GameLoop(int targetFps = 60)
    {
        _clock = new GameClock(targetFps);
    }

    /// <summary>Exposto para diagnóstico/leitura de FPS pela cena, se desejado.</summary>
    public GameClock Clock => _clock;

    /// <summary>Executa o loop até a cena terminar ou Ctrl+C ser pressionado.</summary>
    public void Run(IGameScene scene)
    {
        ArgumentNullException.ThrowIfNull(scene);

        ConsoleCancelEventHandler onCancel = (_, e) =>
        {
            e.Cancel = true; // impede término abrupto; encerra de forma limpa.
            _stopRequested = true;
        };
        Console.CancelKeyPress += onCancel;

        // Eleva a resolução do timer do SO para Sleep(1) preciso → 60 FPS estável.
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
