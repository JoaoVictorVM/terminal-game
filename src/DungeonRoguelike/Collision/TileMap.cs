namespace DungeonRoguelike.Collision;

public sealed class TileMap : ICollisionMap
{
    private readonly bool[] _solid;

    public TileMap(int width, int height)
    {
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

        Width = width;
        Height = height;
        _solid = new bool[width * height];
    }

    public int Width { get; }

    public int Height { get; }

    public void SetSolid(int tileX, int tileY, bool solid)
    {
        if ((uint)tileX >= (uint)Width || (uint)tileY >= (uint)Height)
            return;

        _solid[(tileY * Width) + tileX] = solid;
    }

    // Fora dos limites é sólido para que o jogador nunca escape do mapa.
    public bool IsSolid(int tileX, int tileY)
    {
        if ((uint)tileX >= (uint)Width || (uint)tileY >= (uint)Height)
            return true;

        return _solid[(tileY * Width) + tileX];
    }
}
