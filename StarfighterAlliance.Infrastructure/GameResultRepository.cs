using Microsoft.EntityFrameworkCore;
using StarfighterAlliance.Core.Stats;

namespace StarfighterAlliance.Infrastructure;

public class GameResultRepository : IGameResultRepository
{
	private readonly GameDbContext context;

	public GameResultRepository(GameDbContext context)
	{
		this.context = context;
	}

	public async Task<GameResult> AddGameResultAsync(GameResult gameResult)
	{
		await context.GameResults.AddAsync(gameResult);
		await context.SaveChangesAsync();

		return gameResult;
	}

	public async Task<List<GameResult>> GetAllAsync()
	{
		return await context.GameResults.ToListAsync();
	}

	public async Task<List<GameResult>> GetHighScoresAsync(int numberOfEntries)
	{
		return await context.GameResults
							.OrderByDescending(x => x.FinalScore)
							.Take(numberOfEntries)
							.ToListAsync();
	}

	public async Task<GameResult> GetLatestResultAsync()
	{
		return await context.GameResults
							.OrderByDescending(x => x.GameEndedAt)
							.FirstAsync();
	}
}