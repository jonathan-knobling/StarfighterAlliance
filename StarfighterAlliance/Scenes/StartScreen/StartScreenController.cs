using Godot;

namespace StarfighterAlliance.Scenes.StartScreen;

public partial class StartScreenController : Node
{
	public event SceneChangeHandler? StartGameButtonPressed;

	public override void _Ready()
	{
		var startGameButton = this.GetNode<Button>(path: "Menu Container/Start Game Button");
		var quitGameButton = this.GetNode<Button>(path: "Menu Container/Quit Game Button");

		startGameButton.ButtonUp += OnStartGameButtonOnButtonUp;
		quitGameButton.ButtonUp += OnQuitGame;
	}

	private void OnQuitGame()
	{
		this.GetTree().Quit();
	}

	private void OnStartGameButtonOnButtonUp()
	{
		StartGameButtonPressed?.Invoke(new SceneChangeArgs(this));
	}
}