namespace DungeonRoguelike.Rendering;

// Struct com igualdade por valor para viabilizar a comparação de frames (diff)
// no Renderer. O jogo é preto e branco (PRD §4.3), então carrega só o caractere.
public readonly struct Glyph : IEquatable<Glyph>
{
    public static readonly Glyph Empty = new(' ');

    public char Char { get; }

    public Glyph(char @char) => Char = @char;

    public bool Equals(Glyph other) => Char == other.Char;

    public override bool Equals(object? obj) => obj is Glyph g && Equals(g);

    public override int GetHashCode() => Char.GetHashCode();

    public static bool operator ==(Glyph a, Glyph b) => a.Equals(b);

    public static bool operator !=(Glyph a, Glyph b) => !a.Equals(b);

    public static implicit operator Glyph(char c) => new(c);
}
