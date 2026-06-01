using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using DungeonRoguelike.Core;

namespace DungeonRoguelike.Infrastructure.Input;

// GetAsyncKeyState expõe o estado real das teclas, independente da
// auto-repetição do SO, o que permite movimento contínuo suave e "segurar X
// para defender" (PRD §4.2, §7.1). Não exige bomba de mensagens.
[SupportedOSPlatform("windows")]
public sealed class WindowsRawInputSource : IInputSource
{
    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    private const int KeyDownMask = 0x8000;

    private static readonly GameKey[] AllKeys = Enum.GetValues<GameKey>();
    private readonly bool[] _state = new bool[AllKeys.Length];

    public void Poll()
    {
        foreach (GameKey key in AllKeys)
        {
            bool down = false;
            foreach (int vk in VirtualKeysFor(key))
            {
                if ((GetAsyncKeyState(vk) & KeyDownMask) != 0)
                {
                    down = true;
                    break;
                }
            }

            _state[(int)key] = down;
        }
    }

    public bool IsDown(GameKey key) => _state[(int)key];

    private static int[] VirtualKeysFor(GameKey key) => key switch
    {
        GameKey.Up => new[] { 0x26 },
        GameKey.Down => new[] { 0x28 },
        GameKey.Left => new[] { 0x25 },
        GameKey.Right => new[] { 0x27 },
        GameKey.Attack => new[] { 0x5A },
        GameKey.Defend => new[] { 0x58 },
        GameKey.Confirm => new[] { 0x0D },
        GameKey.Cancel => new[] { 0x1B, 0x51 },
        _ => Array.Empty<int>(),
    };
}
