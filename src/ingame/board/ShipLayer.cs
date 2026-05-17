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
    private const int Rotate270 = (int)(TileSetAtlasSource.TransformTranspose | TileSetAtlasSource.TransformFlipV);
    // Board dimensions
    private const int BoardSizeX = 12;
    private const int BoardSizeY = 12;

    private Dictionary<Vector2I, int> _shipSizes;
    private readonly Dictionary<Vector2I, int> _rotations = new();

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        // Store atlas coords of ships
        _shipSizes = new()
        {
            { _single, 1 }, { _double, 2 }, { _triple, 3 }, { _quad, 4 }
        };
        
        // Initialize rotations with 0
        for (int x = 0; x < BoardSizeX; x++)
        {
            for (int y = 0; y < BoardSizeY; y++)
            {
                _rotations[new Vector2I(x, y)] = 0;
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
    
    public override void _Input(InputEvent @event) 
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed) 
        {
            if (keyEvent.Keycode == Key.R)
            {
                RotateAllShips();
            }
        }
    }

    /// <summary>
    /// Rotates the ship at the specified position 90 degrees to the right.
    /// </summary>
    /// <param name="pos"></param>
    public void RotateShip(Vector2I pos)
    {
        var nextRotation = (_rotations[pos] + 1) % 4;

        var srcId = GetCellSourceId(pos);
        var atlasCoords = GetCellAtlasCoords(pos);
        var rotation = 0;

        switch (nextRotation)
        {
            case 1: rotation = Rotate90; break;
            case 2: rotation = Rotate180; break;
            case 3: rotation = Rotate270; break;
        }
        
        if (!CheckRotationPossible(pos, atlasCoords, rotation)) 
        {
            return;
        }

        _rotations[pos] = nextRotation;
        SetCell(pos, srcId, atlasCoords, rotation);
    }
    
    /// <summary>
    /// Returns true if a rotation of the ship is possible and false if it is not.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="atlasCoords"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public bool CheckRotationPossible(Vector2I pos, Vector2I atlasCoords, int rotation)
    {
        var possible = false;
        if (_shipSizes.TryGetValue(atlasCoords, out var size))
        {
            GD.Print($"Size of ship {atlasCoords} at pos {pos}: {size}");
            switch(rotation) 
            {
                case 0: possible = pos.X + size - 1 <= BoardSizeX - 1; break;
                case Rotate90: possible = pos.Y + size - 1 <= BoardSizeY - 1; break;
                case Rotate180: possible = pos.X + 1 >= size; break;
                case Rotate270: possible = pos.Y + 1 >= size; break;
            }
        }
        return possible;
    }
    
    /// <summary>
    /// For debugging purposes. Rotates all ships.
    /// </summary>
    public void RotateAllShips() 
    {
        for (int i = 0; i < BoardSizeX; i++)
        {
            for (int j = 0; j < BoardSizeY; j++) 
            { 
                RotateShip(new Vector2I(i, j));
            }
        }
    }
}
