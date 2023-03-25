using System;
using DigDug.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DigDug.Characters;

public abstract class Character
{
    public Point Position;
    public Point Dimension;
    public bool Moving;
    private readonly Texture2D _texture;
    private readonly int _subImageWidth;
    private readonly int _subImageHeight;
    protected int SubImageRow;
    private bool _animationUp;
    private readonly TimeSpan _animationTime;
    private TimeSpan _elapsedAnimationTime;
    public CharacterDirection FacingDirection = CharacterDirection.Up; 

    public enum CharacterDirection
    {
        Right,
        Up,
        Left,
        Down
    }

    protected Character(Point position, Point dimension, Texture2D texture, int subImageWidth, int subImageHeight)
    {
        _texture = texture;
        Dimension = dimension;
        Position = position;
        _subImageWidth = subImageWidth;
        _subImageHeight = subImageHeight;
        _animationTime = new TimeSpan(0, 0, 0, 0, 75);
        FacingDirection = CharacterDirection.Right;
    }

    public virtual void Translate(Point point)
    {
        Position.X += point.X;
        Position.Y += point.Y;
    }

    public virtual void Update(GameTime gameTime)
    {
        _elapsedAnimationTime += gameTime.ElapsedGameTime;
        if (_elapsedAnimationTime > _animationTime)
        {
            _elapsedAnimationTime -= _animationTime;
            _animationUp = !_animationUp;
        }
    }

    public void Render(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        var stuff = Convert.ToInt16(_animationUp);
        var index = (int)FacingDirection * _subImageWidth * 2 + _subImageWidth * stuff;
        var row = SubImageRow * _subImageHeight;
        spriteBatch.Draw(_texture,
            new Rectangle(Position.X - Dimension.X / 2, Position.Y - Dimension.Y / 2, Dimension.X, Dimension.Y),
            new Rectangle(index, row, _subImageWidth, _subImageHeight),
            Color.White,
            0,
            new Vector2(_subImageWidth / 2, _subImageHeight / 2),
            SpriteEffects.None,
            0);
        spriteBatch.End();
    }
}