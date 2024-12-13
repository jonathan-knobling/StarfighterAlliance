namespace StarfighterAlliance.Core.Config;

public interface IConfigLoader
{
	void LoadConfig();
	void LoadConfig(string path);
	List<SpaceshipTypeDto> GetSpaceshipTypes();
	float GetObstacleHealthModifier();
}