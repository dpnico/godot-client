using Godot;

namespace GodotClient.Ingame.Boards;

/// <summary>
/// Represents a singular player's board state and handles its logic.
/// </summary>
public partial class Board : Control
{
	private readonly Color _opacity30 = Color.FromHtml("#ffffff4d");

	[Export] private TileMapLayer _grid;
	[Export] private ShipLayer _ships;
	[Export] private TileMapLayer _markers;

	/// <summary>
	/// Called when the node enters the scene tree for the first time.
	/// </summary>
	public override void _Ready()
	{
		// For testing purposes, will be removed later
		SetGridModulate(_opacity30);
	}

	/// <summary>
	/// Called every frame. 'delta' is the elapsed time since the previous frame.
	/// </summary>
	/// <param name="delta"></param>
	public override void _Process(double delta)
	{
	}

	/// <summary>
	/// Applies the specified color as modulate of the grid layer.
	/// </summary>
	/// <param name="color"></param>
	public void SetGridModulate(Color color)
	{
		_grid.SetModulate(color);
	}
}
