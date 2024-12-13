using Godot;

namespace StarfighterAlliance.Core.Config;

public class ConfigService : IConfigService
{
	private readonly IConfigLoader configLoader;

	public ConfigService(IConfigLoader configLoader)
	{
		this.configLoader = configLoader;
	}

	public SpaceshipTypeDto? SelectedSpaceshipType { get; set; }
	public Color LoadedColor { get; set; }

	public void LoadConfig()
	{
		configLoader.LoadConfig();
	}

	public void LoadConfig(string path)
	{
		configLoader.LoadConfig(path);
	}

	public List<SpaceshipTypeDto> GetSpaceshipTypes()
	{
		return configLoader.GetSpaceshipTypes();
	}

	public float GetObstacleHealthModifier()
	{
		return configLoader.GetObstacleHealthModifier();
	}
}