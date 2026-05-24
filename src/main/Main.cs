using Godot;
using System;
using GodotClient.PopUps;

/// <summary>
/// The main class and entry point of the program.
/// </summary>
public partial class Main : Node
{
    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        PopUpManager.Instance.SetRoot(this);
    }

    /// <summary>
    /// Called every frame. 'delta' is the elapsed time since the previous frame.
    /// </summary>
    /// <param name="delta"></param>
    public override void _Process(double delta)
    {
    }
}
