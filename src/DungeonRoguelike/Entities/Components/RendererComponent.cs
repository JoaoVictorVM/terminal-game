using DungeonRoguelike.Core;

namespace DungeonRoguelike.Entities.Components;

// Sprite por direção (PRD §16.6). Mantém apenas o caractere para não acoplar a
// camada de entidades à de renderização; a conversão para Glyph fica na camada
// de apresentação.
public sealed class RendererComponent : Component
{
    private readonly char _up;
    private readonly char _down;
    private readonly char _left;
    private readonly char _right;

    public RendererComponent(char sprite)
        : this(sprite, sprite, sprite, sprite)
    {
    }

    public RendererComponent(char up, char down, char left, char right)
    {
        _up = up;
        _down = down;
        _left = left;
        _right = right;
    }

    public char SpriteFor(Direction direction) => direction switch
    {
        Direction.Up => _up,
        Direction.Down => _down,
        Direction.Left => _left,
        Direction.Right => _right,
        _ => _down,
    };
}
