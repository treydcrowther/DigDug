using System.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DigDug.Systems;

public abstract class System
{
    public abstract void Update(GameTime gameTime);
    public abstract void Render(SpriteBatch spriteBatch);
}