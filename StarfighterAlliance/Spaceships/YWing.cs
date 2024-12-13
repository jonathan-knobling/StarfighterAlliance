using Godot;
using StarfighterAlliance.Spaceships.Shots;

namespace StarfighterAlliance.Spaceships;

public partial class YWing : Spaceship
{
	[Export] public PackedScene MissileScene = null!;
	private int shotCount;


	/// <summary>
	///     Every third shot spawn missile instead of normal shot.
	///     Every other time use base.Shoot() to spawn normal shot.
	/// </summary>
	protected override void Shoot()
	{
		shotCount++;

		if (shotCount % 3 == 0)
		{
			var missileInstance = MissileScene.Instantiate<Missile>();
			missileInstance.ShotPower = this.Attack;
			missileInstance.Position = this.GlobalPosition + this.ShotSpawnpoint;
			this.AddSibling(missileInstance);

			this.CanShoot = false;
			this.ShotTimer.Start();
		}
		else
		{
			base.Shoot();
		}
	}
}