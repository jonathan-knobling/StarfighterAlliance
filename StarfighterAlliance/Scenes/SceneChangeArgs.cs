using Godot;

namespace StarfighterAlliance.Scenes;

/// <summary>
///     Provides arguments for scene change events,
///     including a reference to the controller of the scene being replaced.
/// </summary>
public class SceneChangeArgs
{
	/// <summary>
	///     Initializes a new instance of the <see cref="SceneChangeArgs" /> class.
	///     Also makes sure the <see cref="Node" /> of the old scene is not null.
	/// </summary>
	/// <param name="oldScene">The ndoe of the scene that is being replaced.</param>
	public SceneChangeArgs(Node oldScene)
	{
		OldScene = oldScene;
	}

	/// <summary>
	///     Gets the controller of the scene that is being replaced during a scene change.
	/// </summary>
	public Node OldScene { get; }
}