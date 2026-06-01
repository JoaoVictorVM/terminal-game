namespace DungeonRoguelike.Entities.Components;

// Posição em ponto flutuante para movimento suave entre tiles (PRD §4.2).
public sealed class PositionComponent : Component
{
    public PositionComponent(float x = 0f, float y = 0f)
    {
        X = x;
        Y = y;
    }

    public float X { get; set; }

    public float Y { get; set; }

    public int TileX => (int)X;

    public int TileY => (int)Y;
}
