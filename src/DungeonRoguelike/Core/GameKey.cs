namespace DungeonRoguelike.Core;

/// <summary>
/// Ações lógicas do jogo, desacopladas das teclas físicas (PRD §15).
/// Mapeamento padrão: setas → direções, Z → ataque/interação, X → defesa,
/// Enter → confirmar, Esc/Q → cancelar.
/// </summary>
public enum GameKey
{
    Up,
    Down,
    Left,
    Right,
    Attack,   // Z
    Defend,   // X
    Confirm,  // Enter
    Cancel,   // Esc / Q
}
