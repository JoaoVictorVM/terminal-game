using DungeonRoguelike.Core;
using DungeonRoguelike.Entities;
using DungeonRoguelike.Entities.Components;
using DungeonRoguelike.Entities.Systems;
using DungeonRoguelike.Infrastructure.Input;
using DungeonRoguelike.Rendering;

namespace DungeonRoguelike.Game;

// Cena temporária da Etapa 2.2: movimento contínuo + sprite direcional. O clamp
// na viewport é provisório; a colisão real com paredes entra na Etapa 2.3.
public sealed class PlayerDemoScene : IGameScene
{
    private readonly Renderer _renderer;
    private readonly InputSystem _input;
    private readonly PlayerController _controller;
    private readonly Entity _player;

    private bool _quit;

    public PlayerDemoScene(Renderer renderer, InputSystem input)
    {
        _renderer = renderer;
        _input = input;
        _controller = new PlayerController(input);
        _player = CreatePlayer(renderer.Width / 2, renderer.Height / 2);
    }

    public bool IsRunning => !_quit;

    public void Update(double deltaSeconds)
    {
        _input.Update();

        if (_input.WasPressed(GameKey.Cancel))
            _quit = true;

        _controller.Update(_player, deltaSeconds);
        ClampToViewport(_player.Require<PositionComponent>());
    }

    public void Render()
    {
        _renderer.Clear();
        DrawBorder();

        PositionComponent position = _player.Require<PositionComponent>();
        Direction facing = _player.Require<DirectionComponent>().Facing;

        _renderer.Write(2, 0, " Etapa 2.2 - Movimento ");
        _renderer.Write(2, 1, $" Pos: {position.X,5:0.0},{position.Y,5:0.0}  Dir: {facing} ");
        _renderer.Write(2, _renderer.Height - 1, " setas: mover | Esc/Q: sair ");

        DrawEntity(_player);

        _renderer.Present();
    }

    private void ClampToViewport(PositionComponent position)
    {
        position.X = Math.Clamp(position.X, 1f, _renderer.Width - 2f);
        position.Y = Math.Clamp(position.Y, 1f, _renderer.Height - 2f);
    }

    private void DrawEntity(Entity entity)
    {
        PositionComponent position = entity.Require<PositionComponent>();
        RendererComponent sprite = entity.Require<RendererComponent>();
        Direction facing = entity.Require<DirectionComponent>().Facing;
        _renderer.Set(position.TileX, position.TileY, sprite.SpriteFor(facing));
    }

    private static Entity CreatePlayer(int tileX, int tileY)
    {
        return new Entity()
            .Add(new PositionComponent(tileX, tileY))
            .Add(new DirectionComponent(Direction.Down))
            .Add(new RendererComponent(up: '^', down: 'v', left: '<', right: '>'));
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
