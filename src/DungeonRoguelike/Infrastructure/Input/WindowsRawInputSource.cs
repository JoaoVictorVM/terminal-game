using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using DungeonRoguelike.Core;

namespace DungeonRoguelike.Infrastructure.Input;

/// <summary>
/// Fonte de input para Windows usando <c>GetAsyncKeyState</c> (user32), que
/// expõe o estado real das teclas independentemente da auto-repetição do SO.
/// É o que permite movimento contínuo suave e "segurar X para defender"
/// (PRD §4.2, §7.1). Não exige bomba de mensagens.
/// </summary>
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

    /// <summary>Códigos de tecla virtual (VK) associados a cada ação.</summary>
    private static int[] VirtualKeysFor(GameKey key) => key switch
    {
        GameKey.Up => new[] { 0x26 }, // VK_UP
        GameKey.Down => new[] { 0x28 }, // VK_DOWN
        GameKey.Left => new[] { 0x25 }, // VK_LEFT
        GameKey.Right => new[] { 0x27 }, // VK_RIGHT
        GameKey.Attack => new[] { 0x5A }, // 'Z'
        GameKey.Defend => new[] { 0x58 }, // 'X'
        GameKey.Confirm => new[] { 0x0D }, // VK_RETURN
        GameKey.Cancel => new[] { 0x1B, 0x51 }, // VK_ESCAPE, 'Q'
        _ => Array.Empty<int>(),
    };
}
