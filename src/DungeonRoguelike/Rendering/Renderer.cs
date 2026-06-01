using System.Text;

namespace DungeonRoguelike.Rendering;

// Double buffer com renderização parcial (PRD §16.4): Present compara back vs
// front e escreve no console apenas as células alteradas, agrupando células
// contíguas de uma mesma linha numa única escrita. Nunca usa Console.Clear().
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

    public int LastDirtyCells { get; private set; }

    public int LastDirtyRuns { get; private set; }

    public void Clear() => _back.Fill(Glyph.Empty);

    public void Set(int x, int y, Glyph glyph) => _back.Set(x, y, glyph);

    public void Write(int x, int y, string text) => _back.Write(x, y, text);

    public void Invalidate() => _forceFullRedraw = true;

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

        // Sem console real (saída redirecionada) não há cursor; emite os contadores
        // do diff, o que também serve de prova da renderização parcial em testes.
        if (redirected)
            Console.WriteLine($"present dirtyCells={dirtyCells} runs={dirtyRuns}");
    }
}
