using System.Collections.Generic;
using Godot;

namespace GodotClient.PopUp;

/// <summary>
/// An abstract pop-up class.
/// </summary>
public abstract partial class PopUp : Control
{
    public abstract void SetHeader(string header);
    public abstract void SetContent(string content);
    public abstract void SetButtonText(List<string> text);
    
    /// <summary>
    /// Shows or hides the pop-up and (de)activates its functionality.
    /// </summary>
    /// <param name="visible"></param>
    public virtual void Show(bool visible)
    {
        if (visible)
        {
            GrabFocus();
        }
        else
        {
            GetViewport().GuiReleaseFocus();
        }
        SetVisible(visible);
        var filter = visible ? MouseFilterEnum.Stop : MouseFilterEnum.Ignore;
        SetMouseFilter(filter);
        var processMode = visible ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
        SetProcessMode(processMode);
    }
}