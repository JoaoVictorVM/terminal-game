using DungeonRoguelike.Core;

namespace DungeonRoguelike.Infrastructure.Input;

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

    public static InputSystem Create()
    {
        IInputSource source = OperatingSystem.IsWindows() && !Console.IsInputRedirected
            ? new WindowsRawInputSource()
            : new ConsoleKeyInputSource();

        return new InputSystem(source);
    }

    public void Update()
    {
        _source.Poll();
        for (int i = 0; i < AllKeys.Length; i++)
        {
            _previous[i] = _current[i];
            _current[i] = _source.IsDown(AllKeys[i]);
        }
    }

    public bool IsDown(GameKey key) => _current[(int)key];

    public bool WasPressed(GameKey key) => _current[(int)key] && !_previous[(int)key];

    public bool WasReleased(GameKey key) => !_current[(int)key] && _previous[(int)key];
}
