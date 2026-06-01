using DungeonRoguelike.Game;
using DungeonRoguelike.Infrastructure.Input;
using DungeonRoguelike.Rendering;

namespace DungeonRoguelike;

internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        if (!Console.IsOutputRedirected)
            Console.CursorVisible = false;
        WindowsConsoleMode.Configure();

        (int width, int height) = ResolveViewportSize();

        var loop = new GameLoop(targetFps: 60);
        var renderer = new Renderer(width, height);
        var input = InputSystem.Create();
        var scene = new InputDemoScene(renderer, input, loop.Clock);

        try
        {
            loop.Run(scene);
        }
        finally
        {
            if (!Console.IsOutputRedirected)
            {
                Console.CursorVisible = true;
                Console.SetCursorPosition(0, Math.Min(height, Console.WindowHeight - 1));
            }
            Console.WriteLine("\nLoop encerrado.");
        }
    }

    // Reserva a última linha para evitar o scroll automático ao escrever no
    // canto inferior-direito. Sem console real, usa um padrão fixo.
    private static (int Width, int Height) ResolveViewportSize()
    {
        if (Console.IsOutputRedirected)
            return (80, 25);

        int width = Math.Max(20, Console.WindowWidth);
        int height = Math.Max(10, Console.WindowHeight - 1);
        return (width, height);
    }
}
