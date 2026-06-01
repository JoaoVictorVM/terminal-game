namespace DungeonRoguelike.Core;

/// <summary>
/// Contrato mínimo para algo que participa do game loop em tempo real.
/// Mantém a separação exigida pelo PRD (§16.2): <see cref="Update"/> contém
/// apenas lógica; <see cref="Render"/> contém apenas apresentação.
/// </summary>
public interface IGameScene
{
    /// <summary>Indica se o loop deve continuar executando.</summary>
    bool IsRunning { get; }

    /// <summary>Avança a lógica do jogo em <paramref name="deltaSeconds"/> segundos.</summary>
    void Update(double deltaSeconds);

    /// <summary>Desenha o estado atual. Não deve conter lógica de gameplay.</summary>
    void Render();
}
