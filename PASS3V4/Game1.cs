﻿using Microsoft.Xna.Framework;
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

        Weapon weapon;

        KeyboardState prevKb;
        KeyboardState kb;

        //Step 1: Decide the target Update framerate desired
        int updateFPS = 70;

        //Step 2: Decide the target Draw framerate desired
        int drawFPS = 70;

        //Step 3: Calculate how many updates need to execute before a Draw occurs
        int updateTarget;

        //Step 4: Track how many Updates have passed since the last Draw
        int updateCounter = 0;

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

            IsFixedTimeStep = true;
            graphics.SynchronizeWithVerticalRetrace = false;
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000f / updateFPS);

            // apply the graphics change
            graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            updateTarget = updateFPS / drawFPS;

            spriteBatch = new SpriteBatch(GraphicsDevice);

            Assets.Content = Content;
            Assets.Initialize();

            room = new Room("Tiled/BasicRoom.tmx", GraphicsDevice);

            player = new Player(Content, GraphicsDevice, "Player/Player.csv");

            weapon = new Weapon(GraphicsDevice, Assets.weaponSetImg, new Rectangle(64, 128, 32, 64), player.GetCenterPosition() + new Vector2(0, -64), new Vector2(16, 64), 0.1f);
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
            weapon.Update(gameTime, player.GetCenterPosition());


            //Step 7: Update the number of updates passed since the last Draw
            updateCounter++;

            //Step 8: Trigger a Draw if the Updates executed has reached the target
            //NOTE: most numbers are fine, but there a few that cause the flicker issue. e.g. 2, 4, 6, 8, 10, 12
            //I think you can see the pattern here...
            if (updateCounter == updateTarget)
            {

                //Step 9: Reset the update counter, which will trigger a Draw
                updateCounter = 0;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            //Step 10: Redraw the screen whenever the updateCounter has been reset to 0
            if (updateCounter == 0)
            {
                //Clear the screen with the desired colour
                GraphicsDevice.Clear(Color.Black);

                spriteBatch.Begin();
                room.DrawBack(spriteBatch);

                player.Draw(spriteBatch, false);

                room.DrawFront(spriteBatch);

                room.DrawWallHitboxes(spriteBatch);

                weapon.Draw(spriteBatch);

                spriteBatch.End();
                base.Draw(gameTime);
            }
           
        }
    }
}
