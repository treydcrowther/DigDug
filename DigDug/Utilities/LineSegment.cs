using System;
using DigDug.Systems;
using Microsoft.Xna.Framework;

namespace DigDug.Utilities;

public class LineSegment
{
    public Point StartPoint;
    public Point EndPoint;
    public readonly Point MidPoint;
    
    public LineSegment(Point startPoint, Point endPoint)
    {
        StartPoint = startPoint;
        EndPoint = endPoint;
        MidPoint = new Point((startPoint.X + endPoint.X) / 2, (startPoint.Y + endPoint.Y) /2);
    }

    public void ExtendEnd(Point position)
    {
        EndPoint = position;
    }
    
    public void ExtendStart(Point position)
    {
        StartPoint = position;
    }

    public bool IsInMiddle(Point position, MapSystem.HoleDirection direction)
    {
        if(direction == MapSystem.HoleDirection.Horizontal)
            if (position.X > StartPoint.X
                && position.X < EndPoint.X
                && Math.Abs(position.Y - StartPoint.Y) < 50)
                return true;
        if(direction == MapSystem.HoleDirection.Vertical)
            if (position.Y > StartPoint.Y
                && position.Y < EndPoint.Y
                && Math.Abs(position.X - StartPoint.X) < 50)
                return true;
        return false;
    }
    
    public double DistanceFromStart(Point other)
    {
        return Utilities.CalculateDistance(StartPoint, other);
    }

    public double DistanceFromEnd(Point other)
    {
        return Utilities.CalculateDistance(EndPoint, other);
    }
}