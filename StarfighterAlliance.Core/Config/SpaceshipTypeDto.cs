namespace StarfighterAlliance.Core.Config;

public class SpaceshipTypeDto
{
	public required string Name { get; init; }
	public required string SerializedName { get; set; }
	public required int Health { get; init; }
	public required int AttackDamage { get; init; }
	public required int Speed { get; init; }
}