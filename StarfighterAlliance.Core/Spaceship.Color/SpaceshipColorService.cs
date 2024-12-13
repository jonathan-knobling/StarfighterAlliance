using Godot;

namespace StarfighterAlliance.Core.Spaceship.Color;

public class SpaceshipColorService : ISpaceshipColorService
{
	private readonly IColorFetcher colorFetcher;
	private readonly Godot.Color defaultColor = Colors.RoyalBlue;

	public SpaceshipColorService(IColorFetcher colorFetcher)
	{
		this.colorFetcher = colorFetcher;
	}

	public async Task<Godot.Color> GetColor()
	{
		string colorName = await colorFetcher.FetchColorAsync();

		return Godot.Color.FromString(colorName, defaultColor);
	}
}