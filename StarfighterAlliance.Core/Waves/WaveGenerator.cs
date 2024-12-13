namespace StarfighterAlliance.Core.Waves;

public class WaveGenerator : IWaveFactory
{
	private readonly Random random;

	public WaveGenerator(Random random)
	{
		this.random = random;
	}

	/// <inheritdoc cref="IWaveFactory.GenerateWave" />
	public IEnumerable<ObstacleType> GenerateWave(float currentDifficulty)
	{
		currentDifficulty = Math.Clamp(currentDifficulty, 0f, 1f);

		const float spawnGapBaseProbability = 0.4f;
		const int waveTotalWidth = 20;
		var wave = new List<ObstacleType>();
		var currentOffset = 0;

		int requiredGapPosition = random.Next(waveTotalWidth - 3);

		while (currentOffset < waveTotalWidth)
		{
			if (requiredGapPosition - currentOffset <= 2 && requiredGapPosition - currentOffset >= 0)
			{
				int requiredPaddingToGap = requiredGapPosition - currentOffset;

				if (requiredPaddingToGap == 1)
				{
					yield return ObstacleType.None;
				}
				else if (requiredPaddingToGap == 2)
				{
					yield return currentDifficulty > 0.6f ? ObstacleType.Regular : ObstacleType.Small;
				}

				currentOffset += requiredPaddingToGap;

				foreach (ObstacleType type in Enumerable.Repeat(ObstacleType.None, 3))
				{
					yield return type;
				}

				currentOffset += 3;

				continue;
			}

			if (waveTotalWidth - currentOffset <= 2)
			{
				int requiredPaddingToBorder = waveTotalWidth - currentOffset;
				currentOffset += requiredPaddingToBorder;

				if (requiredPaddingToBorder == 1)
				{
					yield return ObstacleType.None;

					continue;
				}

				if (requiredPaddingToBorder == 2)
				{
					yield return currentDifficulty > 0.6f ? ObstacleType.Regular : ObstacleType.Small;

					continue;
				}
			}

			bool spawnGap = spawnGapBaseProbability - 0.25f * currentDifficulty > random.NextSingle();

			if (spawnGap)
			{
				currentOffset++;

				yield return ObstacleType.None;
			}
			else
			{
				ObstacleType obstacleType = GetObstacleType(currentDifficulty);
				currentOffset += obstacleType.GetWidth();

				yield return obstacleType;
			}
		}
	}

	private ObstacleType GetObstacleType(float difficulty)
	{
		double regularThreshold = 0.55 - 0.45 * difficulty;
		double bigThreshold = 0.8      - 0.25 * difficulty;
		double randomValue = random.NextDouble();

		if (randomValue < regularThreshold)
		{
			return ObstacleType.Small;
		}

		return randomValue < bigThreshold
				   ? ObstacleType.Regular
				   : ObstacleType.Big;
	}
}