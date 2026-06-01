using DungeonRoguelike.Core;
using DungeonRoguelike.Infrastructure;

namespace DungeonRoguelike.Game;

/// <summary>
/// Cena temporária da Etapa 1.2: não há gameplay ainda, apenas a comprovação
/// visual de que o loop roda em FPS fixo estável.
///
/// Não usa <c>Console.Clear()</c> (PRD §16.4); reescreve as mesmas linhas via
/// <see cref="Console.SetCursorPosition"/>. Será substituída pelo renderer com
/// double buffer na Etapa 1.3.
/// </summary>
public sealed class DiagnosticsScene : IGameScene
{
    private readonly GameClock _clock;
    private double _elapsedSeconds;

    public DiagnosticsScene(GameClock clock)
    {
        _clock = clock;
    }

    public bool IsRunning => true; // encerra apenas via Ctrl+C nesta etapa.

    public void Update(double deltaSeconds)
    {
        _elapsedSeconds += deltaSeconds;
    }

    public void Render()
    {
        // Atualiza o painel ~10x por segundo para o número de FPS ficar legível.
        if (_clock.TotalFrames % Math.Max(1, _clock.TargetFps / 10) != 0)
            return;

        // Saída redirecionada (ex.: pipe/teste): sem cursor; emite uma linha simples.
        if (Console.IsOutputRedirected)
        {
            Console.WriteLine(
                $"FPS real={_clock.MeasuredFps:0.0} alvo={_clock.TargetFps} " +
                $"frames={_clock.TotalFrames} tempo={_elapsedSeconds:0.0}s");
            return;
        }

        Console.SetCursorPosition(0, 0);
        Console.Write($"FPS alvo : {_clock.TargetFps,6}");
        Console.SetCursorPosition(0, 1);
        Console.Write($"FPS real : {_clock.MeasuredFps,6:0.0}");
        Console.SetCursorPosition(0, 2);
        Console.Write($"Frames   : {_clock.TotalFrames,6}");
        Console.SetCursorPosition(0, 3);
        Console.Write($"Tempo    : {_elapsedSeconds,6:0.0}s");
        Console.SetCursorPosition(0, 5);
        Console.Write("Pressione Ctrl+C para encerrar.");
    }
}
