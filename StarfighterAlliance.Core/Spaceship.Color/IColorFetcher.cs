namespace StarfighterAlliance.Core.Spaceship.Color;

public interface IColorFetcher
{
	Task<string> FetchColorAsync(CancellationToken cancellationToken = default);
}