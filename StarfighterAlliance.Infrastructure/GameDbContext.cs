using Microsoft.EntityFrameworkCore;
using StarfighterAlliance.Core.Stats;

namespace StarfighterAlliance.Infrastructure;

public sealed class GameDbContext : DbContext
{
	public GameDbContext(DbContextOptions<GameDbContext> options)
		: base(options)
	{
		this.Database.EnsureCreated();
	}

	public DbSet<GameResult> GameResults { get; set; }
}