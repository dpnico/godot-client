using Godot;
using System;

/// <summary>
/// Represents a singlular player's board state and handles its logic.
/// </summary>
public partial class Board : Node2D
{
	[Export] private TileMapLayer _grid;
	[Export] private TileMapLayer _ships;
	[Export] private TileMapLayer _markers;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
