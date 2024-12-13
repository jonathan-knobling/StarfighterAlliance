using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.DependencyInjection.Attributes;
using StarfighterAlliance.Core.Stats;

namespace StarfighterAlliance.Scenes.EndScreen;

public partial class EndScreenController : Node
{
	[Inject] public IGameResultRepository GameResultRepository { get; set; } = null!;

	[Export] public PackedScene HighscoreEntryComponent { get; set; } = null!;

	public event SceneChangeHandler? MainMenuButtonPressed;

	public override void _Ready()
	{
		var mainMenuButton = this.GetNode<Button>(path: "Menu Container/Main Menu Button");
		var quitGameButton = this.GetNode<Button>(path: "Menu Container/Quit Game Button");

		mainMenuButton.ButtonUp += OnMainMenuButtonOnButtonUp;
		quitGameButton.ButtonUp += OnQuitGame;

		Callable.From(PopulateHighscoreData).CallDeferred();
	}

	private async void PopulateHighscoreData()
	{
		var highscoreList = this.GetNode<VBoxContainer>(path: "High Score List");

		List<GameResult> gameResults = await GameResultRepository.GetHighScoresAsync(10);
		GameResult latestResult = await GameResultRepository.GetLatestResultAsync();

		if (!gameResults.Contains(latestResult))
		{
			gameResults.Add(latestResult);
			gameResults = gameResults.OrderByDescending(x => x.FinalScore).ToList();
		}

		for (var index = 0; index < gameResults.Count; index++)
		{
			GameResult result = gameResults[index];
			var entry = HighscoreEntryComponent.Instantiate<HighscoreEntry>();
			entry.InitializeWithData(result, index + 1, result == latestResult);
			highscoreList.AddChild(entry);
		}
	}

	private void OnQuitGame()
	{
		this.GetTree().Quit();
	}

	private void OnMainMenuButtonOnButtonUp()
	{
		MainMenuButtonPressed?.Invoke(new SceneChangeArgs(this));
	}
}