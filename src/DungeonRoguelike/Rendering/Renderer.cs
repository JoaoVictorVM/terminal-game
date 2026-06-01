using System.Text;

namespace DungeonRoguelike.Rendering;

/// <summary>
/// Renderer com double buffer e renderização parcial (PRD §16.4).
///
/// Fluxo por frame: <see cref="Clear"/> → desenhar no back buffer (Set/Write)
/// → <see cref="Present"/>. O <see cref="Present"/> compara back vs front e
/// escreve no console apenas as células que mudaram, agrupando células
/// contíguas de uma mesma linha em uma única chamada de escrita. Nunca usa
/// <c>Console.Clear()</c>.
/// </summary>
public sealed class Renderer
{
    private readonly ScreenBuffer _back;
    private readonly ScreenBuffer _front;
    private readonly StringBuilder _run = new();
    private bool _forceFullRedraw = true;

    public Renderer(int width, int height)
    {
        _back = new ScreenBuffer(width, height);
        _front = new ScreenBuffer(width, height);
    }

    public int Width => _back.Width;

    public int Height => _back.Height;

    /// <summary>Células alteradas no último <see cref="Present"/> (diagnóstico/testes).</summary>
    public int LastDirtyCells { get; private set; }

    /// <summary>Runs (blocos contíguos) escritos no último <see cref="Present"/>.</summary>
    public int LastDirtyRuns { get; private set; }

    /// <summary>Limpa o back buffer para espaços. Início de cada frame.</summary>
    public void Clear() => _back.Fill(Glyph.Empty);

    /// <summary>Desenha uma célula no back buffer.</summary>
    public void Set(int x, int y, Glyph glyph) => _back.Set(x, y, glyph);

    /// <summary>Desenha um texto horizontal no back buffer.</summary>
    public void Write(int x, int y, string text) => _back.Write(x, y, text);

    /// <summary>Força a próxima apresentação a redesenhar a tela inteira (ex.: após resize).</summary>
    public void Invalidate() => _forceFullRedraw = true;

    /// <summary>
    /// Compara back e front e aplica apenas as diferenças no console; depois
    /// promove o back para front.
    /// </summary>
    public void Present()
    {
        bool redirected = Console.IsOutputRedirected;
        int dirtyCells = 0;
        int dirtyRuns = 0;

        for (int y = 0; y < Height; y++)
        {
            int x = 0;
            while (x < Width)
            {
                if (!_forceFullRedraw && _back.Get(x, y) == _front.Get(x, y))
                {
                    x++;
                    continue;
                }

                // Acumula um run de células diferentes (ou tudo, se redraw total).
                int runStart = x;
                _run.Clear();
                while (x < Width && (_forceFullRedraw || _back.Get(x, y) != _front.Get(x, y)))
                {
                    _run.Append(_back.Get(x, y).Char);
                    dirtyCells++;
                    x++;
                }

                dirtyRuns++;
                if (!redirected)
                {
                    Console.SetCursorPosition(runStart, y);
                    Console.Write(_run.ToString());
                }
            }
        }

        _front.CopyFrom(_back);
        _forceFullRedraw = false;
        LastDirtyCells = dirtyCells;
        LastDirtyRuns = dirtyRuns;

        // Modo headless (saída redirecionada): comprova a renderização parcial.
        if (redirected)
            Console.WriteLine($"present dirtyCells={dirtyCells} runs={dirtyRuns}");
    }
}
