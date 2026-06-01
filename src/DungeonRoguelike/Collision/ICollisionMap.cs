namespace DungeonRoguelike.Collision;

public interface ICollisionMap
{
    bool IsSolid(int tileX, int tileY);
}
