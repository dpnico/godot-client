using Godot;
using System.Collections.Generic;

namespace GodotClient.Ingame.Boards;

/// <summary>
/// The layer of the board that contains the ships.
/// </summary>
public partial class ShipLayer : TileMapLayer
{
    // Tile atlas constants
    private const int SrcId = 1;
    private readonly Vector2I _single = new Vector2I(3, 1);
    private readonly Vector2I _double = new Vector2I(0, 2);
    private readonly Vector2I _triple = new Vector2I(0, 1);
    private readonly Vector2I _quad = new Vector2I(0, 0);
    // Rotations
    private const int Rotate90 = (int)(TileSetAtlasSource.TransformTranspose | TileSetAtlasSource.TransformFlipH);
    private const int Rotate180 = (int)(TileSetAtlasSource.TransformFlipH | TileSetAtlasSource.TransformFlipV);
    private const int Rotate270 = (int)(TileSetAtlasSource.TransformFlipH | TileSetAtlasSource.TransformTranspose);
    // Board dimensions
    private const int BoardSizeX = 12;
    private const int BoardSizeY = 12;

    private readonly Dictionary<Vector2I, int> _rotations = new();

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        // Initialize rotations with 0
        for (int x = 0; x < BoardSizeX; x++)
        {
            for (int y = 0; y < BoardSizeY; y++)
            {
                _rotations[new Vector2I(x, y)] = 0;
            }
        }

        // For testing purposes, remove later
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                RotateShip(new Vector2I(i, j));
            }
        }
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
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
        _rotations[pos] = (_rotations[pos] + 1) % 4;

        var srcId = GetCellSourceId(pos);
        var coords = GetCellAtlasCoords(pos);
        var rotation = 0;

        switch (_rotations[pos])
        {
            case 1: rotation = Rotate90; break;
            case 2: rotation = Rotate180; break;
            case 3: rotation = Rotate270; break;
        }

        SetCell(pos, srcId, coords, rotation);
    }
}
