namespace StarfighterAlliance.Core.Spaceship.Color;

public interface ISpaceshipColorService
{
	Task<Godot.Color> GetColor();
}