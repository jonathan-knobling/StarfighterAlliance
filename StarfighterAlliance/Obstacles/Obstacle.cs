using Godot;
using StarfighterAlliance.Spaceships;
using StarfighterAlliance.Spaceships.Shots;

namespace StarfighterAlliance.Obstacles;

public delegate void ObstacleDestroyedEvent(Obstacle obstacle, bool destroyedByPlayer);

public partial class Obstacle : Area2D
{
	private const int BottomWorldBoundCollisionLayer = 1;
	private const int SpaceshipCollisionLayer = 2;

	public const int InitialSpeed = 175;

	private bool isDestroyed;
	[Export] public int Health { get; set; } = 12;

	public float HealthModifier { get; set; }
	public float SpeedModifier { get; set; }

	public event ObstacleDestroyedEvent? ObstacleDestroyed;

	public override void _Ready()
	{
		this.Monitoring = true;
		this.BodyEntered += OnBodyEntered;
		this.BodyExited += OnBodyExited;
	}

	public override void _PhysicsProcess(double delta)
	{
		this.Position += new Vector2(0, (float)(delta * InitialSpeed * SpeedModifier));
	}

	public void Damage(int damage)
	{
		Health -= damage;

		if (Health <= 0)
		{
			var particles = this.GetNode<GpuParticles2D>(path: "Destruction Particle System");
			particles.SetEmitting(true);

			var texture = this.GetNode<Sprite2D>(path: "Texture");
			texture.SetVisible(false);
			isDestroyed = true;
			ObstacleDestroyed?.Invoke(this, true);

			particles.Finished += this.QueueFree;
		}
	}

	private void OnBodyExited(Node2D body)
	{
		if (body is not StaticBody2D staticBody)
		{
			return;
		}

		bool bodyIsWorldBounds = (staticBody.CollisionLayer & (1 << BottomWorldBoundCollisionLayer)) != 0;

		if (bodyIsWorldBounds)
		{
			ObstacleDestroyed?.Invoke(this, false);
			this.QueueFree();
		}
	}

	private void OnBodyEntered(Node2D body)
	{
		if (isDestroyed)
		{
			return;
		}

		CheckSpaceshipCollision(body);
		CheckShotCollision(body);
	}

	private void CheckShotCollision(Node2D body)
	{
		if (body is not Shot shot)
		{
			return;
		}

		if (shot is Missile missile)
		{
			missile.TriggerCollision();
		}

		shot.QueueFree();

		Damage(shot.ShotPower);
	}

	private void CheckSpaceshipCollision(Node2D body)
	{
		if (body is not Spaceship spaceship)
		{
			return;
		}

		spaceship.Damage(Health / 5.0f);

		Damage(int.MaxValue);
	}
}
