using System;
using System.Collections.Generic;

namespace GodotClient.PopUps;

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

    /// <summary>
    /// Removes the pop-up at the top of the stack.
    /// </summary>
    public void RemovePopUp()
    {
        if (_popUpStack.Count == 0)
        {
            throw new InvalidOperationException("Can't remove a pop-up from an empty stack.");
        }
        var popUp = _popUpStack.Pop();
        ShowMostRecent(true);
        PopUpRemoved?.Invoke(popUp.GetPopUpType());
    }

    /// <summary>
    /// Clears the pop-up stack.
    /// </summary>
    public void ClearStack()
    {
        _popUpStack.Clear();
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