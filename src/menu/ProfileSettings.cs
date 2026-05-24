using Godot;
using System;
using GodotClient.PopUps;
using Microsoft.CodeAnalysis.Differencing;

namespace GodotClient.Menu;

public partial class ProfileSettings : PanelContainer
{
    [Export] private TextureRect _profileIcon;
    [Export] private Label _username;
    [Export] private LineEdit _usernameEdit;
    [Export] private Button _usernameEditButton;
    
    private bool _editingUsername = false;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        ActivateControl(_usernameEdit, false);

        _usernameEditButton.Pressed += OnUsernameEditButtonPressed;
    }

    /// <summary>
    /// Called every frame. 'delta' is the elapsed time since the previous frame.
    /// </summary>
    /// <param name="delta"></param>
    public override void _Process(double delta)
    {
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mb && mb.Pressed) 
        {
            var pos = GetGlobalMousePosition();
            var hoveredControl = GetViewport().GuiGetHoveredControl();
            if (_editingUsername)
            {
                if (hoveredControl != _usernameEditButton && hoveredControl != _usernameEdit)
                {
                    GD.Print("Editing terminated early.");
                    TerminateEditingUsername();
                }
            }
        }
    }

    public void OnUsernameEditButtonPressed()
    {
        if (_editingUsername)
        {
            TrySaveUsername();
            TerminateEditingUsername();
        }
        else
        {
            _editingUsername = true;
            ActivateControl(_usernameEdit, true);
            _usernameEdit.GrabFocus();
            _usernameEditButton.Text = "Save";
        }
    }

    public void TrySaveUsername()
    {
        var newName = _usernameEdit.Text.Trim();
        if (newName.Length > 0)
        {
            if (newName.Length <= 16)
            {
                _username.Text = newName;
            }
            else
            {
                PopUpManager.Instance.ShowPopUp(PopUpType.USERNAME_TOO_LONG);
            }
        }
    }
    
    public void TerminateEditingUsername() 
    {
        ActivateControl(_usernameEdit, false);
        _usernameEdit.Text = "";
        _usernameEdit.PlaceholderText = _username.Text;
        _usernameEditButton.Text = "Edit";
        _editingUsername = false;
    }

    public void ActivateControl(Control ctrl, bool active)
    {
        ctrl.SetVisible(active);
        var processMode = active ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
        ctrl.SetProcessMode(processMode);
        var filter = active ? MouseFilterEnum.Stop : MouseFilterEnum.Ignore;
        ctrl.SetMouseFilter(filter);
    }
}
