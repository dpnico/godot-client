using Godot;

namespace GodotClient.Util;

public static class BoardUtil
{
    // Board dimensions
    private const int BoardSizeX = 12;
    private const int BoardSizeY = 12;
    
    public static bool IsWithinBounds(Vector2I pos)
    {
        return pos.X >= 0 && pos.X < BoardSizeX && pos.Y >= 0 & pos.Y < BoardSizeY;
    }
}