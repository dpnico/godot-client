using Godot;
using System;

namespace GodotClient.Menu;

/// <summary>
/// The main menu with Start, Settings, Help and Exit button. It is the first
/// scene that gets loaded when the game is started.
/// </summary>
public partial class MainMenu : Control
{
    [Export] private Button _startButton;
    [Export] private Button _settingsButton;
    [Export] private Button _helpButton;
    [Export] private Button _exitButton;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        // _startButton.Pressed += ...
        // _settingsButton.Pressed += ...
        // _helpButton.Pressed += 
        _exitButton.Pressed += () => GetTree().Quit();
    }

    /// <summary>
    /// Called every frame. 'delta' is the elapsed time since the previous frame.
    /// </summary>
    /// <param name="delta"></param>
    public override void _Process(double delta)
    {
    }
}
