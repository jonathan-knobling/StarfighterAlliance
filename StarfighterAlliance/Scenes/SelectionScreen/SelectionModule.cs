using Godot;
using StarfighterAlliance.Core.Config;

namespace StarfighterAlliance.Scenes.SelectionScreen;

public partial class SelectionModule : Control
{
	private Label attack = null!;
	private Label health = null!;
	private TextureRect icon = null!;
	private Label nameLabel = null!;
	private Label speed = null!;

	public override void _Ready()
	{
		icon = this.GetNode<TextureRect>(path: "Icon");
		nameLabel = this.GetNode<Label>(path: "Name");

		speed = this.GetNode<Label>(path: "Stats/Speed Number");
		attack = this.GetNode<Label>(path: "Stats/Attack Number");
		health = this.GetNode<Label>(path: "Stats/Health Number");
	}

	public void SetSpaceshipData(SpaceshipTypeDto? spaceshipType)
	{
		if (spaceshipType is null)
		{
			return;
		}

		speed.Text = spaceshipType.Speed.ToString();
		attack.Text = spaceshipType.AttackDamage.ToString();
		health.Text = spaceshipType.Health.ToString();
		nameLabel.Text = spaceshipType.Name;

		Image? image = Image.LoadFromFile($"Scenes/SelectionScreen/{spaceshipType.SerializedName}Icon.png");
		var texture = ImageTexture.CreateFromImage(image);
		icon.SetTexture(texture);
	}
}