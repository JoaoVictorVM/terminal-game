namespace DungeonRoguelike.Rendering;

/// <summary>
/// Uma célula renderizável do terminal. O jogo é preto e branco (PRD §4.3),
/// então por ora a célula carrega apenas o caractere. É um <c>struct</c> com
/// igualdade por valor para permitir a comparação de frames (frame diff).
/// </summary>
public readonly struct Glyph : IEquatable<Glyph>
{
    /// <summary>Célula vazia (espaço). Usada para limpar o buffer.</summary>
    public static readonly Glyph Empty = new(' ');

    public char Char { get; }

    public Glyph(char @char) => Char = @char;

    public bool Equals(Glyph other) => Char == other.Char;

    public override bool Equals(object? obj) => obj is Glyph g && Equals(g);

    public override int GetHashCode() => Char.GetHashCode();

    public static bool operator ==(Glyph a, Glyph b) => a.Equals(b);

    public static bool operator !=(Glyph a, Glyph b) => !a.Equals(b);

    /// <summary>Conversão implícita de <see cref="char"/> para escrita ergonômica.</summary>
    public static implicit operator Glyph(char c) => new(c);
}
