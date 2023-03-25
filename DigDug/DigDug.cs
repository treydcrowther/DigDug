using System.Collections.Generic;
using DigDug.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DigDug
{
    public class DigDug : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private List<Systems.System> _systems = new();

        public DigDug()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferHeight = 1920;
            _graphics.PreferredBackBufferWidth = 1380;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            var dug = Content.Load<Texture2D>("Images/Dug");
            var tiles = new Dictionary<string, Texture2D>();
            tiles.Add("Orange", Content.Load<Texture2D>("Images/orange"));
            tiles.Add("Yellow", Content.Load<Texture2D>("Images/yellow"));
            tiles.Add("Red", Content.Load<Texture2D>("Images/red"));
            tiles.Add("Maroon", Content.Load<Texture2D>("Images/maroon"));
            tiles.Add("Sky", Content.Load<Texture2D>("Images/sky"));
            tiles.Add("SkyToDirt", Content.Load<Texture2D>("Images/skyToDirt"));
            tiles.Add("Hole", Content.Load<Texture2D>("Images/downHole"));
            var mapSystem = new MapSystem(tiles);
            var dugSystem = new DugSystem(dug, mapSystem);
            var enemySystem = new EnemySystem();
            _systems.Add(mapSystem);
            _systems.Add(dugSystem);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            foreach(var system in _systems)
                system.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach(var system in _systems)
                system.Render(_spriteBatch);
            base.Draw(gameTime);
        }
    }
}