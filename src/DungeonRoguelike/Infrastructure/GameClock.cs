using System.Diagnostics;

namespace DungeonRoguelike.Infrastructure;

/// <summary>
/// Relógio de timestep fixo para o game loop em tempo real (PRD §4.1).
///
/// Objetivo: manter um FPS alvo estável (padrão 60). Após o trabalho de cada
/// frame, espera o tempo restante usando uma estratégia híbrida
/// (<see cref="Thread.Sleep(int)"/> grosseiro + <see cref="Thread.SpinWait"/>
/// fino), já que o sleep do Windows tem resolução de ~15 ms e sozinho não
/// sustenta 60 FPS com precisão.
/// </summary>
public sealed class GameClock
{
    private readonly double _targetFrameSeconds;
    private readonly Stopwatch _stopwatch = new();

    private long _previousTicks;
    private double _nextFrameSeconds;
    private double _fpsAccumulatorSeconds;
    private int _fpsFrameCounter;

    /// <param name="targetFps">FPS desejado. Padrão: 60.</param>
    public GameClock(int targetFps = 60)
    {
        if (targetFps <= 0)
            throw new ArgumentOutOfRangeException(nameof(targetFps), "FPS alvo deve ser positivo.");

        TargetFps = targetFps;
        _targetFrameSeconds = 1.0 / targetFps;
    }

    /// <summary>FPS alvo configurado.</summary>
    public int TargetFps { get; }

    /// <summary>Tempo decorrido (em segundos) do último frame completo.</summary>
    public double DeltaSeconds { get; private set; }

    /// <summary>Total de frames processados desde <see cref="Start"/>.</summary>
    public long TotalFrames { get; private set; }

    /// <summary>FPS real medido, atualizado ~1x por segundo para leitura estável.</summary>
    public double MeasuredFps { get; private set; }

    /// <summary>Inicia/zera o relógio. Deve ser chamado antes do primeiro frame.</summary>
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

    /// <summary>
    /// Bloqueia até o instante do próximo frame, calculando o
    /// <see cref="DeltaSeconds"/> real e atualizando o FPS medido.
    /// </summary>
    public void WaitForNextFrame()
    {
        // Espera híbrida até o instante agendado: dorme o grosso, refina com spin.
        while (true)
        {
            double remaining = _nextFrameSeconds - TicksToSeconds(_stopwatch.ElapsedTicks);
            if (remaining <= 0)
                break;

            if (remaining > 0.002) // > 2 ms: cede a CPU.
                Thread.Sleep(1);
            else
                Thread.SpinWait(64); // últimos instantes: ajuste fino.
        }

        long now = _stopwatch.ElapsedTicks;
        DeltaSeconds = TicksToSeconds(now - _previousTicks);
        _previousTicks = now;
        TotalFrames++;

        // Agenda o próximo frame de forma absoluta (mantém média de FPS exata).
        _nextFrameSeconds += _targetFrameSeconds;

        // Proteção contra "spiral of death": se atrasamos mais de um frame
        // (ex.: travada do SO), reancora o cronograma ao tempo atual.
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
