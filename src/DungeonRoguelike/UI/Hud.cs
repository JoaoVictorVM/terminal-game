using System.Text;
using DungeonRoguelike.Rendering;

namespace DungeonRoguelike.UI;

// HUD in-game (PRD §14): exibe apenas a vida em corações. Gold nunca aparece
// durante o gameplay, somente no menu principal.
public sealed class Hud
{
    private const char FullHeart = '♥'; // ♥
    private const char EmptyHeart = '♡'; // ♡

    private readonly StringBuilder _builder = new();

    public void Draw(Renderer renderer, int currentHearts, int maxHearts, int x = 2, int y = 0)
    {
        _builder.Clear();
        for (int i = 0; i < maxHearts; i++)
        {
            _builder.Append(i < currentHearts ? FullHeart : EmptyHeart);
            if (i < maxHearts - 1)
                _builder.Append(' ');
        }

        renderer.Write(x, y, _builder.ToString());
    }
}
