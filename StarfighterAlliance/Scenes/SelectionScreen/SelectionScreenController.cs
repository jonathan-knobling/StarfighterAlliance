using System.Collections.Generic;
using Godot;
using Godot.DependencyInjection.Attributes;
using StarfighterAlliance.Core;
using StarfighterAlliance.Core.Config;
using StarfighterAlliance.Core.Spaceship.Color;

namespace StarfighterAlliance.Scenes.SelectionScreen;

public partial class SelectionScreenController : CanvasLayer
{
	private readonly Cycle<SpaceshipTypeDto> spaceshipTypes = new();

	private SelectionModule selectionModuleLeft = new();
	private SelectionModule selectionModuleMid = new();
	private SelectionModule selectionModuleRight = new();
	[Inject] public IConfigService ConfigService { get; set; } = null!;
	[Inject] public ISpaceshipColorService ColorService { get; set; } = null!;

	public event SceneChangeHandler? SpaceshipSelected;

	public override void _Ready()
	{
		var selectButton = this.GetNode<Button>(path: "Select Button");
		selectButton.ButtonUp += OnSelectButtonPressed;

		selectionModuleLeft = this.GetNode<SelectionModule>(path: "Selection Module Left");
		selectionModuleMid = this.GetNode<SelectionModule>(path: "Selection Module Mid");
		selectionModuleRight = this.GetNode<SelectionModule>(path: "Selection Module Right");

		ConfigService.LoadConfig();
		IEnumerable<SpaceshipTypeDto> types = ConfigService.GetSpaceshipTypes();
		spaceshipTypes.AddRange(types);

		ApplySpaceshipDataToModules();
		Callable.From(LoadColorAsync).CallDeferred();
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent is not InputEventMouseButton mouseEvent)
		{
			return;
		}

		if (mouseEvent.ButtonIndex == MouseButton.WheelUp && mouseEvent.Pressed)
		{
			spaceshipTypes.CycleLeft();
			ApplySpaceshipDataToModules();
		}
		else if (mouseEvent.ButtonIndex == MouseButton.WheelDown && mouseEvent.Pressed)
		{
			spaceshipTypes.CycleRight();
			ApplySpaceshipDataToModules();
		}
	}

	private void ApplySpaceshipDataToModules()
	{
		selectionModuleLeft.SetSpaceshipData(spaceshipTypes.Previous);
		selectionModuleMid.SetSpaceshipData(spaceshipTypes.Current);
		selectionModuleRight.SetSpaceshipData(spaceshipTypes.Next);
	}

	/// <summary>
	///     Load spaceship color in the background before game is started
	/// </summary>
	private async void LoadColorAsync()
	{
		ConfigService.LoadedColor = await ColorService.GetColor();
	}

	private void OnSelectButtonPressed()
	{
		ConfigService.SelectedSpaceshipType = spaceshipTypes.Current;

		SpaceshipSelected?.Invoke(new SceneChangeArgs(this));
	}
}