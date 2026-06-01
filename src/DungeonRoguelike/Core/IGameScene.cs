namespace DungeonRoguelike.Core;

public interface IGameScene
{
    bool IsRunning { get; }

    void Update(double deltaSeconds);

    void Render();
}
