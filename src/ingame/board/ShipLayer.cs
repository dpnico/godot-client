using System;
using Godot;
using System.Collections.Generic;
using System.ComponentModel;

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
    private const int Rotate90 =(int)(TileSetAtlasSource.TransformTranspose | TileSetAtlasSource.TransformFlipH);
    private const int Rotate180 = (int)(TileSetAtlasSource.TransformFlipH | TileSetAtlasSource.TransformFlipV);
    private const int Rotate270 = (int)(TileSetAtlasSource.TransformTranspose | TileSetAtlasSource.TransformFlipV);
    private readonly Dictionary<int, Vector2I> _direction = new()
    {
        { 0, new Vector2I(1, 0) }, { Rotate90, new Vector2I(0, 1) },
        { Rotate180, new Vector2I(-1, 0) }, { Rotate270, new Vector2I(0, -1) }
    };
    // Board dimensions
    private const int BoardSizeX = 12;
    private const int BoardSizeY = 12;

    private Dictionary<int, Vector2I> _tileAtlasCoords;
    private readonly Dictionary<Guid, > _position = new();
    private readonly Dictionary<Vector2I, int> _rotation = new();
    private readonly Dictionary<Vector2I, TileOccupation> _state = new();

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        // Store atlas coords of ships
        _tileAtlasCoords = new()
        {
            { 1, _single }, { 2, _double }, { 3, _triple }, { 4, _quad }
        };
        
        // Initialize rotations with 0
        for (int x = 0; x < BoardSizeX; x++)
        {
            for (int y = 0; y < BoardSizeY; y++)
            {
                _rotation[new Vector2I(x, y)] = 0;
                _state[new Vector2I(x, y)] = TileOccupation.FREE; // Temporary, remove later
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
    /// Called when there is an input event. The input event propagates up
    /// through the node tree until a node consumes it.
    /// </summary>
    /// <param name="event"></param>
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
    /// Adds a ship of the specified size at the specified position and with
    /// the specified rotation.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="shipSize"></param>
    /// <param name="rotation"></param>
    public void AddShip(Vector2I pos, int shipSize, int rotation)
    {
        if (!CanPlace(pos, shipSize, rotation))
        {
            return;
        }
        SetCell(pos, SrcId, _tileAtlasCoords[shipSize], rotation);
        for (int i = 0; i < shipSize; i++)
        { 
            var tilesToBlock = GetNeighbors(pos + _direction[rotation]); 
            foreach (var t in tilesToBlock) 
            { 
                _state[t] = TileOccupation.BLOCKED;
            }
        } 
        for (int i = 0; i < shipSize; i++) 
        { 
            _state[pos + _direction[rotation]] = TileOccupation.OCCUPIED;
            // Store ship
        }
    }

    /// <summary>
    /// Returns true if a ship with the specified size and rotation can be
    /// placed at the specified position.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="shipSize"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public bool CanPlace(Vector2I pos, int shipSize, int rotation)
    {
        var canPlace = true;
        for (int i = 2; i < shipSize; i++)
        {
            if (_state[pos + i * _direction[rotation]] != TileOccupation.FREE)
            {
                return false;
            }
        }
        // Currently doesn't work for ships with size 2, implement later
        return canPlace;
    }
    
    /// <summary>
    /// Removes the ship at the specified position.
    /// </summary>
    /// <param name="pos"></param>
    public void RemoveShip(Vector2I pos) 
    {
         // Implement later
    }

    /// <summary>
    /// Rotates the ship at the specified position 90 degrees to the right.
    /// </summary>
    /// <param name="pos"></param>
    public void RotateShip(Vector2I pos)
    {
        var atlasCoords = GetCellAtlasCoords(pos);
        if (!_tileAtlasCoords.ContainsKey(atlasCoords))
        {
            return;
        }
        var srcId = GetCellSourceId(pos);
        var nextRotation = (_rotation[pos] + 1) % 4;
        var rotation = 0;
        switch (nextRotation)
        {
            case 1: rotation = Rotate90; break;
            case 2: rotation = Rotate180; break;
            case 3: rotation = Rotate270; break;
        }
        Vector2I direction;
        switch (rotation) 
        {
            case 0: direction = new Vector2I(1, 0); break;
            case Rotate90: direction = new Vector2I(0, 1); break;
            case Rotate180: direction = new Vector2I(-1, 0); break;
            case Rotate270: direction = new Vector2I(0, -1); break;
            default: throw new ArgumentException($"Invalid rotation: {rotation}.");
        }
        if (!CheckRotationPossible(pos, _tileAtlasCoords[atlasCoords], rotation)) 
        {
            return;
        }
        _rotation[pos] = nextRotation;
        SetCell(pos, srcId, atlasCoords, rotation);
        FreeTilesOnRotation(pos, _tileAtlasCoords[atlasCoords], rotation);
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
    /// Frees previously occupied and blocked tiles that are no longer occupied
    /// or blocked when a ship is rotated.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="shipSize"></param>
    /// <param name="rotation"></param>
    /// <exception cref="ArgumentException"></exception>
    public void FreeTilesOnRotation(Vector2I pos, int shipSize, int rotation)
    {
        Vector2I direction = _direction[rotation];
        var perp = new Vector2I(direction.Y, direction.X);
        for (int i = 1; i < shipSize; i++)
        {
            _state[pos + i * direction] = TileOccupation.FREE; // Occupied tiles
            
            TryFree(pos + (i + 1) * direction - perp); // Tiles beside the ship
            TryFree(pos + (i + i) * direction + perp);
        }
        TryFree(pos + shipSize * direction); // Tile at the end of the ship
    }

    /// <summary>
    /// Frees the tile at the specified position if it is within the board and
    /// not blocked by any remaining adjacent occupied tiles.
    /// </summary>
    /// <param name="pos"></param>
    public void TryFree(Vector2I pos)
    {
        if (_state.ContainsKey(pos) && !IsBlocked(pos))
        {
            _state[pos] = TileOccupation.FREE;
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
            if (_state[n] == TileOccupation.OCCUPIED)
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
    /// Returns the next rotation.
    /// </summary>
    /// <param name="rotation"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public int GetNextRotation(int rotation)
    {
        switch (rotation) 
        {
            case 0: return Rotate90; break;
            case Rotate90: return Rotate180; break;
            case Rotate180: return Rotate270; break;
            case Rotate270: return 0; break;
            default: throw new ArgumentException($"Invalid rotation: {rotation}.");
        }
    }
    
    /// <summary>
    /// Returns the size of the ship represented by the tile at the specified
    /// atlas coords.
    /// </summary>
    /// <param name="coords"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public int GetSizeByAtlasCoords(Vector2I coords) 
    {
        foreach (var ship in _tileAtlasCoords) 
        {
            if (coords == ship.Value)
            {
                return ship.Key;
            }
        }
        throw new ArgumentException($"Invalid atlas coords: {coords}.");
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