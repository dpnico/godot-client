using System;
using System.Threading.Tasks;
using GdUnit4;
using Godot;
using GodotClient.PopUps;
using static GdUnit4.Assertions;

namespace GodotClient.Test.PopUps;

[TestSuite]
[RequireGodotRuntime]
public class PopUpManagerTests
{
    private ISceneRunner _runner;
    private Node _scene;

    private PopUpManager _manager;

    [Before]
    public void SetUp()
    {
        _runner = ISceneRunner.Load("res://scenes/test/test.tscn");
        _scene = _runner.Scene();

        _manager = new PopUpManager(_scene);
    }

    [TestCase]
    public void ShowPopUp()
    {
        _manager.ShowPopUp(PopUpType.EXIT_GAME);
        var stack = _manager.GetStack();
        var popUp = stack.Peek();

        AssertThat(_manager.GetRoot().GetChildCount()).IsEqual(1);
        AssertThat(stack.Count).IsEqual(1);
        AssertThat(popUp.GetPopUpType()).IsEqual(PopUpType.EXIT_GAME);
        AssertThat(popUp.GetHeader().Text).IsEqual("Exit Game");
        AssertThat(popUp.GetContent().Text).IsEqual("Are you sure you want to exit the game?");
        AssertThat(popUp.GetButtons()[1].Text).IsEqual("Exit");
    }

    [TestCase]
    public void RemovePopUp()
    {
        _manager.ShowPopUp(PopUpType.EXIT_GAME);
        _manager.RemovePopUp();

        AssertThat(_manager.GetRoot().GetChildCount()).IsEqual(0);
        AssertThat(_manager.GetStack().Count).IsEqual(0);
    }

    [TestCase]
    [ThrowsException(typeof(InvalidOperationException), "Can't remove a pop-up from an empty stack.")]
    public void RemovePopUpFromEmptyStack()
    {
        _manager.RemovePopUp();
    }

    [TestCase]
    public void ClearStack()
    {
        _manager.ShowPopUp(PopUpType.EXIT_GAME);
        _manager.ShowPopUp(PopUpType.EXIT_GAME);
        _manager.ShowPopUp(PopUpType.EXIT_GAME);
        _manager.ClearStack();

        AssertThat(_manager.GetRoot().GetChildCount()).IsEqual(0);
        AssertThat(_manager.GetStack().Count).IsEqual(0);
    }

    [After]
    public void TearDown()
    {
        _scene.QueueFree();
    }
}