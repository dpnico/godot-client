using Godot;
using System;
using GodotClient.Util;
using GodotClient.Ingame.Boards;

namespace GodotClient.Ingame;

public partial class SetupScreen : Control
{
	[Export] private Board _shipPreview;
	[Export] private Board _shipPlacement;
	
	private (Board, Vector2I) _dragOrigin;
	private (Board, Vector2I) _dragDest;
	
	/// <summary>
	/// Called when the node enters the scene tree for the first time.
	/// </summary>
	public override void _Ready()
	{
	}

	/// <summary>
	/// Called every frame. 'delta' is the elapsed time since the previous frame.
	/// </summary>
	/// <param name="delta"></param>
	public override void _Process(double delta)
	{
	}
	
	/// <summary>
	/// Called when there is an input event. The input event propagates up
	/// through the node tree until a node consumes it.
	/// </summary>
	/// <param name="event"></param>
	public override void _Input(InputEvent @event) 
	{
		if (@event is InputEventMouseButton mbEvent && mbEvent.ButtonIndex == MouseButton.Left)
		{
			var previewPos = _shipPreview.LocalToMap(GetGlobalMousePosition());
			var placementPos = _shipPlacement.LocalToMap(GetGlobalMousePosition());
			
			if (mbEvent.Pressed)
			{
				HandleLeftMouseButtonPress(previewPos, placementPos);
			}
			else
			{
				HandleLeftMouseButtonRelease(previewPos, placementPos);
			}
		}
	}

	public void HandleLeftMouseButtonPress(Vector2I previewPos, Vector2I placementPos)
	{
		if (BoardUtil.IsWithinBounds(previewPos))
		{
			StartDragAction(_shipPreview, previewPos);
			return;
		}
		if (BoardUtil.IsWithinBounds(placementPos))
		{
			StartDragAction(_shipPlacement, placementPos);
		}
	}

	public void StartDragAction(Board board, Vector2I pos)
	{
		if (!board.TryGetOrigin(pos, out var shipOrigin))
		{
			return;
		}
		_dragOrigin = (board, shipOrigin);
		// Continue
	}
	
	public void HandleLeftMouseButtonRelease(Vector2I previewPos, Vector2I placementPos)
	{
		if (BoardUtil.IsWithinBounds(previewPos))
		{
			TryDrop(_shipPreview, previewPos);
			return;
		}
		if (BoardUtil.IsWithinBounds(placementPos))
		{
			TryDrop(_shipPlacement, placementPos);
		}
	}

	public void TryDrop(Board board, Vector2I pos)
	{
		// Check if ship can be placed at release position
		// If true, remove from origin, then add at new position
	}
}
