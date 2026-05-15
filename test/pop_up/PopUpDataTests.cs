using GdUnit4;
using Godot;
using GodotClient.PopUp;
using static GdUnit4.Assertions;

namespace GodotClient.Test.PopUp;

[TestSuite]
public class PopUpDataTests
{
    [TestCase]
    public void PopUpSceneMapTest()
    {
        var singleButtonPopUpScene = GD.Load<PackedScene>("res://scenes/pop_up/single_button_pop_up.tscn");
        var dualButtonPopUpScene = GD.Load<PackedScene>("res://scenes/pop_up/dual_button_pop_up.tscn");
        
        var exitGame = PopUpMap.Scene[PopUpType.EXIT_GAME];

        AssertThat(exitGame).IsEquivalentTo(dualButtonPopUpScene);
    }
}