//Author: Colin Wang
//File Name: ExitRoom.cs
//Project Name: PASS3 a dungeon crawler
//Created Date: May 5, 2024
//Modified Date: June 10, 2024
//Description: Main driver class for the game

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;

namespace PASS3V4
{
    public class Game1 : Game
    {
        // Screen dimensions
        public const int SCREEN_WIDTH = 960;
        public const int SCREEN_HEIGHT = 800;

        // Screen center
        public static Vector2 ScreenCenter = new Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2);

        // Game objects
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        //Random number generator
        public static Random rng = new Random();

        //public static StreamReader sr;
        public static StreamWriter sw;

        // Level of the game
        Level level; // note it should really be a list of levels, but just go lazy
        private int currentLevel = 4;
        
        // Player variable
        Player player;

        // Keyboard and mouse variables
        KeyboardState prevKb;
        KeyboardState kb;
        MouseState mouse;
        MouseState prevMouse;

        // change the FPS varibles
        int updateFPS = 71;
        int drawFPS = 71;
        int updateTarget;
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

            // change the FPS
            IsFixedTimeStep = true;
            graphics.SynchronizeWithVerticalRetrace = false;
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000f / updateFPS);

            // apply the graphics change
            graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // store the update target
            updateTarget = updateFPS / drawFPS;

            spriteBatch = new SpriteBatch(GraphicsDevice);

            // load all assets
            Assets.Content = Content;
            Assets.Initialize();

            // initialize the level
            level = new Level(currentLevel); 
            level.Generate(Content, GraphicsDevice);

            player = new Player(Content, GraphicsDevice, "Player/Player.csv");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // update the keyboard variables
            prevKb = kb;
            kb = Keyboard.GetState();

            // update the mouse variables
            prevMouse = mouse;
            mouse = Mouse.GetState();


            // update the current level
            level.Update(gameTime, player, kb, prevKb, mouse, prevMouse);

            //  Update the number of updates passed since the last Draw
            updateCounter++;

            // Trigger a Draw if the Updates executed has reached the target
            //NOTE: most numbers are fine, but there a few that cause the flicker issue. e.g. 2, 4, 6, 8, 10, 12
            //I think you can see the pattern here...
            if (updateCounter == updateTarget)
            {
                // Reset the update counter, which will trigger a Draw
                updateCounter = 0;
            }

            base.Update(gameTime);
        }


        /// <summary>
        /// This method is called when the game should draw itself.
        /// It is called whenever the game needs to be redrawn.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Clear the screen with the desired colour

            // Check if the update counter has been reset to 0, indicating that it is time to draw
            if (updateCounter == 0)
            {
                GraphicsDevice.Clear(Color.Black);

                // Begin the drawing process
                spriteBatch.Begin();

                // Draw the level
                level.Draw(spriteBatch, player);

                // Draw the mouse position for debugging purposes
                DrawMouseDEBUG(spriteBatch);

                // End the drawing process
                spriteBatch.End();

                // Call the base Draw method to draw any additional elements
                base.Draw(gameTime);
            }
           
        }

        /// <summary>
        /// Draws the mouse position on the screen for debugging purposes.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use for drawing.</param>
        private void DrawMouseDEBUG(SpriteBatch spriteBatch)
        {
            // Calculate the position where to draw the mouse position
            Vector2 position = new Vector2(SCREEN_WIDTH - 200, SCREEN_HEIGHT - 50);

            // Draw the mouse position on the screen
            spriteBatch.DrawString(Assets.debugFont, // The font to use for drawing
                                   mouse.Position.ToString(), // The text to draw
                                   position, // The position on the screen
                                   Color.White); // The color of the text
        }
    }
}
