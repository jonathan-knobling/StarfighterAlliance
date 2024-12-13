using System;
using System.Collections.Generic;
using Godot;
using StarfighterAlliance.Core.Waves;

namespace StarfighterAlliance.Obstacles;

public class Wave
{
	private readonly PackedScene bigObstacle =
		ResourceLoader.Load<PackedScene>(path: "res://Obstacles/big_obstacle.tscn");

	private readonly List<Obstacle> obstacles = [];

	private readonly PackedScene regularObstacle =
		ResourceLoader.Load<PackedScene>(path: "res://Obstacles/regular_obstacle.tscn");

	private readonly PackedScene smallObstacle =
		ResourceLoader.Load<PackedScene>(path: "res://Obstacles/small_obstacle.tscn");

	private readonly Action<Wave, bool> waveDestroyedCallback;

	public Wave(
		Action<Node2D> spawnObstacleCallback,
		Action<Wave, bool> waveDestroyedCallback,
		float difficultyModifier = 0.0f)
	{
		this.waveDestroyedCallback = waveDestroyedCallback;

		difficultyModifier = Mathf.Clamp(difficultyModifier, 0.0f, 1.0f);

		SpawnWave(spawnObstacleCallback, difficultyModifier);
	}

	#region Callbacks

	private void OnObstacleDestroyed(Obstacle obstacle, bool destroyedByPlayer)
	{
		obstacles.Remove(obstacle);

		if (obstacles.Count == 0)
		{
			waveDestroyedCallback.Invoke(this, destroyedByPlayer);
		}
	}

	#endregion

	#region WaveGeneration

	private void SpawnWave(Action<Node2D> spawnObstacleCallback, float difficultyModifier)
	{
		var generator = new WaveGenerator(Random.Shared);
		IEnumerable<ObstacleType> wave = generator.GenerateWave(difficultyModifier);

		var offset = 0;

		foreach (ObstacleType obstacleType in wave)
		{
			if (obstacleType == ObstacleType.None)
			{
				offset++;

				continue;
			}

			int obstacleOffset = offset;
			Obstacle obstacle = InstantiateObstacle(obstacleType, ref offset);

			obstacle.Position = GetObstacleInitialCoords(obstacleOffset, obstacleType);
			obstacle.SpeedModifier = Mathf.Clamp(difficultyModifier  * 1.2f + 1f, 1.0f, 2.2f);
			obstacle.HealthModifier = Mathf.Clamp(difficultyModifier * 1.2f + 1f, 1.0f, 2.2f);

			spawnObstacleCallback.Invoke(obstacle);
			obstacles.Add(obstacle);

			obstacle.ObstacleDestroyed += OnObstacleDestroyed;
		}
	}

	/// <summary>
	///     Instantiates an obstacle of the specified <see cref="ObstacleType" /> and updates the offset based on the
	///     obstacle's width.
	/// </summary>
	/// <param name="obstacleType">
	///     The type of obstacle to instantiate. Must be one of the defined <see cref="ObstacleType" /> values.
	/// </param>
	/// <param name="offset">
	///     A reference to the current offset, which is incremented based on the width of the instantiated obstacle.
	/// </param>
	/// <returns>
	///     An instance of the <see cref="Obstacle" /> corresponding to the specified <paramref name="obstacleType" />.
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException">
	///     Thrown if an unsupported or invalid <see cref="ObstacleType" /> is provided.
	/// </exception>
	private Obstacle InstantiateObstacle(ObstacleType obstacleType, ref int offset)
	{
		Obstacle obstacle = obstacleType switch
		{
			ObstacleType.Small   => smallObstacle.Instantiate<Obstacle>(),
			ObstacleType.Regular => regularObstacle.Instantiate<Obstacle>(),
			ObstacleType.Big     => bigObstacle.Instantiate<Obstacle>(),
			_                    => throw new ArgumentOutOfRangeException(nameof(obstacleType), obstacleType, null)
		};

		offset += obstacleType.GetWidth();

		return obstacle;
	}

	private Vector2 GetObstacleInitialCoords(int obstaclePosition, ObstacleType obstacleType)
	{
		const int obstaclePositionOffset = 96;
		const int waveOffset = 96;

		int positionOffsetX = obstaclePosition * obstaclePositionOffset + waveOffset;

		if (obstacleType == ObstacleType.Big)
		{
			positionOffsetX += 48;
		}

		return new Vector2(positionOffsetX, -waveOffset);
	}

	#endregion
}