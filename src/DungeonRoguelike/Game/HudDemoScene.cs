using DungeonRoguelike.Collision;
using DungeonRoguelike.Core;
using DungeonRoguelike.Entities;
using DungeonRoguelike.Entities.Components;
using DungeonRoguelike.Entities.Systems;
using DungeonRoguelike.Infrastructure.Input;
using DungeonRoguelike.Rendering;
using DungeonRoguelike.UI;

namespace DungeonRoguelike.Game;

// Cena temporária da Etapa 2.4: movimento/colisão das etapas anteriores + HUD
// de vida. Z/Enter alteram a vida apenas para validar o HUD; o sistema de
// saúde real chega na Etapa 4.1.
public sealed class HudDemoScene : IGameScene
{
    private const int MaxHearts = 3;
    private const char WallSprite = '#';

    private readonly Renderer _renderer;
    private readonly InputSystem _input;
    private readonly PlayerController _controller;
    private readonly TileMap _map;
    private readonly Entity _player;
    private readonly Hud _hud = new();

    private int _hearts = MaxHearts;
    private bool _quit;

    public HudDemoScene(Renderer renderer, InputSystem input)
    {
        _renderer = renderer;
        _input = input;
        _controller = new PlayerController(input, new CollisionManager());
        _map = BuildMap(renderer.Width, renderer.Height);
        _player = CreatePlayer(renderer.Width / 2, renderer.Height / 2);
    }

    public bool IsRunning => !_quit;

    public void Update(double deltaSeconds)
    {
        _input.Update();

        if (_input.WasPressed(GameKey.Cancel))
            _quit = true;

        if (_input.WasPressed(GameKey.Attack))
            _hearts = Math.Max(0, _hearts - 1);
        if (_input.WasPressed(GameKey.Confirm))
            _hearts = MaxHearts;

        _controller.Update(_player, deltaSeconds, _map);
    }

    public void Render()
    {
        _renderer.Clear();
        DrawMap();

        _hud.Draw(_renderer, _hearts, MaxHearts);
        _renderer.Write(2, _renderer.Height - 1, " setas: mover | Z(debug): -1 vida | Enter: reset | Esc/Q: sair ");

        Direction facing = _player.Require<DirectionComponent>().Facing;
        PositionComponent position = _player.Require<PositionComponent>();
        _renderer.Set(position.TileX, position.TileY, _player.Require<RendererComponent>().SpriteFor(facing));

        _renderer.Present();
    }

    private void DrawMap()
    {
        for (int y = 0; y < _map.Height; y++)
            for (int x = 0; x < _map.Width; x++)
                if (_map.IsSolid(x, y))
                    _renderer.Set(x, y, WallSprite);
    }

    private static TileMap BuildMap(int width, int height)
    {
        var map = new TileMap(width, height);

        for (int x = 0; x < width; x++)
        {
            map.SetSolid(x, 0, true);
            map.SetSolid(x, height - 1, true);
        }

        for (int y = 0; y < height; y++)
        {
            map.SetSolid(0, y, true);
            map.SetSolid(width - 1, y, true);
        }

        return map;
    }

    private static Entity CreatePlayer(int tileX, int tileY)
    {
        return new Entity()
            .Add(new PositionComponent(tileX, tileY))
            .Add(new DirectionComponent(Direction.Down))
            .Add(new RendererComponent(up: '^', down: 'v', left: '<', right: '>'));
    }
}
