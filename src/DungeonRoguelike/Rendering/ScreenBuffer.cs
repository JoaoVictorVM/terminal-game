namespace DungeonRoguelike.Rendering;

/// <summary>
/// Grade 2D de <see cref="Glyph"/> em armazenamento linear. É um buffer de
/// dados puro, sem qualquer conhecimento do <see cref="Console"/> — toda a
/// escrita no terminal fica no <see cref="Renderer"/> (PRD §16.2).
///
/// Escritas fora dos limites são silenciosamente ignoradas (clipping), de modo
/// que quem desenha não precisa validar coordenadas manualmente.
/// </summary>
public sealed class ScreenBuffer
{
    private Glyph[] _cells;

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

    /// <summary>Preenche todas as células com <paramref name="fill"/>.</summary>
    public void Fill(Glyph fill) => Array.Fill(_cells, fill);

    /// <summary>Define a célula em (x, y); ignora coordenadas fora dos limites.</summary>
    public void Set(int x, int y, Glyph glyph)
    {
        if ((uint)x >= (uint)Width || (uint)y >= (uint)Height)
            return;

        _cells[(y * Width) + x] = glyph;
    }

    /// <summary>Lê a célula em (x, y). Coordenadas fora dos limites retornam <see cref="Glyph.Empty"/>.</summary>
    public Glyph Get(int x, int y)
    {
        if ((uint)x >= (uint)Width || (uint)y >= (uint)Height)
            return Glyph.Empty;

        return _cells[(y * Width) + x];
    }

    /// <summary>Escreve um texto horizontalmente a partir de (x, y), com clipping.</summary>
    public void Write(int x, int y, string text)
    {
        if (text is null || (uint)y >= (uint)Height)
            return;

        for (int i = 0; i < text.Length; i++)
            Set(x + i, y, text[i]);
    }

    /// <summary>Copia todo o conteúdo de <paramref name="source"/> (mesmas dimensões).</summary>
    public void CopyFrom(ScreenBuffer source)
    {
        if (source.Width != Width || source.Height != Height)
            throw new ArgumentException("Dimensões incompatíveis para cópia de buffer.", nameof(source));

        Array.Copy(source._cells, _cells, _cells.Length);
    }
}
