using DungeonRoguelike.Collision;
using DungeonRoguelike.Core;
using DungeonRoguelike.Entities;
using DungeonRoguelike.Entities.Components;
using DungeonRoguelike.Entities.Systems;
using DungeonRoguelike.Infrastructure.Input;
using DungeonRoguelike.Rendering;

namespace DungeonRoguelike.Game;

// Cena temporária da Etapa 2.3: prova colisão com paredes. O mapa fixo aqui é
// um stand-in; salas reais (Room) chegam na Fase 3.
public sealed class CollisionDemoScene : IGameScene
{
    private const char WallSprite = '#';

    private readonly Renderer _renderer;
    private readonly InputSystem _input;
    private readonly PlayerController _controller;
    private readonly TileMap _map;
    private readonly Entity _player;

    private bool _quit;

    public CollisionDemoScene(Renderer renderer, InputSystem input)
    {
        _renderer = renderer;
        _input = input;
        _controller = new PlayerController(input, new CollisionManager());
        _map = BuildMap(renderer.Width, renderer.Height);
        _player = CreatePlayer(renderer.Width / 2, renderer.Height / 4 + 1);
    }

    public bool IsRunning => !_quit;

    public void Update(double deltaSeconds)
    {
        _input.Update();

        if (_input.WasPressed(GameKey.Cancel))
            _quit = true;

        _controller.Update(_player, deltaSeconds, _map);
    }

    public void Render()
    {
        _renderer.Clear();
        DrawMap();

        PositionComponent position = _player.Require<PositionComponent>();
        Direction facing = _player.Require<DirectionComponent>().Facing;

        _renderer.Write(2, 0, " Etapa 2.3 - Colisao ");
        _renderer.Write(2, 1, $" Pos: {position.X,5:0.0},{position.Y,5:0.0}  Dir: {facing} ");

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

        // Parede horizontal com uma passagem, para testar deslizar e bloquear.
        int wallY = height / 2;
        int gap = width / 2;
        for (int x = 3; x < width - 3; x++)
            if (x != gap && x != gap + 1)
                map.SetSolid(x, wallY, true);

        // Alguns pilares isolados.
        map.SetSolid(width / 4, height / 4, true);
        map.SetSolid(3 * width / 4, 3 * height / 4, true);

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
