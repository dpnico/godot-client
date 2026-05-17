using Godot;
using System.Collections.Generic;

namespace GodotClient.Ingame.Boards;

/// <summary>
/// An enum that represents whether a tile is free, occupied by a ship or
/// blocked from occupation by an adjacent occupied tile.
/// </summary>
public enum TileOccupation 
{
    FREE,
    OCCUPIED,
    BLOCKED
}

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
    private readonly Dictionary<Vector2I, int> _positions = new();
    private readonly Dictionary<Vector2I, int> _rotations = new();
    private readonly Dictionary<Vector2I, TileOccupation> _states = new();

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
                _states[new Vector2I(x, y)] = TileOccupation.FREE; // Temporary, remove later
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
        var atlasCoords = GetCellAtlasCoords(pos);
        if (!_shipSizes.ContainsKey(atlasCoords))
        {
            return;
        }
        var srcId = GetCellSourceId(pos);
        var nextRotation = (_rotations[pos] + 1) % 4;
        var rotation = 0;
        switch (nextRotation)
        {
            case 1: rotation = Rotate90; break;
            case 2: rotation = Rotate180; break;
            case 3: rotation = Rotate270; break;
        }
        if (!CheckRotationPossible(pos, _shipSizes[atlasCoords], rotation)) 
        {
            return;
        }
        _rotations[pos] = nextRotation;
        SetCell(pos, srcId, atlasCoords, rotation);
        UpdateTileData(pos, _shipSizes[atlasCoords], rotation);
    }
    
    /// <summary>
    /// Returns true if a rotation of the ship is possible and false if it is not.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="shipSize"></param>
    /// <param name="rotation"></param>
    /// <returns></returns> 
    public bool CheckRotationPossible(Vector2I pos, int shipSize, int rotation)
    {
        var possible = false;
        switch(rotation) 
        {
            case 0: possible = pos.X + shipSize - 1 <= BoardSizeX - 1; break;
            case Rotate90: possible = pos.Y + shipSize - 1 <= BoardSizeY - 1; break;
            case Rotate180: possible = pos.X + 1 >= shipSize; break;
            case Rotate270: possible = pos.Y + 1 >= shipSize; break;
        }
        return possible;
    }

    /// <summary>
    /// Updates occupied and blocked tiles.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="shipSize"></param>
    /// <param name="rotation"></param>
    public void UpdateTileData(Vector2I pos, int shipSize, int rotation)
    {
        switch (rotation) 
        {
            case 0:
                for (int i = 1; i < shipSize; i++)
                {
                    // Free no longer occupied tiles
                    _states[new Vector2I(pos.X + i, pos.Y)] = TileOccupation.FREE;
                    // Check if previously blocked tiles in rows above and below are still blocked
                    var topRight = new Vector2I(pos.X + i + 1, pos.Y - 1);
                    if (_states.ContainsKey(topRight) && !IsBlocked(topRight))
                    {
                        _states[topRight] = TileOccupation.FREE;
                    }
                    var bottomRight = new Vector2I(pos.X + i + 1, pos.Y + 1);
                    if (_states.ContainsKey(bottomRight) && !IsBlocked(bottomRight))
                    {
                        _states[bottomRight] = TileOccupation.FREE;
                    }
                }
                // Check if previously blocked tile all the way to the right is still blocked
                var right = new Vector2I(pos.X + shipSize, pos.Y);
                if (_states.ContainsKey(right) && !IsBlocked(right))
                {
                    _states[right] = TileOccupation.FREE;
                }
                // Block new tiles
                for (int i = 0; i < shipSize; i++)
                {
                    var tilesToBlock = GetNeighbors(new Vector2I(pos.X, pos.Y + i));
                    foreach (var t in tilesToBlock)
                    {
                        _states[t] = TileOccupation.BLOCKED;
                    }
                }
                // Occupy new tiles
                for (int i = 0; i < shipSize; i++)
                {
                    _states[new Vector2I(pos.X, pos.Y + i)] = TileOccupation.OCCUPIED;
                }
                break;
            case Rotate90:
                break;
            case Rotate180:
                break;
            case Rotate270:
                break;
        }
    }
    
    /// <summary>
    /// Returns true if the tile at the specified position is blocked from
    /// occupation by an adjacent occupied tile and false if it is not.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool IsBlocked(Vector2I pos) 
    {
        foreach (var n in GetNeighbors(pos))
        {
            if (_states[n] == TileOccupation.OCCUPIED)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns all valid neighbors of the specified tile.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public List<Vector2I> GetNeighbors(Vector2I pos)
    {
        var neighbors = new List<Vector2I>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                var neighbor = new Vector2I(pos.X + x, pos.Y + y);
                if (neighbor.X < 0 || neighbor.X >= BoardSizeX || neighbor.Y < 0 || neighbor.Y >= BoardSizeY ||
                    x == 0 && y == 0)
                {
                    continue;
                }
                neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }
    
    /// <summary>
    /// For debugging purposes. Rotates all ships when 'R' is pressed.
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