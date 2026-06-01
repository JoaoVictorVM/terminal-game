using DungeonRoguelike.Core;
using DungeonRoguelike.Infrastructure;
using DungeonRoguelike.Rendering;

namespace DungeonRoguelike.Game;

/// <summary>
/// Cena temporária da Etapa 1.3: comprova o double buffer e a renderização
/// parcial. Desenha uma borda estática (que nunca é reescrita após o 1º frame)
/// e um caractere quicando em posição float (PRD §4.2), de modo que apenas as
/// células do caractere mudem a cada frame. Será substituída pelo gameplay real
/// nas próximas fases.
/// </summary>
public sealed class RenderDemoScene : IGameScene
{
    private const char Marker = '@';

    private readonly Renderer _renderer;
    private readonly GameClock _clock;

    private double _x;
    private double _y;
    private double _vx = 18.0; // células/segundo
    private double _vy = 11.0;
    private bool _quit;

    public RenderDemoScene(Renderer renderer, GameClock clock)
    {
        _renderer = renderer;
        _clock = clock;
        _x = renderer.Width / 2.0;
        _y = renderer.Height / 2.0;
    }

    public bool IsRunning => !_quit;

    public void Update(double deltaSeconds)
    {
        PollQuitKey();

        _x += _vx * deltaSeconds;
        _y += _vy * deltaSeconds;

        // Quica dentro da área interna (respeitando a borda).
        if (_x < 1) { _x = 1; _vx = -_vx; }
        if (_x > _renderer.Width - 2) { _x = _renderer.Width - 2; _vx = -_vx; }
        if (_y < 1) { _y = 1; _vy = -_vy; }
        if (_y > _renderer.Height - 2) { _y = _renderer.Height - 2; _vy = -_vy; }
    }

    /// <summary>
    /// Leitura não-bloqueante: encerra com Esc ou Q. O input completo (setas,
    /// Z, X) chega na Etapa 1.4; aqui é apenas uma saída confiável.
    /// </summary>
    private void PollQuitKey()
    {
        if (Console.IsInputRedirected)
            return;

        while (Console.KeyAvailable)
        {
            ConsoleKey key = Console.ReadKey(intercept: true).Key;
            if (key is ConsoleKey.Escape or ConsoleKey.Q)
                _quit = true;
        }
    }

    public void Render()
    {
        _renderer.Clear();
        DrawBorder();

        _renderer.Write(2, 0, $" FPS {_clock.MeasuredFps:0.0} / {_clock.TargetFps} ");
        _renderer.Write(2, _renderer.Height - 1, " Esc/Q para sair ");

        _renderer.Set((int)_x, (int)_y, Marker);

        _renderer.Present();
    }

    private void DrawBorder()
    {
        int w = _renderer.Width;
        int h = _renderer.Height;

        for (int x = 0; x < w; x++)
        {
            _renderer.Set(x, 0, '-');
            _renderer.Set(x, h - 1, '-');
        }

        for (int y = 0; y < h; y++)
        {
            _renderer.Set(0, y, '|');
            _renderer.Set(w - 1, y, '|');
        }

        _renderer.Set(0, 0, '+');
        _renderer.Set(w - 1, 0, '+');
        _renderer.Set(0, h - 1, '+');
        _renderer.Set(w - 1, h - 1, '+');
    }
}
