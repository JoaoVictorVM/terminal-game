using DungeonRoguelike.Entities.Components;

namespace DungeonRoguelike.Entities;

// ECS-lite (PRD §16.5): a entidade é apenas um contêiner de componentes
// indexados por tipo; toda a lógica vive nos sistemas que operam sobre eles.
public sealed class Entity
{
    private readonly Dictionary<Type, Component> _components = new();

    public Entity Add(Component component)
    {
        ArgumentNullException.ThrowIfNull(component);
        _components[component.GetType()] = component;
        return this;
    }

    public T? Get<T>() where T : Component
        => _components.TryGetValue(typeof(T), out Component? component) ? (T)component : null;

    public bool Has<T>() where T : Component => _components.ContainsKey(typeof(T));

    public T Require<T>() where T : Component
        => Get<T>() ?? throw new InvalidOperationException($"Entidade não possui o componente {typeof(T).Name}.");
}
