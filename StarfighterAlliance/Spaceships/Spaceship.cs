using Ardalis.GuardClauses;
using Godot;
using StarfighterAlliance.Core.Config;
using StarfighterAlliance.Scenes.GameScreen;
using StarfighterAlliance.Spaceships.Shots;

namespace StarfighterAlliance.Spaceships;

public partial class Spaceship : CharacterBody2D
{
	private const int HealthBarInitialScale = 150;
	private const float ShotCooldownTime = 0.35f;
	protected bool CanShoot = true;

	private Sprite2D healthBarSprite = null!;
	private int initialHealth;
	protected Vector2 ShotSpawnpoint;
	protected Timer ShotTimer = null!;

	public int Speed { get; set; }
	public int Attack { get; set; }
	public int Health { get; set; }

	[Export] public PackedScene ShotScene { get; set; } = null!;

	/// <summary>
	///     Sets this spaceships attributes from the passed <see cref="SpaceshipTypeDto" />.
	/// </summary>
	/// <param name="spaceshipType">The <see cref="SpaceshipTypeDto" /> to get the Attributes from</param>
	public void SetAttributes(SpaceshipTypeDto spaceshipType)
	{
		Speed = spaceshipType.Speed;
		Attack = spaceshipType.AttackDamage;
		Health = spaceshipType.Health;

		initialHealth = Health;
	}

	/// <summary>
	///     Inflicts damage to this spaceship (subtracts health) and updates the healthbar.
	/// </summary>
	/// <param name="damage">Amount of damage to inflict to this spaceship instance</param>
	public void Damage(float damage)
	{
		Health -= Mathf.FloorToInt(damage);

		float healthBarPercentage = (float)Health / initialHealth;

		healthBarSprite.Scale = healthBarSprite.Scale with { X = healthBarPercentage * HealthBarInitialScale };
		healthBarSprite.Modulate = GetHealthBarColor(healthBarPercentage);

		if (Health <= 0)
		{
			this.QueueFree();
			var gameScreenController = this.GetParent<GameScreenController>();
			gameScreenController.EndGame();
		}
	}

	public override void _Ready()
	{
		Guard.Against.Null(ShotScene, nameof(ShotScene), message: "ShotScene must be selected.");

		healthBarSprite = this.GetNode<Sprite2D>(path: "Healthbar");
		ShotSpawnpoint = this.GetNode<Node2D>(path: "Shot Spawnpoint").Position;

		AddShotTimerToSceneTree();
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		Move();

		if (Input.IsKeyPressed(Key.Space) && CanShoot)
		{
			Shoot();
		}
	}

	private void AddShotTimerToSceneTree()
	{
		ShotTimer = new Timer
		{
			WaitTime = ShotCooldownTime,
			Autostart = false,
			OneShot = true
		};

		ShotTimer.Timeout += () => CanShoot = true;
		this.AddChild(ShotTimer);
	}

	private void Move()
	{
		float inputX = Input.GetAxis(negativeAction: "moveLeft", positiveAction: "moveRight");
		float inputY = Input.GetAxis(negativeAction: "moveUp", positiveAction: "moveDown");
		Vector2 direction = new Vector2(inputX, inputY).Normalized();
		this.Velocity = direction * Speed;

		this.MoveAndSlide();
	}

	protected virtual void Shoot()
	{
		InstantiateShot(ShotSpawnpoint);

		CanShoot = false;
		ShotTimer.Start();
	}

	protected void InstantiateShot(Vector2 position)
	{
		var shotInstance = ShotScene.Instantiate<Shot>();
		shotInstance.ShotPower = Attack;
		shotInstance.Position = this.GlobalPosition + position;
		this.AddSibling(shotInstance);
	}

	private static Color GetHealthBarColor(float healthBarPercentage)
	{
		healthBarPercentage = Mathf.Clamp(healthBarPercentage, 0.0f, 1.0f);

		float red = 1.0f - healthBarPercentage;
		float green = healthBarPercentage;

		return new Color(red, green, 0.0f);
	}
}
