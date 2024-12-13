namespace StarfighterAlliance.Core.Waves;

/// <summary>
///     Defines a factory interface for generating waves of obstacles based on difficulty.
/// </summary>
public interface IWaveFactory
{
	/// <summary>
	///     Generates a sequence of obstacle types for a wave based on the current difficulty level.
	/// </summary>
	/// <param name="currentDifficulty">
	///     The current difficulty level, represented as a float. Higher values indicate greater difficulty.
	///     The difficulty has to be in a range between 0 to 1 to be valid. Otherwise the value gets clamped to the nearest
	///     valid value.
	/// </param>
	/// <returns>
	///     An enumerable collection of <see cref="ObstacleType" /> representing the obstacles in the wave.
	/// </returns>
	IEnumerable<ObstacleType> GenerateWave(float currentDifficulty);
}