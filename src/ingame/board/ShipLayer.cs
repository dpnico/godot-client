using System;
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
/// Stores all relevant data of a ship.
/// </summary>
public class Ship
{
    public Guid ShipId;
    public Vector2I Origin;
    public int Size;
    public Vector2I Direction;
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
    // Board dimensions
    private const int BoardSizeX = 12; 
    private const int BoardSizeY = 12;

    private readonly Dictionary<Guid, Ship> _ships = new();
    private readonly Dictionary<Vector2I, Vector2I> _shipTiles = new();
    private readonly Dictionary<Vector2I, TileOccupation> _state = new();

    public void AddShip(Ship ship)
    {
        var perp = new Vector2I(ship.Direction.Y, ship.Direction.X);
        for (int i =  -1; i <= ship.Size; i++) 
        {
            for (int j = -1; j <= 1; j++)
            {
                _state[ship.Origin + i * ship.Direction + j * perp] = TileOccupation.BLOCKED;
            }
        }
        for (int i = 0; i < ship.Size; i++)
        {
            var pos = ship.Origin + i * ship.Direction;
            _state[pos] = TileOccupation.OCCUPIED;
            _shipTiles[pos] = ship.Origin;
        }
        _ships[ship.ShipId] = ship;
    }
    
    public Ship RemoveShip(Guid shipId) 
    {
        if (!_ships.ContainsKey(shipId))
        {
            throw new ArgumentException($"Ship with ID {shipId} does not exist.");
        }
        return RemoveShipAt(_ships[shipId].Origin);
    }

    public Ship RemoveShipAt(Vector2I pos)
    {
        if (!TryGetShipAt(pos, out Ship ship))
        {
            return null;
        }
        var perp = new Vector2I(ship.Direction.Y, ship.Direction.X);
        for (int i = 0; i < ship.Size; i++)
        {
            _state[ship.Origin + i * ship.Direction] = TileOccupation.FREE;
        }
        for (int i = -1; i <= ship.Size; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                TryFree(ship.Origin + i * ship.Direction + j * perp);
            }
        }
        return ship;
    }

    public void RotateShipAt(Vector2I pos)
    {
        var ship = RemoveShipAt(pos);
        if (ship == null)
        {
            return;
        }
        ship. = GetNextRotation(ship.Direction);
        
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

    public bool TryGetShipAt(Vector2I pos, out Ship ship)
    {
        if (!_shipTiles.ContainsKey(pos))
        {
            ship = null;
            return false;
        }
        foreach (var s in _ships)
        {
            if (s.Value.Origin.Equals(_shipTiles[pos]))
            {
                ship = s.Value;
                return true;
            }
        }
        throw new ArgumentException($"Inconsistent data: A ship is registered at {pos}, but is not mapped to an origin.");
    }

    public int GetRotation(Vector2I direction)
    {
        switch (direction)
        {
            case (1, 0): return 0;
            case (0, 1): return Rotate90;
            case (-1, 0): return Rotate180;
            case (0, -1): return Rotate270;
            default: throw new ArgumentException($"{direction} is not a valid direction.");
        }
    }
    
    public int GetNextRotation(Vector2I direction) 
    {
        switch (direction)
        {
            case (1, 0): return Rotate90;
            case (0, 1): return Rotate180;
            case (-1, 0): return Rotate270;
            case (0, -1): return 0;
            default: throw new ArgumentException($"{direction} is not a valid direction.");
        }
    }
}