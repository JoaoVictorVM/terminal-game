using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace DungeonRoguelike.Infrastructure.Input;

/// <summary>
/// Ajustes do modo do console no Windows para jogos em tempo real.
///
/// Desabilita o QuickEdit: por padrão, clicar na janela do console entra em
/// modo de seleção e <b>pausa</b> a aplicação até o usuário pressionar Enter —
/// inaceitável num loop de tempo real. Em outras plataformas é um no-op.
/// </summary>
public static class WindowsConsoleMode
{
    private const int StdInputHandle = -10;
    private const uint EnableQuickEditMode = 0x0040;
    private const uint EnableExtendedFlags = 0x0080;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetConsoleMode(nint hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleMode(nint hConsoleHandle, uint dwMode);

    /// <summary>Aplica os ajustes; silenciosamente ignorado fora do Windows.</summary>
    public static void Configure()
    {
        if (OperatingSystem.IsWindows())
            DisableQuickEdit();
    }

    [SupportedOSPlatform("windows")]
    private static void DisableQuickEdit()
    {
        nint handle = GetStdHandle(StdInputHandle);
        if (handle == nint.Zero || handle == -1)
            return;

        if (!GetConsoleMode(handle, out uint mode))
            return;

        // ENABLE_EXTENDED_FLAGS é obrigatório para que a alteração do QuickEdit valha.
        uint newMode = (mode | EnableExtendedFlags) & ~EnableQuickEditMode;
        SetConsoleMode(handle, newMode);
    }
}
