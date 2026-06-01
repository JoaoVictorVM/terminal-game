using DungeonRoguelike.Core;

namespace DungeonRoguelike.Infrastructure.Input;

public interface IInputSource
{
    void Poll();

    bool IsDown(GameKey key);
}
