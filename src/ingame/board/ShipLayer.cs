using Godot;
using System;

/// <summary>
/// The layer of the board that contains the ships.
/// </summary>
public partial class ShipLayer : TileMapLayer
{
	/// <summary>
	/// Called when the node enters the scene tree for the first time.
	/// </summary>
	public override void _Ready()
	{
		// For testing purposes, remove later
		for (int i = 0; i < 12; i++) 
		{
			for (int j = 0; j < 12; j++) 
			{
				RotateShip(new Vector2I(i, j));
			}
		}
	}

	/// <summary>
	/// Called every frame. 'delta' is the elapsed time since the previous frame.
	/// </summary>
	/// <param name="delta"></param>
	public override void _Process(double delta)
	{
	}
	
	/// <summary>
	/// Rotates the ship at the specified position 90 degrees to the right.
	/// </summary>
	/// <param name="pos"></param>
	public void RotateShip(Vector2I pos)
	{
		var srcId = GetCellSourceId(pos);
		var coords = GetCellAtlasCoords(pos);
		
		SetCell(pos, srcId, coords, (int)(TileSetAtlasSource.TransformTranspose | TileSetAtlasSource.TransformFlipH));
	}
}
