using Godot;
using Microsoft.Extensions.Logging;

namespace StarfighterAlliance.Obstacles;

public partial class GameCountdown : Node
{
	public delegate void GameCountDownEndedEvent();
	
	
	public GameCountDownEndedEvent? GameCountDownEnded;
	
	private Label label = null!;
	private SceneTreeTimer timer = null!;

	public override void _Ready()
	{
		timer = this.GetTree().CreateTimer(4, false);
		
		timer.Timeout += () =>
		{
			this.QueueFree();
			label.Visible = false;
			GameCountDownEnded?.Invoke();
		};

		label = this.GetNode<Label>("Countdown");
		label.Visible = true;
	}

	public override void _Process(double delta)
	{
		label.Text = timer.GetTimeLeft() < 1 ? "Start!" : Mathf.CeilToInt(timer.GetTimeLeft() - 1).ToString();
	}
}