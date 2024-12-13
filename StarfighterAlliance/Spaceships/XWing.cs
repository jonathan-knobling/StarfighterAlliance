using Godot;

namespace StarfighterAlliance.Spaceships;

public partial class XWing : Spaceship
{
	private Vector2 shotSpawnpoint2;

	public override void _Ready()
	{
		base._Ready();

		shotSpawnpoint2 = this.GetNode<Node2D>(path: "Shot Spawnpoint2").Position;
	}

	protected override void Shoot()
	{
		this.InstantiateShot(shotSpawnpoint2);

		base.Shoot();
	}
}