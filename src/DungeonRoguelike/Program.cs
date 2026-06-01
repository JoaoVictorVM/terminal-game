using DungeonRoguelike.Game;
using DungeonRoguelike.Infrastructure.Input;
using DungeonRoguelike.Rendering;

namespace DungeonRoguelike;

/// <summary>
/// Ponto de entrada / composition root da aplicação.
/// Etapa 1.3: monta loop + renderer com double buffer e roda a cena de demo.
/// </summary>
internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        if (!Console.IsOutputRedirected)
            Console.CursorVisible = false;
        WindowsConsoleMode.Configure(); // evita pausa ao clicar (QuickEdit)

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

    /// <summary>
    /// Tamanho da viewport. Reserva a última linha para evitar o scroll
    /// automático ao escrever no canto inferior-direito. Usa um padrão seguro
    /// quando não há console real (saída redirecionada/testes).
    /// </summary>
    private static (int Width, int Height) ResolveViewportSize()
    {
        if (Console.IsOutputRedirected)
            return (80, 25);

        int width = Math.Max(20, Console.WindowWidth);
        int height = Math.Max(10, Console.WindowHeight - 1);
        return (width, height);
    }
}
