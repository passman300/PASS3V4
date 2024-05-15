using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace PASS3V4
{
    public class Game1 : Game
    {
        private const int SCREEN_WIDTH = 960;
        private const int SCREEN_HEIGHT = 800;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        Room room;
        Player player;

        KeyboardState prevKb;
        KeyboardState kb;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;

            // apply the graphics change
            graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Assets.Content = Content;
            Assets.Initialize();

            room = new Room("Tiled/BasicRoom.tmx", GraphicsDevice);

            player = new Player(Content, GraphicsDevice, "Player/Player.csv");
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            prevKb = kb;
            kb = Keyboard.GetState();

            // TODO: Add your update logic here
            //player.Update(gameTime, Keyboard.GetState(), Keyboard.GetState());

            //for (int i = 0; i < room.BackLayers.Length; i++)
            //{
            //    room.BackLayers[i].Update(gameTime);
            //}

            //for (int i = 0; i < room.FrontLayers.Length; i++)
            //{
            //    room.FrontLayers[i].Update(gameTime);
            //}

            room.Update(gameTime, player, kb, prevKb);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
             
            // TODO: Add your drawing code here

            spriteBatch.Begin();
            room.DrawBack(spriteBatch);

            player.Draw(spriteBatch, false);

            room.DrawFront(spriteBatch);

            room.DrawWallHitboxes(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
