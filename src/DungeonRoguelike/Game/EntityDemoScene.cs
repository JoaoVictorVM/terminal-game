using DungeonRoguelike.Core;
using DungeonRoguelike.Entities;
using DungeonRoguelike.Entities.Components;
using DungeonRoguelike.Infrastructure.Input;
using DungeonRoguelike.Rendering;

namespace DungeonRoguelike.Game;

// Cena temporária da Etapa 2.1: prova Position + Direction + sprite por direção.
// As setas apenas viram o personagem; o movimento contínuo entra na Etapa 2.2.
public sealed class EntityDemoScene : IGameScene
{
    private readonly Renderer _renderer;
    private readonly InputSystem _input;
    private readonly Entity _player;

    private bool _quit;

    public EntityDemoScene(Renderer renderer, InputSystem input)
    {
        _renderer = renderer;
        _input = input;
        _player = CreatePlayer(renderer.Width / 2, renderer.Height / 2);
    }

    public bool IsRunning => !_quit;

    public void Update(double deltaSeconds)
    {
        _input.Update();

        if (_input.WasPressed(GameKey.Cancel))
            _quit = true;

        DirectionComponent direction = _player.Require<DirectionComponent>();
        if (_input.WasPressed(GameKey.Up)) direction.Facing = Direction.Up;
        else if (_input.WasPressed(GameKey.Down)) direction.Facing = Direction.Down;
        else if (_input.WasPressed(GameKey.Left)) direction.Facing = Direction.Left;
        else if (_input.WasPressed(GameKey.Right)) direction.Facing = Direction.Right;
    }

    public void Render()
    {
        _renderer.Clear();
        DrawBorder();

        _renderer.Write(2, 0, " Etapa 2.1 - Entidade ");
        _renderer.Write(2, 1, $" Direcao: {_player.Require<DirectionComponent>().Facing} ");
        _renderer.Write(2, _renderer.Height - 1, " setas: virar | Esc/Q: sair ");

        DrawEntity(_player);

        _renderer.Present();
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
