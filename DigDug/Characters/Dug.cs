using System;
using DigDug.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DigDug.Characters;

public class Dug : Character
{
    private double _velocity;
    private Point _bounds;
    public bool Digging;
    public Dug(Point position, Point dimension, Texture2D texture, int subImageWidth, int subImageHeight) 
        : base(position, dimension, texture, subImageWidth, subImageHeight)
    {
        _velocity = .25;
        _bounds = new Point(Constants.GAME_WIDTH, Constants.GAME_HEIGHT);
    }

    public override void Update(GameTime gameTime)
    {
        if (!Moving) return;
        var pixels = Convert.ToInt32(gameTime.ElapsedGameTime.Milliseconds * _velocity);

        var translation = FacingDirection switch
        {
            CharacterDirection.Up => new Point(0, -pixels),
            CharacterDirection.Down => new Point(0, pixels),
            CharacterDirection.Left => new Point(-pixels, 0),
            CharacterDirection.Right => new Point(pixels, 0),
            _ => throw new ArgumentOutOfRangeException()
        };
        Translate(translation);
        SubImageRow = Digging ? 1 : 0;
        base.Update(gameTime);
    }
    
    public override void Translate(Point point)
    {
        if (Position.X + point.X > _bounds.X
            || Position.X + point.X  - Dimension.X < 0
            || Position.Y + point.Y > _bounds.Y
            || Position.Y + point.Y < Constants.SKY_HEIGHT)
            return;
        Position.X += point.X;
        Position.Y += point.Y;
    }
    
}