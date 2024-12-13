using System.Collections.Generic;
using Godot;
using StarfighterAlliance.Obstacles;

namespace StarfighterAlliance.Spaceships.Shots;

public partial class Missile : Shot
{
	private readonly List<Obstacle> obstaclesInExplosionRadius = [];
	private Area2D ExplosionRadius { get; set; } = null!;

	public override void _Ready()
	{
		base._Ready();

		ExplosionRadius = this.GetNode<Area2D>(path: "Explosion Radius");
		ExplosionRadius.AreaEntered += OnExplosionRadiusEntered;
		ExplosionRadius.AreaExited += OnExplosionRadiusExited;
	}

	public void TriggerCollision()
	{
		SpawnExplosionParticles();

		foreach (Obstacle? obstacle in obstaclesInExplosionRadius)
		{
			obstacle.Damage(this.ShotPower);
		}
	}

	private void SpawnExplosionParticles()
	{
		// source https://www.youtube.com/watch?v=F1Fyj3Lh_Pc
		var particle = ResourceLoader.Load<PackedScene>(path: "res://Spaceships/Shots/Explosion.tscn")
									 .Instantiate<GpuParticles2D>();

		this.AddSibling(particle);
		particle.Position = this.Position;
		particle.SetEmitting(true);
		particle.Finished += particle.QueueFree;
	}

	private void OnExplosionRadiusExited(Area2D area)
	{
		if (area is Obstacle obstacle)
		{
			obstaclesInExplosionRadius.Remove(obstacle);
		}
	}

	private void OnExplosionRadiusEntered(Area2D area)
	{
		if (area is Obstacle obstacle)
		{
			obstaclesInExplosionRadius.Add(obstacle);
		}
	}
}
