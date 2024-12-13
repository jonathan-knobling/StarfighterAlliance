using Godot;

namespace StarfighterAlliance.Spaceships.Shots;

public partial class Shot : Node2D
{
	private const int ShotSpeed = 1000;

	public int ShotPower { get; set; }

	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = Vector2.Up;
		this.Position += direction * ShotSpeed * (float)delta;

		DespawnShotIfOutsideViewport();
	}

	private void DespawnShotIfOutsideViewport()
	{
		Vector2 viewportCenter = this.GetViewportRect().GetCenter();
		bool isOutsideViewport = this.GlobalPosition.DistanceSquaredTo(viewportCenter) > 1200 * 1200;

		if (isOutsideViewport)
		{
			this.QueueFree();
		}
	}
}