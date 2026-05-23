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
/// The layer of the board that contains and visualizes the ships.
/// </summary>
public partial class ShipLayer : TileMapLayer
{
    // Tile atlas constants
    private const int SrcId = 1;
    private readonly Vector2I _single = new(3, 1);
    private readonly Vector2I _double = new(0, 2);
    private readonly Vector2I _triple = new(0, 1);
    private readonly Vector2I _quad = new(0, 0);
    // Rotations
    private const int Rotate90 = (int)(TileSetAtlasSource.TransformTranspose | TileSetAtlasSource.TransformFlipH);
    private const int Rotate180 = (int)(TileSetAtlasSource.TransformFlipH | TileSetAtlasSource.TransformFlipV);
    private const int Rotate270 = (int)(TileSetAtlasSource.TransformTranspose | TileSetAtlasSource.TransformFlipV);
    // Board dimensions
    private const int BoardSizeX = 12;
    private const int BoardSizeY = 12;

    private readonly Dictionary<Guid, Ship> _ships = new();
    private readonly Dictionary<Vector2I, Vector2I> _shipTiles = new();
    private readonly Dictionary<Vector2I, TileOccupation> _state = new();
    
    public bool Debug = false;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        Debug = true;
        for (int x = 0; x < BoardSizeX; x++)
        {
            for (int y = 0; y < BoardSizeY; y++)
            {
                _state[new Vector2I(x, y)] = TileOccupation.FREE;
            }
        }
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
            if (keyEvent.Keycode == Key.I)
            {
                InitShips();
            }
            if (keyEvent.Keycode == Key.D)
            {
                RemoveAllShips();
            }
            if (keyEvent.Keycode == Key.R)
            {
                RotateAllShips();
            }
        }
    }

    /// <summary>
    /// Initializes the ship layer with a starter configuration of ships.
    /// </summary>
    public void InitShips()
    {
        var ships = new StartShipConfig();
        foreach (var s in ships)
        {
            AddShip(s);
        }
    }

    public void AddShip(Ship ship)
    {
        if (_ships.ContainsKey(ship.ShipId))
        {
            throw new ArgumentException($"A ship with the ID {ship.ShipId} is already registered.");
        }
        var perp = new Vector2I(ship.Direction.Y, ship.Direction.X);
        for (int i = -1; i <= ship.Size; i++)
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
        SetCell(ship.Origin, SrcId, GetAtlasCoords(ship.Size), GetRotation(ship.Direction));
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
        GD.Print("RemoveShipAt is called.");
        if (!TryGetShipAt(pos, out Ship ship))
        {
            return null;
        }
        var perp = new Vector2I(ship.Direction.Y, ship.Direction.X);
        for (int i = 0; i < ship.Size; i++)
        {
            var shipTile = ship.Origin + i * ship.Direction;
            _state[shipTile] = TileOccupation.FREE;
            _shipTiles.Remove(shipTile);
        }
        for (int i = -1; i <= ship.Size; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                TryFree(ship.Origin + i * ship.Direction + j * perp);
            }
        }
        SetCell(ship.Origin, -1);
        _ships.Remove(ship.ShipId);
        return ship;
    }

    public void RotateShipAt(Vector2I pos)
    {
        var ship = RemoveShipAt(pos);
        if (ship == null)
        {
            return;
        }
        ship.Rotate(1);
        if (CanPlace(ship))
        {
            AddShip(ship);
        }
        else
        {
            ship.Rotate(-1);
            AddShip(ship);
        }
    }

    public bool CanPlace(Ship ship)
    {
        if (!FitsOnBoard(ship))
        {
            return false;
        }
        for (int i = 0; i < ship.Size; i++)
        {
            if (_state[ship.Origin + i * ship.Direction] != TileOccupation.FREE)
            {
                return false;
            }
        }
        return true;
    }

    public bool FitsOnBoard(Ship ship)
    {
        switch (ship.Direction)
        {
            case (1, 0): return ship.Origin.X + ship.Size - 1 <= BoardSizeX - 1;
            case (0, 1): return ship.Origin.Y + ship.Size - 1 <= BoardSizeY - 1;
            case (-1, 0): return ship.Origin.X + 1 >= ship.Size;
            case (0, -1): return ship.Origin.Y + 1 >= ship.Size;
            default: throw new ArgumentException($"Invalid direction: {ship.Size}.");
        }
    }

    public void TryFree(Vector2I pos)
    {
        GD.Print("TryFree is called.");
        if (_state.ContainsKey(pos) && !IsBlocked(pos))
        {
            _state[pos] = TileOccupation.FREE;
        }
    }

    public bool IsBlocked(Vector2I pos)
    {
        GD.Print("IsBlocked is called.");
        foreach (var n in GetNeighbors(pos))
        {
            if (_state[n] == TileOccupation.OCCUPIED)
            {
                return true;
            }
        }
        return false;
    }

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

    public Vector2I GetAtlasCoords(int shipSize)
    {
        switch (shipSize)
        {
            case 1: return _single;
            case 2: return _double;
            case 3: return _triple;
            case 4: return _quad;
            default: throw new ArgumentException($"Invalid ship size: {shipSize}.");
        }
    }

    /// <summary>
    /// For debugging purposes. Removes all ships.
    /// </summary>
    public void RemoveAllShips()
    {
        Dictionary<Guid, Ship> shipCopies = new();
        foreach (var s in _ships)
        {
            shipCopies[s.Key] = s.Value;
        }
        foreach (var s in shipCopies)
        {
            RemoveShipAt(s.Value.Origin);
        }
    }

    /// <summary>
    /// For debugging purposes. Rotates all ships.
    /// </summary>
    public void RotateAllShips()
    {
        Dictionary<Guid, Ship> shipCopies = new();
        foreach (var s in _ships)
        {
            shipCopies[s.Key] = s.Value;
        }
        foreach (var s in shipCopies)
        {
            RotateShipAt(s.Value.Origin);
        }
    }

    /// <summary>
    /// Returns the dictionary of ships.
    /// </summary>
    /// <returns></returns>
    public Dictionary<Guid, Ship> GetShips()
    {
        return _ships;
    }

    /// <summary>
    /// Returns the dictionary of ship tiles.
    /// </summary>
    /// <returns></returns>
    public Dictionary<Vector2I, Vector2I> GetShipTiles()
    {
        return _shipTiles;
    }

    /// <summary>
    /// Returns the dictionary of board tile states.
    /// </summary>
    /// <returns></returns>
    public Dictionary<Vector2I, TileOccupation> GetState()
    {
        return _state;
    }
}