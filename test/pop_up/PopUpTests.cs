using System.Collections.Generic;
using GdUnit4;
using Godot;
using GodotClient.PopUps;
using GodotClient.Test.Testing;
using static GdUnit4.Assertions;

namespace GodotClient.Test.PopUps;

[TestSuite]
[RequireGodotRuntime]
public class PopUpTests
{
    private readonly PackedScene _testScene = GD.Load<PackedScene>("res://scenes/test/test.tscn");
    private TestScene _scene;

    private static readonly string SingleButtonPopUpPath = "res://scenes/pop_up/single_button_pop_up.tscn";
    private static readonly string DualButtonPopUpPath = "res://scenes/pop_up/dual_button_pop_up.tscn";
    private readonly PackedScene _singleButtonPopUpScene = GD.Load<PackedScene>(SingleButtonPopUpPath);
    private readonly PackedScene _dualButtonPopUpScene = GD.Load<PackedScene>(DualButtonPopUpPath);
    private PopUp _singleButtonPopUp;
    private PopUp _dualButtonPopUp;

    [Before]
    public void SetUp()
    {
        _scene = _testScene.Instantiate<TestScene>();

        _singleButtonPopUp = _singleButtonPopUpScene.Instantiate<PopUp>();
        _dualButtonPopUp = _dualButtonPopUpScene.Instantiate<PopUp>();
        _scene.AddChild(_singleButtonPopUp);
        _scene.AddChild(_dualButtonPopUp);
    }

    [TestCase]
    public void SetHeader()
    {
        _singleButtonPopUp.SetHeader("Test");

        AssertThat(_singleButtonPopUp.GetHeader().Text).IsEqual("Test");
    }

    [TestCase]
    public void SetContent()
    {
        _singleButtonPopUp.SetContent("This is a test");

        AssertThat(_singleButtonPopUp.GetContent().Text).IsEqual("This is a test");
    }

    [TestCase]
    public void SetButtonTextSingle()
    {
        _singleButtonPopUp.SetButtonText(new List<string>() { "Button 1" });

        AssertThat(_singleButtonPopUp.GetButtons()[0].Text).IsEqual("Button 1");
    }

    [TestCase]
    public void SetButtonTextDual()
    {
        _dualButtonPopUp.SetButtonText(new List<string>() { "Button 1", "Button 2" });

        AssertThat(_dualButtonPopUp.GetButtons()[0].Text).IsEqual("Button 1");
        AssertThat(_dualButtonPopUp.GetButtons()[1].Text).IsEqual("Button 2");
    }

    [TestCase]
    public void Init()
    {
        _dualButtonPopUp.Init(PopUpType.EXIT_GAME);

        AssertThat(_dualButtonPopUp.GetPopUpType()).IsEqual(PopUpType.EXIT_GAME);
        AssertThat(_dualButtonPopUp.GetHeader().Text).IsEqual("Exit Game");
        AssertThat(_dualButtonPopUp.GetContent().Text).IsEqual("Are you sure you want to exit the game?");
        AssertThat(_dualButtonPopUp.GetButtons()[0].Text).IsEqual("Cancel");
        AssertThat(_dualButtonPopUp.GetButtons()[1].Text).IsEqual("Exit");
    }

    [TestCase]
    public void Show()
    {
        _singleButtonPopUp.Show(true);

        AssertThat(_singleButtonPopUp.IsVisible()).IsTrue();
        AssertThat(_singleButtonPopUp.GetMouseFilter()).IsEqual(Control.MouseFilterEnum.Stop);
        AssertThat(_singleButtonPopUp.GetProcessMode()).IsEqual(Node.ProcessModeEnum.Inherit);
    }

    [TestCase]
    public void Hide()
    {
        _singleButtonPopUp.Show(false);

        AssertThat(_singleButtonPopUp.IsVisible()).IsFalse();
        AssertThat(_singleButtonPopUp.GetMouseFilter()).IsEqual(Control.MouseFilterEnum.Ignore);
        AssertThat(_singleButtonPopUp.GetProcessMode()).IsEqual(Node.ProcessModeEnum.Disabled);
    }

    [TestCase]
    public void SceneMap()
    {
        var exitGame = PopUpMap.Scene[PopUpType.EXIT_GAME];

        AssertThat(exitGame.ResourcePath).Contains(DualButtonPopUpPath);
    }

    [TestCase]
    public void ConfigMap()
    {
        var exitGame = PopUpMap.Config[PopUpType.EXIT_GAME];

        AssertThat(exitGame.Header).IsEqual("Exit Game");
        AssertThat(exitGame.Content).IsEqual("Are you sure you want to exit the game?");
        AssertThat(exitGame.ButtonText.Count).IsEqual(2);
        AssertThat(exitGame.ButtonText[0]).IsEqual("Cancel");
        AssertThat(exitGame.ButtonText[1]).IsEqual("Exit");
    }

    [After]
    public void TearDown()
    {
        _scene.QueueFree();
    }
}