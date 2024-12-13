using Godot;
using StarfighterAlliance.Core.Stats;

namespace StarfighterAlliance.Scenes.EndScreen;

public partial class HighscoreEntry : Control
{
	/// <summary>
	///     Initializes this Highscore Entry Component with the specified data.
	/// </summary>
	/// <param name="result">The data of the game that resulted in this highscore</param>
	/// <param name="scorePosition">The ranking number of this highscore</param>
	/// <param name="isLatestResult">If this was the result of the latest game</param>
	public void InitializeWithData(GameResult result, int scorePosition, bool isLatestResult)
	{
		if (isLatestResult)
		{
			this.GetNode<Control>(path: "Selection").Visible = true;
		}

		this.GetNode<Label>(path: "Properties/Score Position").Text = scorePosition.ToString();
		this.GetNode<Label>(path: "Properties/Score Amount").Text = result.FinalScore.ToString();
		this.GetNode<Label>(path: "Properties/Wave Count").Text = result.WaveCount.ToString();
		this.GetNode<Label>(path: "Properties/Spaceship Type").Text = result.UsedSpaceship;
		this.GetNode<Label>(path: "Properties/Timestamp").Text = result.GameEndedAt.ToString(format: "dd.MM.yy HH:mm");
	}
}
