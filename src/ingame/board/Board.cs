using System;
using Godot;

namespace GodotClient.Ingame.Boards;

/// <summary>
/// Represents a singular player's board state and handles its logic.
/// </summary>
public partial class Board : Control
{
    private readonly Color _opacity30 = Color.FromHtml("#ffffff4d");

    [Export] private TileMapLayer _grid;
    [Export] private ShipLayer _shipLayer;
    [Export] private TileMapLayer _markerLayer;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
    }

    /// <summary>
    /// Called every frame. 'delta' is the elapsed time since the previous frame.
    /// </summary>
    /// <param name="delta"></param>
    public override void _Process(double delta)
    {
    }
    
    /// <summary>
    /// Initializes the ship layer with a starter configuration of ships.
    /// </summary>
    public void InitShips()
    {
        var ships = new StartShipConfig();
        foreach (var s in ships)
        {
            _shipLayer.AddShip(s);
        }
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
    
    public void AddShip(Ship ship) 
    {
        _shipLayer.AddShip(ship);
    }

    public void RemoveShip(Guid id)
    {
        _shipLayer.RemoveShip(id);
    }
    
    public void RotateShipAt(Vector2I pos) 
    {
        _shipLayer.RotateShipAt(pos);
    }
    
    public bool CanPlaceShip(Ship ship) 
    {
        return _shipLayer.CanPlace(ship);
    }
    
    public bool TryGetShipAt(Vector2I pos, out Ship ship)
    {
        return _shipLayer.TryGetShipAt(pos, out ship);
    }
    
    public void SetTransparent()
    {
        _grid.SetModulate(_opacity30);
    }
}