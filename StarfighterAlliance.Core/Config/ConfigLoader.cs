using System.IO.Abstractions;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace StarfighterAlliance.Core.Config;

public class ConfigLoader : IConfigLoader
{
	private readonly IFileSystem fileSystem;
	private readonly ILogger<ConfigLoader> logger;
	private Config? cachedConfig;

	public ConfigLoader(IFileSystem fileSystem, ILogger<ConfigLoader> logger)
	{
		this.fileSystem = fileSystem;
		this.logger = logger;
	}

	/// <summary>
	///     Load config with default file path
	/// </summary>
	public void LoadConfig()
	{
		LoadConfig(path: "StarfighterAlliance/config.json");
	}

	/// <summary>
	///     Load config with custom file path
	/// </summary>
	/// <param name="path">Path of the json file that contains the configuration.</param>
	public void LoadConfig(string path)
	{
		try
		{
			string json = fileSystem.File.ReadAllText(path);
			var config = JsonSerializer.Deserialize<Config>(json);
			cachedConfig = config ?? LoadDefaultConfig();
		}
		catch (Exception e)
		{
			logger.LogWarning(message: "Json config could not be loaded: {exception}", e.Message);
			logger.LogInformation(message: "Loading default config values.");

			cachedConfig = LoadDefaultConfig();
		}
	}

	public List<SpaceshipTypeDto> GetSpaceshipTypes()
	{
		if (cachedConfig is null)
		{
			LoadConfig();
		}

		return cachedConfig!.SpaceshipTypes.ToList();
	}

	public float GetObstacleHealthModifier()
	{
		if (cachedConfig is null)
		{
			LoadConfig();
		}

		return cachedConfig!.ObstacleHealthModifier;
	}

	private Config LoadDefaultConfig()
	{
		return new Config
		{
			ObstacleHealthModifier = 1.0f,
			SpaceshipTypes =
			[
				new SpaceshipTypeDto
				{
					Name = "Millenium Falcon",
					SerializedName = "MilleniumFalcon",
					Health = 10,
					AttackDamage = 5,
					Speed = 650
				},
				new SpaceshipTypeDto
				{
					Name = "X-Wing",
					SerializedName = "XWing",
					Health = 10,
					AttackDamage = 5,
					Speed = 650
				},
				new SpaceshipTypeDto
				{
					Name = "Y-Wing",
					SerializedName = "YWing",
					Health = 10,
					AttackDamage = 5,
					Speed = 650
				}
			]
		};
	}

	private class Config
	{
		public required float ObstacleHealthModifier { get; init; }
		public required SpaceshipTypeDto[] SpaceshipTypes { get; init; }
	}
}