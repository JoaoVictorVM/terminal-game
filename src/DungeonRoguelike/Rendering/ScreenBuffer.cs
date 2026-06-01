namespace DungeonRoguelike.Rendering;

public sealed class ScreenBuffer
{
    private readonly Glyph[] _cells;

    public ScreenBuffer(int width, int height)
    {
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

        Width = width;
        Height = height;
        _cells = new Glyph[width * height];
        Fill(Glyph.Empty);
    }

    public int Width { get; }

    public int Height { get; }

    public void Fill(Glyph fill) => Array.Fill(_cells, fill);

    // Escritas fora dos limites são ignoradas para que quem desenha não precise
    // validar coordenadas a cada chamada.
    public void Set(int x, int y, Glyph glyph)
    {
        if ((uint)x >= (uint)Width || (uint)y >= (uint)Height)
            return;

        _cells[(y * Width) + x] = glyph;
    }

    public Glyph Get(int x, int y)
    {
        if ((uint)x >= (uint)Width || (uint)y >= (uint)Height)
            return Glyph.Empty;

        return _cells[(y * Width) + x];
    }

    public void Write(int x, int y, string text)
    {
        if (text is null || (uint)y >= (uint)Height)
            return;

        for (int i = 0; i < text.Length; i++)
            Set(x + i, y, text[i]);
    }

    public void CopyFrom(ScreenBuffer source)
    {
        if (source.Width != Width || source.Height != Height)
            throw new ArgumentException("Dimensões incompatíveis para cópia de buffer.", nameof(source));

        Array.Copy(source._cells, _cells, _cells.Length);
    }
}
