namespace DungeonRoguelike.Collision;

public sealed class CollisionManager
{
    // Resolve o movimento por eixo (X depois Y) para permitir deslizar ao longo
    // das paredes: se um eixo esbarra num tile sólido, só ele é bloqueado.
    // Assume passos menores que 1 tile por frame (sem tunneling nas velocidades
    // do jogo); checa apenas o tile de destino.
    public (float X, float Y) ResolveMove(ICollisionMap map, float x, float y, float deltaX, float deltaY)
    {
        ArgumentNullException.ThrowIfNull(map);

        float resolvedX = x;
        float resolvedY = y;

        float candidateX = x + deltaX;
        if (!map.IsSolid(ToTile(candidateX), ToTile(resolvedY)))
            resolvedX = candidateX;

        float candidateY = y + deltaY;
        if (!map.IsSolid(ToTile(resolvedX), ToTile(candidateY)))
            resolvedY = candidateY;

        return (resolvedX, resolvedY);
    }

    // floor (não truncamento): (int)(-0.5) seria 0, mapeando posições negativas
    // para o tile 0 por engano. floor(-0.5) = -1 mantém o índice correto.
    private static int ToTile(float coordinate) => (int)MathF.Floor(coordinate);
}
