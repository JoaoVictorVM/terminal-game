using DungeonRoguelike.Collision;
using Xunit;

namespace DungeonRoguelike.Tests.Collision;

public class CollisionManagerTests
{
    private static TileMap OpenMap(int width = 10, int height = 10)
    {
        // Sem paredes internas; só os limites externos são sólidos (via IsSolid).
        return new TileMap(width, height);
    }

    [Fact]
    public void ResolveMove_AllowsMovementIntoFloor()
    {
        var map = OpenMap();
        var collision = new CollisionManager();

        (float x, float y) = collision.ResolveMove(map, 5f, 5f, 0.5f, 0f);

        Assert.Equal(5.5f, x);
        Assert.Equal(5f, y);
    }

    [Fact]
    public void ResolveMove_BlocksMovementIntoSolidTile()
    {
        var map = OpenMap();
        map.SetSolid(6, 5, true);
        var collision = new CollisionManager();

        (float x, float y) = collision.ResolveMove(map, 5.5f, 5f, 1f, 0f); // destino entraria no tile 6

        Assert.Equal(5.5f, x); // X bloqueado
        Assert.Equal(5f, y);
    }

    [Fact]
    public void ResolveMove_SlidesAlongWall_WhenOnlyOneAxisBlocked()
    {
        var map = OpenMap();
        map.SetSolid(6, 5, true); // parede à direita
        var collision = new CollisionManager();

        (float x, float y) = collision.ResolveMove(map, 5.5f, 5f, 1f, 1f); // diagonal contra a parede

        Assert.Equal(5.5f, x); // X bloqueado pela parede
        Assert.Equal(6f, y);   // Y livre: desliza para baixo
    }

    [Fact]
    public void ResolveMove_TreatsOutOfBoundsAsSolid()
    {
        var map = OpenMap(width: 10, height: 10);
        var collision = new CollisionManager();

        (float x, float y) = collision.ResolveMove(map, 0.5f, 5f, -1f, 0f); // tentaria ir para tile -1

        Assert.Equal(0.5f, x); // bloqueado pelo limite
        Assert.Equal(5f, y);
    }

    [Fact]
    public void ResolveMove_BlocksVerticalIntoSolidTileBelow()
    {
        var map = OpenMap();
        map.SetSolid(5, 6, true); // parede abaixo
        var collision = new CollisionManager();

        (float x, float y) = collision.ResolveMove(map, 5f, 5.5f, 0f, 1f);

        Assert.Equal(5f, x);
        Assert.Equal(5.5f, y); // Y bloqueado pela parede abaixo
    }
}
