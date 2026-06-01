using DungeonRoguelike.Game;

namespace DungeonRoguelike;

/// <summary>
/// Ponto de entrada / composition root da aplicação.
/// Etapa 1.2: monta o game loop com FPS fixo e roda uma cena de diagnóstico.
/// </summary>
internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        if (!Console.IsOutputRedirected)
            Console.CursorVisible = false;

        var loop = new GameLoop(targetFps: 60);
        var scene = new DiagnosticsScene(loop.Clock);

        try
        {
            loop.Run(scene);
        }
        finally
        {
            if (!Console.IsOutputRedirected)
            {
                Console.CursorVisible = true;
                Console.SetCursorPosition(0, 7);
            }
            Console.WriteLine("Loop encerrado.");
        }
    }
}
