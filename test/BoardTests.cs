using GdUnit4;
using Godot;
using GodotClient.Test.Testing;
using GodotClient.Ingame.Boards;
using static GdUnit4.Assertions;

namespace GodotClient.Test;

[TestSuite]
[RequireGodotRuntime]
public class BoardTests
{
    private ISceneRunner _runner;
    private TestScene _scene;

    private readonly PackedScene _boardScene = GD.Load<PackedScene>("res://scenes/ingame/board/board.tscn");
    private Board _board;

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

    public void SetUp()
    {
        _runner = ISceneRunner.Load("res://scenes/test/test.tscn");
        _scene = _runner.Scene() as TestScene;

        _board = _boardScene.Instantiate<Board>();
        _scene.AddChild(_board);
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
        _board.AddShip(ship);
        var shipLayer = _board.GetShipLayer();
        var ships = _board.GetShips();
        var shipTiles = _board.GetShipTiles();
        var state = _board.GetState();

        AssertThat(ships.Count).IsEqual(1);
        AssertThat(ships[id]).IsEqual(ship);
        AssertThat(shipTiles.Count).IsEqual(2);
        AssertThat(shipTiles[pos]).IsEqual(pos);
        AssertThat(shipTiles[new Vector2I(2, 6)]).IsEqual(pos);
        AssertThat(state[pos]).IsEqual(TileOccupation.OCCUPIED);
        AssertThat(state[new Vector2I(2, 6)]).IsEqual(TileOccupation.OCCUPIED);
        AssertThat(state[new Vector2I(0, 5)]).IsEqual(TileOccupation.BLOCKED);
        AssertThat(state[new Vector2I(3, 7)]).IsEqual(TileOccupation.BLOCKED);
        AssertThat(shipLayer.GetCellAtlasCoords(pos)).IsEqual(_double);

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
        _board.AddShip(ship);
        _board.RemoveShip(id);
        var shipLayer = _board.GetShipLayer();
        var ships = _board.GetShips();
        var shipTiles = _board.GetShipTiles();
        var state = _board.GetState();

        AssertThat(ships.Count).IsEqual(0);
        AssertThat(ships.ContainsKey(id)).IsFalse();
        AssertThat(shipTiles.Count).IsEqual(0);
        AssertThat(state[pos]).IsEqual(TileOccupation.FREE);
        AssertThat(state[new Vector2I(2, 6)]).IsEqual(TileOccupation.FREE);
        AssertThat(state[new Vector2I(0, 5)]).IsEqual(TileOccupation.FREE);
        AssertThat(state[new Vector2I(3, 7)]).IsEqual(TileOccupation.FREE);
        AssertThat(shipLayer.GetCellSourceId(pos)).IsEqual(-1);

        TearDown();
    }

    [TestCase]
    public void RemoveShipAt_PositionIsOrigin()
    {
        SetUp();

        Vector2I pos = new(1, 6);
        Ship ship = new()
        {
            Origin = pos,
            Size = 2,
        };
        var id = ship.ShipId;
        _board.AddShip(ship);
        _board.RemoveShipAt(pos);
        var shipLayer = _board.GetShipLayer();
        var ships = _board.GetShips();
        var shipTiles = _board.GetShipTiles();
        var state = _board.GetState();

        AssertThat(ships.Count).IsEqual(0);
        AssertThat(ships.ContainsKey(id)).IsFalse();
        AssertThat(shipTiles.Count).IsEqual(0);
        AssertThat(state[pos]).IsEqual(TileOccupation.FREE);
        AssertThat(state[new Vector2I(2, 6)]).IsEqual(TileOccupation.FREE);
        AssertThat(state[new Vector2I(0, 5)]).IsEqual(TileOccupation.FREE);
        AssertThat(state[new Vector2I(3, 7)]).IsEqual(TileOccupation.FREE);
        AssertThat(shipLayer.GetCellSourceId(pos)).IsEqual(-1);

        TearDown();
    }

    [TestCase]
    public void RemoveShipAt_PositionIsNotOrigin()
    {
        SetUp();

        Vector2I pos = new(1, 6);
        Ship ship = new()
        {
            Origin = pos,
            Size = 2,
        };
        var id = ship.ShipId;
        _board.AddShip(ship);
        _board.RemoveShipAt(new Vector2I(2, 6));
        var shipLayer = _board.GetShipLayer();
        var ships = _board.GetShips();
        var shipTiles = _board.GetShipTiles();
        var state = _board.GetState();

        AssertThat(ships.Count).IsEqual(0);
        AssertThat(ships.ContainsKey(id)).IsFalse();
        AssertThat(shipTiles.Count).IsEqual(0);
        AssertThat(state[pos]).IsEqual(TileOccupation.FREE);
        AssertThat(state[new Vector2I(2, 6)]).IsEqual(TileOccupation.FREE);
        AssertThat(state[new Vector2I(0, 5)]).IsEqual(TileOccupation.FREE);
        AssertThat(state[new Vector2I(3, 7)]).IsEqual(TileOccupation.FREE);
        AssertThat(shipLayer.GetCellSourceId(pos)).IsEqual(-1);

        TearDown();
    }

    [TestCase]
    public void Rotate_PositiveRotation()
    {
        Ship ship = new()
        {
            Origin = new Vector2I(1, 6),
            Size = 2
        };
        ship.Rotate(2);

        AssertThat(ship.Direction).IsEqual(new Vector2I(-1, 0));
    }

    [TestCase]
    public void Rotate_NegativeRotation()
    {
        Ship ship = new()
        {
            Origin = new Vector2I(1, 6),
            Size = 2
        };
        ship.Rotate(-1);

        AssertThat(ship.Direction).IsEqual(new Vector2I(0, -1));
    }

    [TestCase]
    public void RotateShipAt_RotationPossible()
    {
        SetUp();

        Vector2I pos = new(1, 6);
        Ship ship = new()
        {
            Origin = pos,
            Size = 2
        };
        var id = ship.ShipId;
        _board.AddShip(ship);
        _board.RotateShipAt(pos);
        var shipLayer = _board.GetShipLayer();
        var ships = _board.GetShips();
        var shipTiles = _board.GetShipTiles();
        var state = _board.GetState();

        AssertThat(ships.Count).IsEqual(1);
        AssertThat(ships[id]).IsEqual(ship);
        AssertThat(shipTiles.Count).IsEqual(2);
        AssertThat(shipTiles[pos]).IsEqual(pos);
        AssertThat(shipTiles.ContainsKey(new Vector2I(2, 6))).IsFalse();
        AssertThat(shipTiles[new Vector2I(1, 7)]).IsEqual(pos);
        AssertThat(state[pos]).IsEqual(TileOccupation.OCCUPIED);
        AssertThat(state[new Vector2I(2, 6)]).IsEqual(TileOccupation.BLOCKED);
        AssertThat(state[new Vector2I(1, 7)]).IsEqual(TileOccupation.OCCUPIED);
        AssertThat(state[new Vector2I(0, 5)]).IsEqual(TileOccupation.BLOCKED);
        AssertThat(state[new Vector2I(3, 7)]).IsEqual(TileOccupation.FREE);
        AssertThat(state[new Vector2I(2, 8)]).IsEqual(TileOccupation.BLOCKED);
        AssertThat(shipLayer.GetCellAtlasCoords(pos)).IsEqual(_double);
        AssertThat(shipLayer.GetCellAlternativeTile(pos)).IsEqual(Rotate90);

        TearDown();
    }

    [TestCase]
    public void RotateShipAt_RotationNotPossible()
    {
        SetUp();

        Vector2I pos = new(1, 11);
        Ship ship = new()
        {
            Origin = pos,
            Size = 2
        };
        var id = ship.ShipId;
        _board.AddShip(ship);
        _board.RotateShipAt(pos);
        var shipLayer = _board.GetShipLayer();
        var ships = _board.GetShips();
        var shipTiles = _board.GetShipTiles();
        var state = _board.GetState();

        AssertThat(ships.Count).IsEqual(1);
        AssertThat(ships[id]).IsEqual(ship);
        AssertThat(shipTiles.Count).IsEqual(2);
        AssertThat(shipTiles[pos]).IsEqual(pos);
        AssertThat(shipTiles[new Vector2I(2, 11)]).IsEqual(pos);
        AssertThat(shipTiles.ContainsKey(new Vector2I(1, 12))).IsFalse();
        AssertThat(state[pos]).IsEqual(TileOccupation.OCCUPIED);
        AssertThat(state[new Vector2I(2, 11)]).IsEqual(TileOccupation.OCCUPIED);
        AssertThat(state[new Vector2I(3, 10)]).IsEqual(TileOccupation.BLOCKED);
        AssertThat(shipLayer.GetCellAtlasCoords(pos)).IsEqual(_double);
        AssertThat(shipLayer.GetCellAlternativeTile(pos)).IsEqual(0);

        TearDown();
    }

    public void TearDown()
    {
        _runner.Dispose();
    }
}