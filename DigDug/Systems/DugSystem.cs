using System;
using DigDug.Characters;
using DigDug.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DigDug.Systems;

public class DugSystem : System
{
    private readonly Dug _dug;
    private readonly MapSystem _mapSystem;
    
    public DugSystem(Texture2D dugTexture, MapSystem mapSystem)
    {
        _dug = new Dug(new Point(Constants.GAME_WIDTH / 2, Constants.SKY_HEIGHT), new Point(Constants.DUG_DIMENSIONS), dugTexture, 16, 16);
        _mapSystem = mapSystem;
    }
    
    public override void Update(GameTime gameTime)
    {
        _dug.Moving = false;
        if (Keyboard.GetState().IsKeyDown(Keys.Right))
        {
            _dug.Moving = true;
            _dug.FacingDirection = Character.CharacterDirection.Right;
        }
        if (Keyboard.GetState().IsKeyDown(Keys.Left))
        {
            _dug.Moving = true;
            _dug.FacingDirection = Character.CharacterDirection.Left;
        }
        if (Keyboard.GetState().IsKeyDown(Keys.Down))
        {
            _dug.Moving = true;
            _dug.FacingDirection = Character.CharacterDirection.Down;
        }
        if (Keyboard.GetState().IsKeyDown(Keys.Up))
        {
            _dug.Moving = true;
            _dug.FacingDirection = Character.CharacterDirection.Up;
        }
        
        var position = new Point(_dug.Position.X - _dug.Dimension.X / 2, _dug.Position.Y - _dug.Dimension.Y / 2);
        var direction = _dug.FacingDirection switch
        {
            Character.CharacterDirection.Down => MapSystem.HoleDirection.Vertical,
            Character.CharacterDirection.Up => MapSystem.HoleDirection.Vertical,
            Character.CharacterDirection.Left => MapSystem.HoleDirection.Horizontal,
            Character.CharacterDirection.Right => MapSystem.HoleDirection.Horizontal,
            _ => throw new ArgumentOutOfRangeException()
        };
        if(_dug.Moving)
            _dug.Digging = _mapSystem.AddDugOutArea(position, direction);
        _dug.Update(gameTime);
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        _dug.Render(spriteBatch);
    }
}