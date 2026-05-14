using Godot;
using System;
using System.Collections.Generic;

namespace GodotClient.PopUp;

/// <summary>
/// A generic pop-up with a single button.
/// </summary>
public partial class SingleButtonPopUp : PopUp
{
    [Export] private Label _header;
    [Export] private RichTextLabel _content;
    [Export] private Button _button;

    public event Action ButtonPressed;
	
    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        _button.Pressed += OnButtonPressed;
    }

    /// <summary>
    /// Called when the button is pressed.
    /// </summary>
    public void OnButtonPressed()
    {
        ButtonPressed?.Invoke();
        Show(false);
    }
    
    /// <summary>
    /// Hides the pop-up if backspace or enter is pressed.
    /// </summary>
    /// <param name="event"></param>
    public override void _Input(InputEvent @event)
    {
        if (!(@event is InputEventKey keyEvent) || !keyEvent.Pressed)
        {
            return;
        }
        if (keyEvent.Keycode == Key.Backspace || keyEvent.Keycode == Key.Enter)
        {
            OnButtonPressed();
        }
    }
    
    /// <summary>
    /// Sets the header of the pop-up.
    /// </summary>
    /// <param name="header"></param>
    public override void SetHeader(string header)
    {
        _header.Text = header;
    }

    /// <summary>
    /// Sets the content of the pop-up.
    /// </summary>
    /// <param name="content"></param>
    public override void SetContent(string content)
    {
        _content.Text = content;
    }

    /// <summary>
    /// Sets the text on the button.
    /// </summary>
    /// <param name="text"></param>
    public override void SetButtonText(List<string> text)
    {
        _button.Text = text[0];
    }
}