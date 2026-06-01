using DungeonRoguelike.Core;
using DungeonRoguelike.Entities.Components;
using DungeonRoguelike.Infrastructure.Input;

namespace DungeonRoguelike.Entities.Systems;

public sealed class PlayerController
{
    private const float MoveSpeed = 14f; // tiles por segundo
    private const float DiagonalScale = 0.70710678f; // 1/raiz(2): diagonal não anda mais rápido

    private readonly InputSystem _input;

    public PlayerController(InputSystem input)
    {
        _input = input ?? throw new ArgumentNullException(nameof(input));
    }

    public void Update(Entity player, double deltaSeconds)
    {
        UpdateFacing(player.Require<DirectionComponent>());
        UpdatePosition(player.Require<PositionComponent>(), deltaSeconds);
    }

    // A direção é a última seta pressionada (PRD §4.2): atualiza só na borda de
    // subida, então segurar uma seta não sobrescreve uma virada mais recente.
    private void UpdateFacing(DirectionComponent direction)
    {
        if (_input.WasPressed(GameKey.Up)) direction.Facing = Direction.Up;
        if (_input.WasPressed(GameKey.Down)) direction.Facing = Direction.Down;
        if (_input.WasPressed(GameKey.Left)) direction.Facing = Direction.Left;
        if (_input.WasPressed(GameKey.Right)) direction.Facing = Direction.Right;
    }

    private void UpdatePosition(PositionComponent position, double deltaSeconds)
    {
        float dx = 0f;
        float dy = 0f;
        if (_input.IsDown(GameKey.Left)) dx -= 1f;
        if (_input.IsDown(GameKey.Right)) dx += 1f;
        if (_input.IsDown(GameKey.Up)) dy -= 1f;
        if (_input.IsDown(GameKey.Down)) dy += 1f;

        if (dx != 0f && dy != 0f)
        {
            dx *= DiagonalScale;
            dy *= DiagonalScale;
        }

        float step = MoveSpeed * (float)deltaSeconds;
        position.X += dx * step;
        position.Y += dy * step;
    }
}
