using System;
using GdUnit4;
using Godot;
using GodotClient.PopUps;
using GodotClient.Test.Testing;
using static GdUnit4.Assertions;

namespace GodotClient.Test.PopUps;

[TestSuite]
[RequireGodotRuntime]
public class PopUpManagerTests
{
    private ISceneRunner _runner;
    private TestScene _scene;

    private readonly PopUpManager _manager = PopUpManager.Instance;

    public void SetUp()
    {
        _runner = ISceneRunner.Load("res://scenes/test/test.tscn");
        _scene = _runner.Scene() as TestScene;
        
        _manager.SetRoot(_scene);
    }

    [TestCase]
    public void ShowPopUp()
    {
        SetUp();

        _manager.ShowPopUp(PopUpType.EXIT_GAME);
        var stack = _manager.GetStack();
        var popUp = stack.Peek();

        AssertThat(_manager.GetRoot().GetChildCount()).IsEqual(1);
        AssertThat(stack.Count).IsEqual(1);
        AssertThat(popUp.GetPopUpType()).IsEqual(PopUpType.EXIT_GAME);
        AssertThat(popUp.GetHeader().Text).IsEqual("Exit Game");
        AssertThat(popUp.GetContent().Text).IsEqual("Are you sure you want to exit the game?");
        AssertThat(popUp.GetButtons()[1].Text).IsEqual("Exit");

        TearDown();
    }

    [TestCase]
    public void RemovePopUp()
    {
        SetUp();

        _manager.ShowPopUp(PopUpType.EXIT_GAME);
        AssertThat(_manager.GetRoot().GetChildCount()).IsEqual(1);
        _manager.RemovePopUp();

        AssertThat(_manager.GetRoot().GetChildCount()).IsEqual(0);
        AssertThat(_manager.GetStack().Count).IsEqual(0);

        TearDown();
    }

    [TestCase]
    [ThrowsException(typeof(InvalidOperationException), "Can't remove a pop-up from an empty stack.")]
    public void RemovePopUp_EmptyStack()
    {
        SetUp();

        _manager.RemovePopUp();
    }

    [TestCase]
    public void ClearStack()
    {
        SetUp();

        _manager.ShowPopUp(PopUpType.EXIT_GAME);
        _manager.ShowPopUp(PopUpType.EXIT_GAME);
        _manager.ShowPopUp(PopUpType.EXIT_GAME);
        _manager.ClearStack();

        AssertThat(_manager.GetRoot().GetChildCount()).IsEqual(0);
        AssertThat(_manager.GetStack().Count).IsEqual(0);

        TearDown();
    }

    public void TearDown()
    {
        _runner.Dispose();
        _manager.ClearStack();
    }
}