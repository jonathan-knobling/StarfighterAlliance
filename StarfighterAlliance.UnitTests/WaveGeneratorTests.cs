using FluentAssertions;
using StarfighterAlliance.Core.Waves;

namespace StarfighterAlliance.UnitTests;

public class WaveGeneratorTests
{
	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(2)]
	[InlineData(3)]
	[InlineData(4)]
	[InlineData(5)]
	[InlineData(6)]
	[InlineData(7)]
	[InlineData(int.MaxValue)]
	[InlineData(int.MinValue)]
	[InlineData(10101989)]
	[InlineData(-10101989)]
	public void GenerateWave_ShouldHaveTotalWidthOf20(int rndSeed)
	{
		var random = new Random(rndSeed);
		float difficulty = random.NextSingle();

		// Arrange
		var waveGenerator = new WaveGenerator(random);
		const int expectedWaveTotalWidth = 20;

		// Act
		List<ObstacleType>? wave = waveGenerator.GenerateWave(difficulty).ToList();
		int totalWidth = wave.Sum(obstacleType => obstacleType.GetWidth());

		// Assert
		totalWidth.Should().Be(expectedWaveTotalWidth,
							   because: "the total width of obstacles in the wave must always sum up to 20 units.");
	}
}