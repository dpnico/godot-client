using System;
using System.Collections.Generic;
using Godot;
using GodotClient.Util;

namespace GodotClient.Ingame.Boards;

/// <summary>
/// Represents a singular player's board state and handles its logic.
/// </summary>
public partial class Board : Control
{
    [Export] private TileMapLayer _grid;
    [Export] private TileMapLayer _shipLayer;
    [Export] private TileMapLayer _markerLayer;

    private readonly Color _opacity30 = Color.FromHtml("#ffffff4d");
    private readonly Color _opacity65 = Color.FromHtml("#ffffffa6");

    // Tile atlas constants
    private const int ShipsSrcId = 1;
    // Board dimensions
    private const int BoardSizeX = 12;
    private const int BoardSizeY = 12;

    private readonly Dictionary<Guid, Ship> _ships = new();
    private readonly Dictionary<Vector2I, Vector2I> _shipTiles = new();
    private readonly Dictionary<Vector2I, TileOccupation> _shipLayerState = new();

    public event Action RandomPressed;
    public event Action ReadyPressed;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        _grid.SetModulate(_opacity65);
        for (int x = 0; x < BoardSizeX; x++)
        {
            for (int y = 0; y < BoardSizeY; y++)
            {
                _shipLayerState[new Vector2I(x, y)] = TileOccupation.FREE;
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
                _shipLayerState[ship.Origin + i * ship.Direction + j * perp] = TileOccupation.BLOCKED;
            }
        }
        for (int i = 0; i < ship.Size; i++)
        {
            var pos = ship.Origin + i * ship.Direction;
            _shipLayerState[pos] = TileOccupation.OCCUPIED;
            _shipTiles[pos] = ship.Origin;
        }
        _shipLayer.SetCell(ship.Origin, ShipsSrcId, BoardUtil.GetAtlasCoords(ship.Size),
            BoardUtil.GetRotation(ship.Direction));
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
            var shipTile = ship.Origin + i * ship.Direction;
            _shipLayerState[shipTile] = TileOccupation.FREE;
            _shipTiles.Remove(shipTile);
        }
        for (int i = -1; i <= ship.Size; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                var posToFree = ship.Origin + i * ship.Direction + j * perp;
                if (_shipLayerState.ContainsKey(posToFree) && !IsBlocked(posToFree))
                {
                    _shipLayerState[posToFree] = TileOccupation.FREE;
                }
            }
        }
        _shipLayer.SetCell(ship.Origin, -1);
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
        if (CanPlaceShip(ship))
        {
            AddShip(ship);
        }
        else
        {
            ship.Rotate(-1);
            AddShip(ship);
        }
    }

    public bool CanPlaceShip(Ship ship)
    {
        if (!FitsOnBoard(ship))
        {
            return false;
        }
        for (int i = 0; i < ship.Size; i++)
        {
            if (_shipLayerState[ship.Origin + i * ship.Direction] != TileOccupation.FREE)
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

    public bool IsBlocked(Vector2I pos)
    {
        foreach (var n in BoardUtil.GetNeighbors(pos))
        {
            if (_shipLayerState[n] == TileOccupation.OCCUPIED)
            {
                return true;
            }
        }
        return false;
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

    public void SetTransparent()
    {
        _grid.SetModulate(_opacity30);
    }

    /// <summary>
    /// Returns the map coordinates of the cell containing the given
    /// localPosition. If localPosition is in global coordinates, consider
    /// using ToLocal(Vector2) before passing it to this method. See also
    /// MapToLocal(Vector2I).
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Vector2I LocalToMap(Vector2 pos)
    {
        return _grid.LocalToMap(_grid.ToLocal(pos));
    }

    public TileMapLayer GetGrid()
    {
        return _grid;
    }

    public TileMapLayer GetShipLayer()
    {
        return _shipLayer;
    }

    public TileMapLayer GetMarkerLayer()
    {
        return _markerLayer;
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
        return _shipLayerState;
    }
}