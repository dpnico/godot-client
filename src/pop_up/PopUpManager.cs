using System.Collections.Generic;
using Godot;

namespace GodotClient.PopUp;

/// <summary>
/// Instantiates and manages all pop-ups.
/// </summary>
public partial class PopUpManager : Node
{
    private readonly PackedScene _singleButtonPopUpScene =
        GD.Load<PackedScene>("res://scenes/pop_up/single_button_pop_up.tscn");
    private readonly PackedScene _dualButtonPopUpScene =
        GD.Load<PackedScene>("res://scenes/pop_up/dual_button_pop_up.tscn");
    
    public DualButtonPopUp ExitGamePopUp;

    public override void _Ready()
    {
        ExitGamePopUp = InitializeExitGamePopUp();
    }

    public DualButtonPopUp InitializeExitGamePopUp()
    {
        var popUp = _dualButtonPopUpScene.Instantiate<DualButtonPopUp>();
        popUp.SetHeader("Exit Game");
        popUp.SetContent("Are you sure you want to exit the game?");
        popUp.SetButtonText(new List<string>()
        {
            "Cancel", "Exit"
        });
        AddChild(popUp);
        return popUp;
    }
}