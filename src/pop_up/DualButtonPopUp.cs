using Godot;
using System;
using System.Collections.Generic;

namespace GodotClient.PopUp;

/// <summary>
/// A generic pop-up with two buttons for different options.
/// </summary>
public partial class DualButtonPopUp : PopUp
{
	[Export] private Label _header;
	[Export] private RichTextLabel _content;
	[Export] private Button _firstOptionButton;
	[Export] private Button _secondOptionButton;

	public event Action FirstOptionSelected;
	public event Action SecondOptionSelected;
	
	/// <summary>
	/// Called when the node enters the scene tree for the first time.
	/// </summary>
	public override void _Ready()
	{
		_firstOptionButton.Pressed += OnFirstPressed;
	}

	/// <summary>
	/// Called when the first option button is pressed.
	/// </summary>
	public void OnFirstPressed()
	{
		FirstOptionSelected?.Invoke();
		Show(false);
	}

	/// <summary>
	/// Called when the second option button is pressed.
	/// </summary>
	public void OnSecondPressed()
	{
		SecondOptionSelected?.Invoke();
		Show(false);
	}
	
	/// <summary>
	/// Selects the first option if backspace is pressed and the second option
	/// if enter is pressed.
	/// </summary>
	/// <param name="event"></param>
	public override void _Input(InputEvent @event)
	{
		if (!(@event is InputEventKey keyEvent) || !keyEvent.Pressed)
		{
			return;
		}
		if (keyEvent.Keycode == Key.Backspace)
		{
			OnFirstPressed();
		}
		if (keyEvent.Keycode == Key.Enter)
		{
			OnSecondPressed();
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
	/// Sets the text on both option buttons individually.
	/// </summary>
	/// <param name="text"></param>
	public override void SetButtonText(List<string> text)
	{
		_firstOptionButton.Text = text[0];
		_secondOptionButton.Text = text[1];
	}
}
