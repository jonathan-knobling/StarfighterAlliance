namespace StarfighterAlliance.Core.Stats;

public delegate void StateChangedEvent();

public interface IScoreManager
{
	event StateChangedEvent StateChanged;
	
	int Score { get; }
	int WaveCount { get; }

	void AddScore(int amount);
	void IncrementWaveCount();
	void Reset();
}