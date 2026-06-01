using DungeonRoguelike.Core;
using DungeonRoguelike.Infrastructure;
using DungeonRoguelike.Infrastructure.Input;
using DungeonRoguelike.Rendering;

namespace DungeonRoguelike.Game;

// Cena temporária da Etapa 1.4; será substituída pelo jogador real na Fase 2.
public sealed class InputDemoScene : IGameScene
{
    private const double MoveSpeed = 16.0;
    private const double AttackFlashSeconds = 0.15;

    private readonly Renderer _renderer;
    private readonly InputSystem _input;
    private readonly GameClock _clock;

    private double _x;
    private double _y;
    private double _attackFlash;
    private bool _quit;

    public InputDemoScene(Renderer renderer, InputSystem input, GameClock clock)
    {
        _renderer = renderer;
        _input = input;
        _clock = clock;
        _x = renderer.Width / 2.0;
        _y = renderer.Height / 2.0;
    }

    public bool IsRunning => !_quit;

    public void Update(double deltaSeconds)
    {
        _input.Update();

        if (_input.WasPressed(GameKey.Cancel))
            _quit = true;

        // Defender impede movimento (PRD §7.1).
        if (!_input.IsDown(GameKey.Defend))
        {
            double dx = 0, dy = 0;
            if (_input.IsDown(GameKey.Left)) dx -= 1;
            if (_input.IsDown(GameKey.Right)) dx += 1;
            if (_input.IsDown(GameKey.Up)) dy -= 1;
            if (_input.IsDown(GameKey.Down)) dy += 1;

            _x += dx * MoveSpeed * deltaSeconds;
            _y += dy * MoveSpeed * deltaSeconds;
            _x = Math.Clamp(_x, 1, _renderer.Width - 2);
            _y = Math.Clamp(_y, 1, _renderer.Height - 2);
        }

        if (_input.WasPressed(GameKey.Attack))
            _attackFlash = AttackFlashSeconds;
        else if (_attackFlash > 0)
            _attackFlash = Math.Max(0, _attackFlash - deltaSeconds);
    }

    public void Render()
    {
        _renderer.Clear();
        DrawBorder();

        _renderer.Write(2, 0, $" FPS {_clock.MeasuredFps:0.0} / {_clock.TargetFps} ");
        _renderer.Write(2, _renderer.Height - 1, " setas: mover | Z: atacar | X: defender | Esc/Q: sair ");

        string keys = $"^{Mark(GameKey.Up)} v{Mark(GameKey.Down)} <{Mark(GameKey.Left)} >{Mark(GameKey.Right)}  " +
                      $"Z{Mark(GameKey.Attack)} X{Mark(GameKey.Defend)}";
        _renderer.Write(2, 1, keys);

        if (_input.IsDown(GameKey.Defend))
            _renderer.Write(2, 2, "DEFENDENDO");
        else if (_attackFlash > 0)
            _renderer.Write(2, 2, "ATAQUE!   ");
        else
            _renderer.Write(2, 2, "          ");

        _renderer.Set((int)_x, (int)_y, '@');

        _renderer.Present();
    }

    private string Mark(GameKey key) => _input.IsDown(key) ? "*" : ".";

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
