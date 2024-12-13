namespace StarfighterAlliance.Core.Waves;

/// <summary>
///     Represents different types of obstacles in a wave.
/// </summary>
public enum ObstacleType
{
	/// <summary>
	///     Represents a Gap in a wave with a width of 1 Unit
	/// </summary>
	None,

	/// <summary>
	///     Represents a small obstacle with a width of 2 Units
	/// </summary>
	Small,

	/// <summary>
	///     Represents a regular obstacle with a width of 2 Units
	/// </summary>
	Regular,

	/// <summary>
	///     Represents a big obstacle with a width of 3 Units
	/// </summary>
	Big
}

public static class ObstacleTypeExtensions
{
	public static int GetWidth(this ObstacleType type)
	{
		return type switch
		{
			ObstacleType.None    => 1,
			ObstacleType.Small   => 2,
			ObstacleType.Regular => 2,
			ObstacleType.Big     => 3,
			_                    => 1
		};
	}
}