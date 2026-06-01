using DungeonRoguelike.Core;

namespace DungeonRoguelike.Entities.Components;

public sealed class DirectionComponent : Component
{
    public DirectionComponent(Direction facing = Direction.Down)
    {
        Facing = facing;
    }

    public Direction Facing { get; set; }
}
