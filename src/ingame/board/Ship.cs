using System;
using System.Collections.Generic;
using Godot;

namespace GodotClient.Ingame.Boards;

/// <summary>
/// Stores all relevant data of a ship.
/// </summary>
public class Ship
{
    public Guid ShipId { get; set; } = Guid.NewGuid();
    public Vector2I Origin { get; set; }
    public int Size { get; set; }
    public Vector2I Direction { get; set; } = new Vector2I(1, 0);

    /// <summary>
    /// Changes the direction of the ship according to the amount of rotations.
    /// Negative rotations are allowed.
    /// </summary>
    /// <param name="amt"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Rotate(int amt)
    {
        if (amt == 0)
        {
            return;
        }
        if (amt < 0)
        {
            var posAmt = Math.Abs(amt) % 4;
            switch (posAmt)
            {
                case 1: amt = 3; break;
                case 3: amt = 1; break;
            }
        }
        else
        {
            amt = amt % 4;
        }
        int prev;
        switch (Direction)
        {
            case (1, 0): prev = 0; break;
            case (0, 1): prev = 1; break;
            case (-1, 0): prev = 2; break;
            case (0, -1): prev = 3; break;
            default: throw new InvalidOperationException($"Can't rotate ship because its direction is invalid: {Direction}.");
        }
        int next = (prev + amt) % 4;
        switch (next)
        {
            case 0: Direction = new Vector2I(1, 0); break;
            case 1: Direction = new Vector2I(0, 1); break;
            case 2: Direction = new Vector2I(-1, 0); break;
            case 3: Direction = new Vector2I(0, -1); break;
        }
    }
}

/// <summary>
/// A starter configuration of ships on a board. Features 1 size 4 ship, 2 size
/// 3 ships, 3 size 2 ships and 4 size 1 ships.
/// </summary>
public class StartShipConfig : HashSet<Ship>
{
    public StartShipConfig()
    {
        Add(new()
        {
            Origin = new Vector2I(1, 1),
            Size = 4
        });
        Add(new()
        {
            Origin = new Vector2I(6, 1),
            Size = 3
        });
        Add(new()
        {
            Origin = new Vector2I(1, 3),
            Size = 3
        });
        Add(new()
        {
            Origin = new Vector2I(5, 3),
            Size = 2
        });
        Add(new()
        {
            Origin = new Vector2I(8, 3),
            Size = 2
        });
        Add(new()
        {
            Origin = new Vector2I(1, 5),
            Size = 2
        });
        Add(new()
        {
            Origin = new Vector2I(4, 5),
            Size = 1
        });
        Add(new()
        {
            Origin = new Vector2I(6, 5),
            Size = 1
        });
        Add(new()
        {
            Origin = new Vector2I(8, 5),
            Size = 1
        });
        Add(new()
        {
            Origin = new Vector2I(10, 5),
            Size = 1
        });
    }
}