using DungeonRoguelike.Core;

namespace DungeonRoguelike.Infrastructure.Input;

/// <summary>
/// Camada de input do jogo. Lê o estado bruto de uma <see cref="IInputSource"/>
/// uma vez por frame (fase Input do loop, PRD §16.3) e expõe consultas de
/// estado (<see cref="IsDown"/>) e de borda (<see cref="WasPressed"/>,
/// <see cref="WasReleased"/>) — útil para "segurar" (mover/defender) versus
/// "apertar" (atacar/confirmar).
/// </summary>
public sealed class InputSystem
{
    private static readonly GameKey[] AllKeys = Enum.GetValues<GameKey>();

    private readonly IInputSource _source;
    private readonly bool[] _current = new bool[AllKeys.Length];
    private readonly bool[] _previous = new bool[AllKeys.Length];

    public InputSystem(IInputSource source)
    {
        _source = source ?? throw new ArgumentNullException(nameof(source));
    }

    /// <summary>Cria o sistema escolhendo a melhor fonte para a plataforma atual.</summary>
    public static InputSystem Create()
    {
        IInputSource source = OperatingSystem.IsWindows() && !Console.IsInputRedirected
            ? new WindowsRawInputSource()
            : new ConsoleKeyInputSource();

        return new InputSystem(source);
    }

    /// <summary>Atualiza o estado do frame. Chame uma vez no início de cada frame.</summary>
    public void Update()
    {
        _source.Poll();
        for (int i = 0; i < AllKeys.Length; i++)
        {
            _previous[i] = _current[i];
            _current[i] = _source.IsDown(AllKeys[i]);
        }
    }

    /// <summary>A tecla está pressionada neste frame.</summary>
    public bool IsDown(GameKey key) => _current[(int)key];

    /// <summary>A tecla passou de solta para pressionada neste frame (borda de subida).</summary>
    public bool WasPressed(GameKey key) => _current[(int)key] && !_previous[(int)key];

    /// <summary>A tecla passou de pressionada para solta neste frame (borda de descida).</summary>
    public bool WasReleased(GameKey key) => !_current[(int)key] && _previous[(int)key];
}
