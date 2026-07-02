using System.Collections.Generic;
using Godot;

namespace GodotClient.PopUps;

/// <summary>
/// Stores the initialization data of a singular pop-up.
/// </summary>
public record PopUpConfig
{
    public required string Header { get; init; }
    public required string Content { get; init; }
    public required IReadOnlyList<string> ButtonText { get; init; }
}

/// <summary>
/// An enum that defines all possible pop-ups.
/// </summary>
public enum PopUpType
{
    EXIT_GAME,
    USERNAME_TOO_LONG
}

/// <summary>
/// Stores the corresponding scene path and config for each pop-up type.
/// </summary>
public static class PopUpMap
{
    private static readonly PackedScene SingleButtonPopUp =
        GD.Load<PackedScene>("res://scenes/pop_up/single_button_pop_up.tscn");
    private static readonly PackedScene DualButtonPopUp =
        GD.Load<PackedScene>("res://scenes/pop_up/dual_button_pop_up.tscn");

    public static readonly IReadOnlyDictionary<PopUpType, PackedScene> Scene =
        new Dictionary<PopUpType, PackedScene>()
        {
            { PopUpType.EXIT_GAME, DualButtonPopUp },
            { PopUpType.USERNAME_TOO_LONG, SingleButtonPopUp },
        };

    public static readonly IReadOnlyDictionary<PopUpType, PopUpConfig> Config =
        new Dictionary<PopUpType, PopUpConfig>()
        {
            {
                PopUpType.EXIT_GAME, new PopUpConfig()
                {
                    Header = "Exit Game",
                    Content = "Are you sure you want to exit the game?",
                    ButtonText = new List<string>() { "Cancel", "Exit" }
                }
            },
            {
                PopUpType.USERNAME_TOO_LONG, new PopUpConfig()
                {
                    Header = "Username Too Long",
                    Content = "The entered username was too long. Your username cannot be longer than 16 characters.",
                    ButtonText = new List<string>() { "OK" }
                }
            }
        };
}