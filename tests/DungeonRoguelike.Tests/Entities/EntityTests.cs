using DungeonRoguelike.Core;
using DungeonRoguelike.Entities;
using DungeonRoguelike.Entities.Components;
using Xunit;

namespace DungeonRoguelike.Tests.Entities;

public class EntityTests
{
    [Fact]
    public void Get_ReturnsComponent_AfterAdd()
    {
        var entity = new Entity().Add(new PositionComponent(3f, 4f));

        PositionComponent? position = entity.Get<PositionComponent>();

        Assert.NotNull(position);
        Assert.Equal(3f, position!.X);
        Assert.Equal(4f, position.Y);
    }

    [Fact]
    public void Get_ReturnsNull_WhenComponentMissing()
    {
        var entity = new Entity();

        Assert.Null(entity.Get<DirectionComponent>());
        Assert.False(entity.Has<DirectionComponent>());
    }

    [Fact]
    public void Add_ReplacesComponentOfSameType()
    {
        var entity = new Entity()
            .Add(new PositionComponent(1f, 1f))
            .Add(new PositionComponent(9f, 9f));

        Assert.Equal(9f, entity.Require<PositionComponent>().X);
    }

    [Fact]
    public void Require_Throws_WhenComponentMissing()
    {
        var entity = new Entity();

        Assert.Throws<InvalidOperationException>(() => entity.Require<PositionComponent>());
    }

    [Theory]
    [InlineData(Direction.Up, '^')]
    [InlineData(Direction.Down, 'v')]
    [InlineData(Direction.Left, '<')]
    [InlineData(Direction.Right, '>')]
    public void RendererComponent_ReturnsSpritePerDirection(Direction facing, char expected)
    {
        var sprite = new RendererComponent(up: '^', down: 'v', left: '<', right: '>');

        Assert.Equal(expected, sprite.SpriteFor(facing));
    }

    [Fact]
    public void PositionComponent_TileCoordinates_TruncateTowardZero()
    {
        var position = new PositionComponent(10.7f, 7.2f);

        Assert.Equal(10, position.TileX);
        Assert.Equal(7, position.TileY);
    }
}
