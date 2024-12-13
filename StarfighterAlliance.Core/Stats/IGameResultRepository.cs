namespace StarfighterAlliance.Core.Stats;

public interface IGameResultRepository
{
	Task<GameResult> AddGameResultAsync(GameResult gameResult);
	Task<List<GameResult>> GetAllAsync();
	Task<List<GameResult>> GetHighScoresAsync(int numberOfEntries);
	Task<GameResult> GetLatestResultAsync();
}