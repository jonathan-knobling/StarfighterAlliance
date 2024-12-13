using System.Threading.Tasks;
using Godot;
using Godot.DependencyInjection.Attributes;
using StarfighterAlliance.Infrastructure;

namespace StarfighterAlliance.Scenes.PauseMenu;

public partial class PauseMenuController : Control
{
	[Inject] public GameDbContext Context { get; set; } = null!;

	public override void _Ready()
	{
		var quitGameButton = this.GetNode<Button>(path: "GridContainer/Quit Game Button");
		var resumeButton = this.GetNode<Button>(path: "GridContainer/Resume Button");

		quitGameButton.ButtonUp += OnQuitGame;
		resumeButton.ButtonUp += TogglePaused;
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed(action: "pause"))
		{
			TogglePaused();
		}
	}

	private void TogglePaused()
	{
		// source https://www.youtube.com/watch?v=e9-WQg1yMCY
		var animationPlayer = this.GetNode<AnimationPlayer>(path: "AnimationPlayer");
		var uiContainer = this.GetNode<GridContainer>(path: "GridContainer");

		if (this.GetTree().Paused)
		{
			this.GetTree().Paused = false;
			animationPlayer.PlayBackwards(name: "blur");
			uiContainer.Visible = false;

			return;
		}

		animationPlayer.Play(name: "blur");

		async void ShowButtonsAfterDelay()
		{
			await Task.Delay(200);
			uiContainer.Visible = true;
		}

		Callable.From(ShowButtonsAfterDelay).CallDeferred();

		this.GetTree().Paused = true;
	}

	private void OnQuitGame()
	{
		this.GetTree().Quit();
	}
}
