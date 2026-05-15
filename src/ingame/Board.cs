using Godot;
using System;

namespace GodotClient.Ingame;

/// <summary>
/// Represents a singlular player's board state and handles its logic.
/// </summary>
public partial class Board : Node2D
{
    [Export] private TileMapLayer _grid;
    [Export] private TileMapLayer _ships;
    [Export] private TileMapLayer _markers;

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
}
