using Microsoft.Xna.Framework;

namespace DigDug.Utilities;

public static class Utilities
{
    public static double CalculateDistance(Point one, Point two)
    {
        var x2X1 = two.X - one.X; 
        var y2Y1 = two.Y - one.Y;
        return x2X1 * x2X1 + y2Y1 * y2Y1;
    }
}