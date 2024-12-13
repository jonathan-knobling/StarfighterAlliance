using System;
using System.Diagnostics;
using Ardalis.GuardClauses;
using Godot;
using Godot.DependencyInjection.Attributes;
using Microsoft.Extensions.Logging;
using StarfighterAlliance.Scenes.EndScreen;
using StarfighterAlliance.Scenes.GameScreen;
using StarfighterAlliance.Scenes.SelectionScreen;
using StarfighterAlliance.Scenes.StartScreen;

namespace StarfighterAlliance.Scenes;

/// <summary>
///     Controls the active scene in the game, allowing transitions between different Screens.
/// </summary>
public partial class SceneController : Node
{
	/// <summary>
	///     The scene to display as the Start Screen.
	///     Must be a PackedScene that instantiates a StartScreenController.
	/// </summary>
	[Export]
	public PackedScene? StartScreen { get; set; }

	/// <summary>
	///     The scene to display as the Spaceship Selection Screen.
	///     Must be a PackedScene that instantiates a EndScreenController.
	/// </summary>
	[Export]
	public PackedScene? SpaceshipSelectionScreen { get; set; }

	/// <summary>
	///     The scene to display as the Game Screen.
	///     Must be a PackedScene that instantiates a GameScreenController.
	/// </summary>
	[Export]
	public PackedScene? GameScreen { get; set; }

	/// <summary>
	///     The scene to display as the End Screen.
	///     Must be a PackedScene that instantiates a EndScreenController.
	/// </summary>
	[Export]
	public PackedScene? EndScreen { get; set; }

	[Inject] public ILogger<SceneController> Logger { get; set; } = null!;

	public override void _Ready()
	{
		Guard.Against.Null(StartScreen, nameof(StartScreen), message: "Start Screen must be selected.");
		Guard.Against.Null(SpaceshipSelectionScreen, nameof(SpaceshipSelectionScreen),
						   message: "Spaceship Selection Screen must be selected.");

		Guard.Against.Null(GameScreen, nameof(GameScreen), message: "Game Screen must be selected.");
		Guard.Against.Null(EndScreen, nameof(EndScreen), message: "End Screen must be selected.");

		TransitionToStartScreen(null);
	}

	private void TransitionToStartScreen(SceneChangeArgs? sceneChangeArgs)
	{
		if (StartScreen is null)
		{
			throw new UnreachableException(message: "start screen cannot be null");
		}

		TransitionToScene<StartScreenController>(
			StartScreen,
			sceneChangeArgs,
			startScreenController =>
			{
				startScreenController.StartGameButtonPressed += TransitionToSpaceshipSelectionScreen;
			});
	}

	private void TransitionToSpaceshipSelectionScreen(SceneChangeArgs sceneChangeArgs)
	{
		if (SpaceshipSelectionScreen is null)
		{
			throw new UnreachableException(message: "spaceship selection screen cannot be null");
		}

		TransitionToScene<SelectionScreenController>(
			SpaceshipSelectionScreen,
			sceneChangeArgs,
			selectionController =>
			{
				selectionController.SpaceshipSelected += TransitionToGameScreen;
			});
	}

	private void TransitionToGameScreen(SceneChangeArgs? sceneChangeArgs)
	{
		if (GameScreen is null)
		{
			throw new UnreachableException(message: "game screen cannot be null");
		}

		TransitionToScene<GameScreenController>(
			GameScreen,
			sceneChangeArgs,
			gameScreenController =>
			{
				gameScreenController.GameEnded += TransitionToEndScreen;
			});
	}

	private void TransitionToEndScreen(SceneChangeArgs? sceneChangeArgs)
	{
		if (EndScreen is null)
		{
			throw new UnreachableException(message: "end screen cannot be null");
		}

		TransitionToScene<EndScreenController>(
			EndScreen,
			sceneChangeArgs,
			gameScreenController =>
			{
				gameScreenController.MainMenuButtonPressed += TransitionToStartScreen;
			});
	}

	private void TransitionToScene<T>(
		PackedScene scene,
		SceneChangeArgs? sceneChangeArgs,
		Action<T>? setupCallback = null)
		where T : Node
	{
		sceneChangeArgs?.OldScene.QueueFree();

		if (scene.Instantiate() is T sceneInstance)
		{
			this.AddChild(sceneInstance);
			setupCallback?.Invoke(sceneInstance);
			Logger.LogInformation("Switched to {SceneName}", sceneInstance.Name);

			return;
		}

		Logger.LogError("Failed to instantiate scene. It has to be of type {SceneType}", typeof(T).Name);
	}
}