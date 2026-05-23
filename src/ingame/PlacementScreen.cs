using Godot;
using System;
using GodotClient.Util;
using GodotClient.Ingame.Boards;

namespace GodotClient.Ingame;

public partial class PlacementScreen : Control
{
	[Export] private Button _randomButton;
	[Export] private Button _readyButton;
	[Export] private Board _shipPreview;
	[Export] private Board _shipPlacement;
	[Export] private Sprite2D _dragAndDropPreview;
	
	private (Board, Vector2I) _dragOrigin;
	private Ship _shipBeingDragged;
	private Vector2I? _dragOffset;
	
	/// <summary>
	/// Called when the node enters the scene tree for the first time.
	/// </summary>
	public override void _Ready()
	{
		_shipPreview.SetTransparent();
		_shipPreview.InitShips();
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
		if (@event is InputEventMouseButton mbEvent)
		{
			var previewPos = _shipPreview.LocalToMap(GetGlobalMousePosition());
			var placementPos = _shipPlacement.LocalToMap(GetGlobalMousePosition());
			if (mbEvent.ButtonIndex == MouseButton.Left)
			{
				if (mbEvent.Pressed)
				{
					HandleLeftMouseButtonPress(previewPos, placementPos);
				}
				else
				{
					HandleLeftMouseButtonRelease(previewPos, placementPos);
				}
			}
			if (mbEvent.ButtonIndex == MouseButton.Right && mbEvent.Pressed)
			{
				HandleRightMouseButtonPress(previewPos, placementPos);
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
	
	public void HandleRightMouseButtonPress(Vector2I previewPos, Vector2I placementPos)
	{
		if (BoardUtil.IsWithinBounds(previewPos))
		{
			_shipPreview.RotateShipAt(previewPos);
			return;
		}
		if (BoardUtil.IsWithinBounds(placementPos))
		{
			_shipPlacement.RotateShipAt(placementPos);
		}
	}

	public void StartDragAction(Board board, Vector2I pos)
	{
		if (!board.TryGetShipAt(pos, out var ship))
		{
			return;
		}
		board.RemoveShip(ship.ShipId);
		_dragOrigin = (board, ship.Origin);
		_shipBeingDragged = ship;
		_dragOffset = BoardUtil.GetOffsetFromOrigin(ship.Origin, pos);
		// Show ship preview while dragging
	}

	public void TryDrop(Board board, Vector2I pos)
	{
		if (_shipBeingDragged == null)
		{
			return;
		}
		var offset = _dragOffset.HasValue ? _dragOffset.Value : new Vector2I(0, 0);
		Ship shipAtNewOrigin = new()
		{
			ShipId = _shipBeingDragged.ShipId,
			Origin = pos - offset,
			Size = _shipBeingDragged.Size,
			Direction = _shipBeingDragged.Direction
		};
		if (board.CanPlaceShip(shipAtNewOrigin))
		{
            board.AddShip(shipAtNewOrigin);
		}
		else
		{
			_dragOrigin.Item1.AddShip(_shipBeingDragged);
		}
		_shipBeingDragged = null;
		_dragOffset = null;
	}
}
