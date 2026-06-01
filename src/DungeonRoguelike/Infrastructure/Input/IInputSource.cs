using DungeonRoguelike.Core;

namespace DungeonRoguelike.Infrastructure.Input;

/// <summary>
/// Fonte de baixo nível do estado das teclas. Abstrai a forma como o estado
/// "pressionada agora" é obtido em cada plataforma (Windows usa o estado real
/// via GetAsyncKeyState; o fallback infere por eventos de teclado).
/// </summary>
public interface IInputSource
{
    /// <summary>Captura um instantâneo do estado das teclas para este frame.</summary>
    void Poll();

    /// <summary>Indica se a tecla está pressionada no instantâneo atual.</summary>
    bool IsDown(GameKey key);
}
