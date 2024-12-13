namespace StarfighterAlliance.Core.Stats;

public class GameResult : IEquatable<GameResult>
{
	public Guid Id { get; init; }
	public required int FinalScore { get; init; }
	public required int WaveCount { get; init; }
	public required string UsedSpaceship { get; init; }
	public required DateTime GameEndedAt { get; init; }

	public bool Equals(GameResult? other)
	{
		if (ReferenceEquals(null, other))
		{
			return false;
		}

		if (ReferenceEquals(this, other))
		{
			return true;
		}

		return Id.Equals(other.Id);
	}


	public static bool operator ==(GameResult result1, GameResult result2)
	{
		return result1.Id == result2.Id;
	}

	public static bool operator !=(GameResult result1, GameResult result2)
	{
		return result1.Id != result2.Id;
	}

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(null, obj))
		{
			return false;
		}

		if (ReferenceEquals(this, obj))
		{
			return true;
		}

		if (obj.GetType() != this.GetType())
		{
			return false;
		}

		return Equals((GameResult)obj);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Id, FinalScore, WaveCount, UsedSpaceship, GameEndedAt);
	}
}