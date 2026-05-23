using System;
using GdUnit4;
using Godot;
using GodotClient.Test.Testing;
using GodotClient.Ingame.Boards;
using static GdUnit4.Assertions;

namespace GodotClient.Test;

[TestSuite]
[RequireGodotRuntime]
public class ShipLayerTests
{
    private readonly PackedScene _testScene = GD.Load<PackedScene>("res://scenes/test/test.tscn");
    private TestScene _scene;

    private readonly PackedScene _shipLayerScene = GD.Load<PackedScene>("res://scenes/ingame/board/ship_layer.tscn");
    private ShipLayer _shipLayer;

    public void SetUp()
    {
        _scene = _testScene.Instantiate<TestScene>();

        _shipLayer = _shipLayerScene.Instantiate<ShipLayer>();
        _scene.AddChild(_shipLayer);
    }

    [TestCase]
    public void AddShip()
    {
        SetUp();

        Vector2I pos = new(1, 6);
        Ship ship = new()
        {
            Origin = pos,
            Size = 2
        };
        var id = ship.ShipId;
        _shipLayer.AddShip(ship);
        var ships = _shipLayer.GetShips();
        var shipTiles = _shipLayer.GetShipTiles();
        var state = _shipLayer.GetState();

        AssertThat(ships.Count).IsEqual(1);
        AssertThat(ships[id]).IsEqual(ship);
        AssertThat(shipTiles.Count).IsEqual(2);
        AssertThat(shipTiles[pos]).IsEqual(pos);
        AssertThat(shipTiles[new Vector2I(2, 6)]).IsEqual(pos);
        AssertThat(state[pos]).IsEqual(TileOccupation.OCCUPIED);
        AssertThat(state[new Vector2I(2, 6)]).IsEqual(TileOccupation.OCCUPIED);
        AssertThat(state[new Vector2I(0, 5)]).IsEqual(TileOccupation.BLOCKED);
        AssertThat(state[new Vector2I(3, 7)]).IsEqual(TileOccupation.BLOCKED);

        TearDown();
    }

    [TestCase]
    public void RemoveShip()
    {
        SetUp();
        
        Vector2I pos = new(1, 6);
        Ship ship = new()
        {
            Origin = pos,
            Size = 2,
        };
        var id = ship.ShipId;
        _shipLayer.AddShip(ship);
        _shipLayer.RemoveShip(id);
        var ships = _shipLayer.GetShips();
        var shipTiles = _shipLayer.GetShipTiles();
        var state = _shipLayer.GetState();

        AssertThat(ships.Count).IsEqual(0);
        AssertThat(ships.ContainsKey(id)).IsFalse();
        AssertThat(shipTiles.Count).IsEqual(0);
        AssertThat(state[pos]).IsEqual(TileOccupation.FREE);
        AssertThat(state[new Vector2I(2, 6)]).IsEqual(TileOccupation.FREE);
        AssertThat(state[new Vector2I(0, 5)]).IsEqual(TileOccupation.FREE);
        AssertThat(state[new Vector2I(3, 7)]).IsEqual(TileOccupation.FREE);

        TearDown();
    }

    [TestCase]
    public void RemoveTileAtOrigin()
    {
        SetUp();

        Vector2I pos = new(1, 6);
        Ship ship = new()
        {
            Origin = pos,
            Size = 2,
        };
        var id = ship.ShipId;
        _shipLayer.AddShip(ship);
        _shipLayer.RemoveShipAt(pos);
        var ships = _shipLayer.GetShips();
        var shipTiles = _shipLayer.GetShipTiles();
        var state = _shipLayer.GetState();

        AssertThat(ships.Count).IsEqual(0);
        AssertThat(ships.ContainsKey(id)).IsFalse();
        AssertThat(shipTiles.Count).IsEqual(0);
        AssertThat(state[pos]).IsEqual(TileOccupation.FREE);
        AssertThat(state[new Vector2I(2, 6)]).IsEqual(TileOccupation.FREE);
        AssertThat(state[new Vector2I(0, 5)]).IsEqual(TileOccupation.FREE);
        AssertThat(state[new Vector2I(3, 7)]).IsEqual(TileOccupation.FREE);

        TearDown();
    }

    [TestCase]
    public void RemoveTileAtNotAnOrigin()
    {
        SetUp();

        Vector2I pos = new(1, 6);
        Ship ship = new()
        {
            Origin = pos,
            Size = 2,
        };
        var id = ship.ShipId;
        _shipLayer.AddShip(ship);
        _shipLayer.RemoveShipAt(new Vector2I(2, 6));
        var ships = _shipLayer.GetShips();
        var shipTiles = _shipLayer.GetShipTiles();
        var state = _shipLayer.GetState();

        AssertThat(ships.Count).IsEqual(0);
        AssertThat(ships.ContainsKey(id)).IsFalse();
        AssertThat(shipTiles.Count).IsEqual(0);
        AssertThat(state[pos]).IsEqual(TileOccupation.FREE);
        AssertThat(state[new Vector2I(2, 6)]).IsEqual(TileOccupation.FREE);
        AssertThat(state[new Vector2I(0, 5)]).IsEqual(TileOccupation.FREE);
        AssertThat(state[new Vector2I(3, 7)]).IsEqual(TileOccupation.FREE);

        TearDown();
    }

    public void TearDown()
    {
        _scene.Dispose();
    }
}