using System;
using System.Collections.Generic;
using Godot;

namespace GodotClient.PopUp;

/// <summary>
/// A generic pop-up.
/// </summary>
public partial class PopUp : Control
{
    [Export] private Label _header;
    [Export] private RichTextLabel _content;
    [Export] private Button[] _buttons;

    private PopUpType _type;

    public event Action<int> ButtonPressed;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        for (int i = 0; i < _buttons.Length; i++)
        {
            _buttons[i].Pressed += () => ButtonPressed?.Invoke(i);
        }
    }

    /// <summary>
    /// Initializes the pop-up with header, content and button text based on
    /// the specified type.
    /// </summary>
    /// <param name="type"></param>
    public void Init(PopUpType type)
    {
        _type = type;
        if (PopUpMap.Config.TryGetValue(type, out var config))
        {
            SetHeader(config.Header);
            SetContent(config.Content);
            SetButtonText(config.ButtonText);
        }
        else
        {
            throw new ArgumentException($"Pop-up type {type} is not mapped to a config.");
        }
    }

    /// <summary>
    /// Sets the header of the pop-up.
    /// </summary>
    /// <param name="header"></param>
    public void SetHeader(string header)
    {
        _header.Text = header;
    }

    /// <summary>
    /// Sets the content of the pop-up.
    /// </summary>
    /// <param name="content"></param>
    public void SetContent(string content)
    {
        _content.Text = content;
    }

    /// <summary>
    /// Sets the text on both option buttons individually.
    /// </summary>
    /// <param name="text"></param>
    public void SetButtonText(IEnumerable<string> text)
    {
        var textList = new List<string>();
        foreach (var t in text)
        {
            textList.Add(t);
        }
        for (int i = 0; i < textList.Count; i++)
        {
            _buttons[i].Text = textList[i];
        }
    }

    /// <summary>
    /// Shows or hides the pop-up and (de)activates its functionality.
    /// </summary>
    /// <param name="show"></param>
    public virtual void Show(bool show)
    {
        if (show)
        {
            GrabFocus();
        }
        else
        {
            GetViewport().GuiReleaseFocus();
        }
        SetVisible(show);
        var filter = show ? MouseFilterEnum.Stop : MouseFilterEnum.Ignore;
        SetMouseFilter(filter);
        var processMode = show ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
        SetProcessMode(processMode);
    }

    /// <summary>
    /// Returns the type of the pop-up as an enum value.
    /// </summary>
    /// <returns></returns>
    public PopUpType GetPopUpType()
    {
        return _type;
    }

    /// <summary>
    /// Returns the pop-up header.
    /// </summary>
    /// <returns></returns>
    public Label GetHeader()
    {
        return _header;
    }

    /// <summary>
    /// Returns the pop-up content.
    /// </summary>
    /// <returns></returns>
    public RichTextLabel GetContent()
    {
        return _content;
    }

    /// <summary>
    /// Returns the pop-up buttons.
    /// </summary>
    /// <returns></returns>
    public Button[] GetButtons()
    {
        return _buttons;
    }
}