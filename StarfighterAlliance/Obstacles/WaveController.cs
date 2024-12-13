using System.Collections.Generic;
using Godot;
using Godot.DependencyInjection.Attributes;
using StarfighterAlliance.Core.Config;
using StarfighterAlliance.Core.Stats;

namespace StarfighterAlliance.Obstacles;

public partial class WaveController : Node
{
	private const int WaveSurvivedScoreGain = 100;
	private const int WaveDestroyedScoreGain = 175;
	private readonly List<Wave> waves = [];
	private float healthModifier;

	private Timer waveTimer = null!;

	[Inject] public IConfigLoader ConfigLoader { get; set; } = null!;
	[Inject] public IScoreManager ScoreManager { get; set; } = null!;

	public override void _Ready()
	{
		healthModifier = ConfigLoader.GetObstacleHealthModifier();

		var countdown = this.GetNode<GameCountdown>("Game Countdown");
		countdown.GameCountDownEnded += StartWaveGeneration;

		waveTimer = new Timer
		{
			WaitTime = 4.5f,
			Autostart = false,
			OneShot = false
		};

		waveTimer.Timeout += OnTimerEnded;
		this.AddChild(waveTimer);
	}

	private void OnTimerEnded()
	{
		waveTimer.WaitTime = double.Max(waveTimer.WaitTime * 0.97f, 1.65f);

		GenerateNewWave();
	}

	private void StartWaveGeneration()
	{
		waveTimer.Start();
		GenerateNewWave();
	}

	private void GenerateNewWave()
	{
		waves.Add(new Wave(SpawnObstacle, OnWaveDestroyed, GetWaveDifficultyModifier()));
		ScoreManager.IncrementWaveCount();
	}

	private void SpawnObstacle(Node2D obstacle)
	{
		this.AddChild(obstacle);
	}

	private void OnWaveDestroyed(Wave wave, bool destroyedByPlayer)
	{
		waves.Remove(wave);
		ScoreManager.AddScore(destroyedByPlayer ? WaveDestroyedScoreGain : WaveSurvivedScoreGain);
	}

	private float GetWaveDifficultyModifier()
	{
		return Mathf.Min(ScoreManager.WaveCount, 25) / 25.0f;
	}
}