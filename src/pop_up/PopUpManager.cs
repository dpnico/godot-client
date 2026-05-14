using System;
using System.Collections.Generic;
using Godot;

namespace GodotClient.PopUp;

/// <summary>
/// Instantiates and manages all pop-ups.
/// </summary>
public class PopUpManager
{
    private Stack<PopUp> _popUpStack = new Stack<PopUp>();

    public event Action<PopUpType> PopUpRemoved;

    /// <summary>
    /// Instantiates and shows a new pop-up of the specified type.
    /// </summary>
    /// <param name="type"></param>
    public void ShowPopUp(PopUpType type)
    {
        PopUp popUp = null;
        if (PopUpMap.Scene.TryGetValue(type, out var scene))
        {
            popUp = scene.Instantiate<PopUp>();
        }
        else
        {
            throw new ArgumentException($"Pop-up type {type} is not mapped to a scene.");
        }
        popUp.Init(type);
        ShowMostRecent(false);
        popUp.Show(true);
        _popUpStack.Push(popUp);
    }

    /// <summary>
    /// Shows or hides the most recent pop-up on the stack.
    /// </summary>
    /// <param name="show"></param>
    public void ShowMostRecent(bool show)
    {
        var mostRecent = _popUpStack.Peek();
        if (mostRecent != null)
        {
            mostRecent.Show(show);
        }
    }

    public void RemovePopUp(PopUpType type)
    {
        var popUp = _popUpStack.Pop();
        ShowMostRecent(true);
        PopUpRemoved?.Invoke(popUp.GetPopUpType());
    }

    /// <summary>
    /// Returns the stack of active pop-ups.
    /// </summary>
    /// <returns></returns>
    public Stack<PopUp> GetStack()
    {
        return _popUpStack;
    }
}