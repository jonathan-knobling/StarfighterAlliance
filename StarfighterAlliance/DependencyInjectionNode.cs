using Godot.DependencyInjection;

namespace StarfighterAlliance;

/// <summary>
///     Autoload Singleton script that is responsible for injecting all Dependencies, that are registered in the Main
///     Script into the Scene tree.
/// </summary>
public partial class DependencyInjectionNode : DependencyInjectionManagerNode
{
}