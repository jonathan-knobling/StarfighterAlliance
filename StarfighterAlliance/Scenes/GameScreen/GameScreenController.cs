using System;
using Godot;
using Godot.DependencyInjection.Attributes;
using Microsoft.Extensions.Logging;
using StarfighterAlliance.Core.Config;
using StarfighterAlliance.Core.Stats;
using StarfighterAlliance.Spaceships;

namespace StarfighterAlliance.Scenes.GameScreen;

public partial class GameScreenController : Node
{
	[Inject] public IConfigService ConfigService { get; set; } = null!;
	[Inject] public ILogger<GameScreenController> Logger { get; set; } = null!;
	[Inject] public IScoreManager ScoreManager { get; set; } = null!;
	[Inject] public IGameResultRepository GameResultRepository { get; set; } = null!;

	public event SceneChangeHandler? GameEnded;

	private Label pointLabel = null!;
	private Label waveCounterLabel = null!;

	public override void _Ready()
	{
		pointLabel = this.GetNode<Label>(path: "UI/Game UI/Points");
		waveCounterLabel = this.GetNode<Label>(path: "UI/Game UI/Wave Counter");

		ScoreManager.StateChanged += OnScoreUpdate;
		ScoreManager.Reset();

		SpawnSpaceship();
	}

	internal void EndGame()
	{
		var result = new GameResult
		{
			FinalScore = ScoreManager.Score,
			WaveCount = ScoreManager.WaveCount,
			UsedSpaceship = ConfigService.SelectedSpaceshipType!.Name,
			GameEndedAt = DateTime.Now
		};

		GameResultRepository.AddGameResultAsync(result);

		GameEnded?.Invoke(new SceneChangeArgs(this));
	}

	private void SpawnSpaceship()
	{
		const string path = "res://Spaceships/{0}.tscn";
		SpaceshipTypeDto? spaceshipType = ConfigService.SelectedSpaceshipType;

		if (spaceshipType == null || string.IsNullOrEmpty(spaceshipType.SerializedName))
		{
			Logger.LogError(message: "SelectedSpaceshipType or SerializedName is null or invalid!");

			return;
		}

		string formattedPath = string.Format(path, spaceshipType.SerializedName.ToLower());
		var spaceshipScene = ResourceLoader.Load<PackedScene>(formattedPath);

		if (spaceshipScene == null)
		{
			Logger.LogError(message: "Failed to load scene at path: {path}", formattedPath);

			return;
		}

		var spaceship = spaceshipScene.Instantiate<Spaceship>();

		if (spaceship == null)
		{
			Logger.LogError(message: "Failed to instantiate the spaceship scene!");

			return;
		}

		spaceship.SetAttributes(spaceshipType);
		spaceship.Position = new Vector2(835, 830);
		this.AddChild(spaceship);


		var spaceshipSprite = spaceship.GetNode<Sprite2D>(path: "Texture");
		SetSpaceshipColor(spaceshipSprite);
	}

	private void SetSpaceshipColor(Sprite2D spaceshipSprite)
	{
		if (spaceshipSprite is not { Material: ShaderMaterial shaderMaterial })
		{
			Logger.LogError(message: "Texture or ShaderMaterial of Spaceship not found.");

			return;
		}

		shaderMaterial.SetShaderParameter(param: "swap_color", ConfigService.LoadedColor);
	}

	private void OnScoreUpdate()
	{
		if (IsInstanceValid(pointLabel) && IsInstanceValid(waveCounterLabel))
		{
			pointLabel.Text = $"points: {ScoreManager.Score}";
			waveCounterLabel.Text = $"wave: {ScoreManager.WaveCount}";
		}
	}
}
