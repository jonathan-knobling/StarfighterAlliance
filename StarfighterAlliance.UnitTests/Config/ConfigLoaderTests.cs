using System.IO.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using StarfighterAlliance.Core.Config;
using FileSystem = StarfighterAlliance.Core.FileSystem;

namespace StarfighterAlliance.UnitTests.Config;

public class ConfigLoaderTests
{
	#region JsonConstants

	private const string ValidJson = """
									 {
									    "ObstacleHealthModifier": 1.5,
									    "SpaceshipTypes": 
									    [
									        { "Name": "TestShip", "SerializedName": "TestShip", "Health": 100, "AttackDamage": 50, "Speed": 300 }
									    ]
									 }
									 """;

	private const string InvalidJson = """
									   {
									      "ObstacleHealthModifiers": "1.5",
									      "SpaceshipType":
									      {
									          { "Name": "TestShip", "SerializedName": "TestShip", "Health": 100, "AttackDamage": 50, "Speed": 300 }
									      {
									   }
									   """;

	private const string ValidJsonMissingProperties = """
													  {
													     "ObstacleHealthModifier": 1.5,
													     "SpaceshipTypes": 
													     [
													         { "Name": "TestShip", "Speed": 300 }
													     ]
													  }
													  """;

	#endregion

	#region Logger

	private readonly ILogger<ConfigLoader> logger;

	public ConfigLoaderTests()
	{
		logger = Substitute.For<ILogger<ConfigLoader>>();
	}

	#endregion

	#region LoadConfigTests

	[Fact]
	public void LoadConfig_ShouldLoadConfig_WhenJsonIsValid()
	{
		// Arrange
		var fileSystem = Substitute.For<IFileSystem>();
		fileSystem.File.ReadAllText(Arg.Any<string>()).Returns(ValidJson);
		var configLoader = new ConfigLoader(fileSystem, logger);

		// Act
		configLoader.LoadConfig();

		// Assert
		fileSystem.File.ReceivedWithAnyArgs().ReadAllText(path: "");
		logger.DidNotReceiveWithAnyArgs().LogWarning(message: "");

		List<SpaceshipTypeDto>? spaceshipTypes = configLoader.GetSpaceshipTypes();
		spaceshipTypes.Should().ContainSingle();
		spaceshipTypes[0].Name.Should().Be(expected: "TestShip");
		configLoader.GetObstacleHealthModifier().Should().Be(1.5f);
	}

	[Fact]
	public void LoadConfig_ShouldLoadDefaultConfig_WhenFileIsMissing()
	{
		// Arrange
		var fileSystem = Substitute.For<IFileSystem>();
		fileSystem.File.ReadAllText(Arg.Any<string>()).Throws<FileNotFoundException>();
		var configLoader = new ConfigLoader(fileSystem, logger);

		// Act
		configLoader.LoadConfig();

		// Assert
		fileSystem.File.ReceivedWithAnyArgs().ReadAllText(path: "");
		logger.ReceivedWithAnyArgs().LogWarning(message: "");

		List<SpaceshipTypeDto>? spaceshipTypes = configLoader.GetSpaceshipTypes();
		spaceshipTypes.Should().NotBeEmpty();
		spaceshipTypes[0].Name.Should().NotBeEmpty();
		configLoader.GetObstacleHealthModifier().Should().Be(1.0f);
	}

	[Fact]
	public void LoadConfig_ShouldLoadDefaultConfig_WhenJsonIsInvalid()
	{
		// Arrange
		var fileSystem = Substitute.For<IFileSystem>();
		fileSystem.File.ReadAllText(Arg.Any<string>()).Returns(InvalidJson);
		var configLoader = new ConfigLoader(fileSystem, logger);

		// Act
		configLoader.LoadConfig();

		// Assert
		fileSystem.File.ReceivedWithAnyArgs().ReadAllText(path: "");
		logger.ReceivedWithAnyArgs().LogWarning(message: "");

		List<SpaceshipTypeDto>? spaceshipTypes = configLoader.GetSpaceshipTypes();
		spaceshipTypes.Should().NotBeEmpty();
		spaceshipTypes[0].Name.Should().NotBeEmpty();
		configLoader.GetObstacleHealthModifier().Should().Be(1.0f);
	}

	[Fact]
	public void LoadConfig_ShouldLoadDefaultConfig_WhenJsonIsMissingProperties()
	{
		// Arrange
		var fileSystem = Substitute.For<IFileSystem>();
		fileSystem.File.ReadAllText(Arg.Any<string>()).Returns(ValidJsonMissingProperties);
		var configLoader = new ConfigLoader(fileSystem, logger);

		// Act
		configLoader.LoadConfig();

		// Assert
		fileSystem.File.ReceivedWithAnyArgs().ReadAllText(path: "");
		logger.ReceivedWithAnyArgs().LogWarning(message: "");

		List<SpaceshipTypeDto>? spaceshipTypes = configLoader.GetSpaceshipTypes();
		spaceshipTypes.Should().NotBeEmpty();
		spaceshipTypes[0].Name.Should().NotBeEmpty();
		configLoader.GetObstacleHealthModifier().Should().Be(1.0f);
	}

	[Fact]
	public void LoadConfig_ShouldLoadJsonFile_WhenDefaultFileSystemIsInjected()
	{
		// Arrange
		var fileSystem = new FileSystem();
		var configLoader = new ConfigLoader(fileSystem, logger);

		// Act
		configLoader.LoadConfig(path: "StarfighterAlliance.UnitTests/Config/config.json");

		// Assert
		logger.DidNotReceiveWithAnyArgs().LogWarning(message: "");

		List<SpaceshipTypeDto>? spaceshipTypes = configLoader.GetSpaceshipTypes();
		spaceshipTypes.Should().NotBeEmpty();
		spaceshipTypes[0].Name.Should().NotBeEmpty();
		configLoader.GetObstacleHealthModifier().Should().Be(1.5f);
	}

	#endregion

	#region GetSpaceshipTypesTests

	[Fact]
	public void GetSpaceshipTypes_ShouldReturnTypes_WhenConfigCouldBeLoaded()
	{
		// Arrange
		var fileSystem = Substitute.For<IFileSystem>();
		fileSystem.File.ReadAllText(Arg.Any<string>()).Returns(ValidJson);
		var configLoader = new ConfigLoader(fileSystem, logger);

		// Act
		configLoader.LoadConfig();
		List<SpaceshipTypeDto>? spaceshipTypes = configLoader.GetSpaceshipTypes();

		// Assert
		logger.DidNotReceiveWithAnyArgs().LogWarning(message: "");
		fileSystem.File.ReceivedWithAnyArgs().ReadAllText(path: "");

		spaceshipTypes.Should().NotBeEmpty();
		spaceshipTypes[0].Name.Should().NotBeEmpty();
	}

	[Fact]
	public void GetSpaceshipTypes_ShouldLoadConfig_WhenConfigWasNotLoaded()
	{
		// Arrange
		var fileSystem = Substitute.For<IFileSystem>();
		fileSystem.File.ReadAllText(Arg.Any<string>()).Returns(ValidJson);
		var configLoader = new ConfigLoader(fileSystem, logger);

		// Act
		List<SpaceshipTypeDto>? spaceshipTypes = configLoader.GetSpaceshipTypes();

		// Assert
		logger.DidNotReceiveWithAnyArgs().LogWarning(message: "");
		fileSystem.File.ReceivedWithAnyArgs().ReadAllText(path: "");

		spaceshipTypes.Should().NotBeEmpty();
		spaceshipTypes[0].Name.Should().NotBeEmpty();
	}

	#endregion

	#region GetObstacleHealthModifierTests

	[Fact]
	public void GetObstacleHealthModifier_ShouldReturnModifier_WhenConfigCouldBeLoaded()
	{
		// Arrange
		var fileSystem = Substitute.For<IFileSystem>();
		fileSystem.File.ReadAllText(Arg.Any<string>()).Returns(ValidJson);
		var configLoader = new ConfigLoader(fileSystem, logger);

		// Act
		configLoader.LoadConfig();
		float healthModifier = configLoader.GetObstacleHealthModifier();

		// Assert
		logger.DidNotReceiveWithAnyArgs().LogWarning(message: "");
		fileSystem.File.ReceivedWithAnyArgs().ReadAllText(path: "");

		healthModifier.Should().NotBeApproximately(0.0f, 0.0001f);
	}

	[Fact]
	public void GetObstacleHealthModifier_ShouldLoadConfig_WhenConfigWasNotLoaded()
	{
		// Arrange
		var fileSystem = Substitute.For<IFileSystem>();
		fileSystem.File.ReadAllText(Arg.Any<string>()).Returns(ValidJson);
		var configLoader = new ConfigLoader(fileSystem, logger);

		// Act
		float healthModifier = configLoader.GetObstacleHealthModifier();

		// Assert
		logger.DidNotReceiveWithAnyArgs().LogWarning(message: "");
		fileSystem.File.ReceivedWithAnyArgs().ReadAllText(path: "");

		healthModifier.Should().NotBeApproximately(0.0f, 0.0001f);
	}

	#endregion
}