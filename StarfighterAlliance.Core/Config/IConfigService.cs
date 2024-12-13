using Godot;

namespace StarfighterAlliance.Core.Config;

public interface IConfigService : IConfigLoader
{
	public SpaceshipTypeDto? SelectedSpaceshipType { get; set; }
	public Color LoadedColor { get; set; }
}