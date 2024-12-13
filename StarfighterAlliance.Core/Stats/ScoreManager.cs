using Godot;

namespace StarfighterAlliance.Core.Stats;

public class ScoreManager : IScoreManager
{
	private int score;
	private int waveCount;

	public event StateChangedEvent? StateChanged;

	public int Score
	{
		get => score;
		private set => score = Mathf.Max(0, value);
	}

	public int WaveCount
	{
		get => waveCount;
		private set => waveCount = Mathf.Max(0, value);
	}

	public void AddScore(int amount)
	{
		Score += amount;
		StateChanged?.Invoke();
	}

	public void IncrementWaveCount()
	{
		WaveCount++;
		StateChanged?.Invoke();
	}

	public void Reset()
	{
		Score = 0;
		WaveCount = 0;
		StateChanged?.Invoke();
	}
}