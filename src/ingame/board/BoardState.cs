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