using System;
using System.Collections.Generic;
using Godot;

namespace GodotClient.Util;

public static class BoardUtil
{
    // Ship tile atlas constants
    private static readonly Vector2I Single = new(3, 1);
    private static readonly Vector2I Double = new(0, 2);
    private static readonly Vector2I Triple = new(0, 1);
    private static readonly Vector2I Quad = new(0, 0);
    // Rotations
    private const int Rotate90 = (int)(TileSetAtlasSource.TransformTranspose | TileSetAtlasSource.TransformFlipH);
    private const int Rotate180 = (int)(TileSetAtlasSource.TransformFlipH | TileSetAtlasSource.TransformFlipV);
    private const int Rotate270 = (int)(TileSetAtlasSource.TransformTranspose | TileSetAtlasSource.TransformFlipV);
    // Board dimensions
    private const int BoardSizeX = 12;
    private const int BoardSizeY = 12;
    
    public static bool IsWithinBounds(Vector2I pos)
    {
        return pos.X >= 0 && pos.X < BoardSizeX && pos.Y >= 0 & pos.Y < BoardSizeY;
    }
    
    public static List<Vector2I> GetNeighbors(Vector2I pos)
    {
        var neighbors = new List<Vector2I>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                var neighbor = new Vector2I(pos.X + x, pos.Y + y);
                if (neighbor.X < 0 || neighbor.X >= BoardSizeX || neighbor.Y < 0 || neighbor.Y >= BoardSizeY ||
                    x == 0 && y == 0)
                {
                    continue;
                }
                neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }

    public static Vector2I GetOffsetFromOrigin(Vector2I origin, Vector2I pos)
    {
        return pos - origin;
    }
    
    public static int GetRotation(Vector2I direction)
    {
        switch (direction)
        {
            case (1, 0): return 0;
            case (0, 1): return Rotate90;
            case (-1, 0): return Rotate180;
            case (0, -1): return Rotate270;
            default: throw new ArgumentException($"{direction} is not a valid direction.");
        }
    }
    
    public static Vector2I GetAtlasCoords(int shipSize)
    {
        switch (shipSize)
        {
            case 1: return Single;
            case 2: return Double;
            case 3: return Triple;
            case 4: return Quad;
            default: throw new ArgumentException($"Invalid ship size: {shipSize}.");
        }
    }
}