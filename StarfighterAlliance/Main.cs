using System.IO.Abstractions;
using Godot;
using Godot.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StarfighterAlliance.Core.Config;
using StarfighterAlliance.Core.Logging;
using StarfighterAlliance.Core.Spaceship.Color;
using StarfighterAlliance.Core.Stats;
using StarfighterAlliance.Infrastructure;
using FileSystem = StarfighterAlliance.Core.FileSystem;

namespace StarfighterAlliance;

/// <summary>
///     <para>
///         Script of the root node that is parent of the Scene Controller Node and entry point of the application.
///         All other Nodes are children of this Node.
///     </para>
///     <para>
///         Is also responsible for registering services used for dependency injection
///     </para>
/// </summary>
public partial class Main : Node, IServicesConfigurator
{
	/// <summary>
	///     Sets up the services needed by the game and registers them with the dependency injection container.
	///     Use this method to register game systems or services, such as game state management, input handling, or other
	///     reusable components.
	/// </summary>
	/// <param name="services">The DI container of the game where services are registered.</param>
	/// <remarks>
	///     Supports property, field and method injection.
	///     Supported service lifetimes are transient, scoped and singleton.
	/// </remarks>
	/// <example>
	///     Register services:
	///     <code>
	/// public void ConfigureServices(IServiceCollection services)
	/// {
	///     // Register Configuration Service
	///     services.AddSingleton&lt;ConfigService&gt;();
	/// 
	///     // Register services that implement interfaces
	///     services.AddTransient&lt;IPlayerRepository,
	///         PlayerRepository&gt;();
	/// 
	///     // Register a database connection context
	///     services.AddDbContext&lt;GameDbContext&gt;(options => 
	///         options.UseSqlServer("connection_string"));
	/// }
	/// </code>
	///     To use a registered service inject the service into your script:
	///     <code>
	/// public class Player : CharacterBody2D
	/// {
	///     [Inject] public ConfigService Configuration;
	/// 
	///     public override void _Ready()
	///     {
	///         this.health = Configuration.PlayerHealth;
	///     }
	/// }
	/// </code>
	/// </example>
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddGodotLogger();
		services.AddTransient<IFileSystem, FileSystem>();
		services.AddSingleton<IConfigLoader, ConfigLoader>();
		services.AddSingleton<IConfigService, ConfigService>();
		services.AddTransient<IColorFetcher, WebSocketColorFetcher>();
		services.AddTransient<ISpaceshipColorService, SpaceshipColorService>();
		services.AddSingleton<IScoreManager, ScoreManager>();
		services.AddDbContext<GameDbContext>(options => options.UseSqlite(connectionString: "Data Source=data.db"));
		services.AddTransient<IGameResultRepository, GameResultRepository>();
	}
}