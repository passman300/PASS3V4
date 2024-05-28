using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace PASS3V4
{
    public class Game1 : Game
    {
        public const int SCREEN_WIDTH = 960;
        public const int SCREEN_HEIGHT = 800;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public static Random rng = new Random();

        Level level;
        Player player;

        Rectangle tempRec = new Rectangle(480, 400, 90, 50);

        KeyboardState prevKb;
        KeyboardState kb;
        MouseState mouse;
        MouseState prevMouse;

        //Step 1: Decide the target Update framerate desired
        int updateFPS = 71;

        //Step 2: Decide the target Draw framerate desired
        int drawFPS = 71;

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

            level = new Level(1);
            level.Generate(GraphicsDevice);

            player = new Player(Content, GraphicsDevice, "Player/Player.csv");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            prevKb = kb;
            kb = Keyboard.GetState();

            prevMouse = mouse;
            mouse = Mouse.GetState();

            level.Update(gameTime, player, kb, prevKb, mouse, prevMouse);

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

                level.Draw(spriteBatch, player);

                spriteBatch.End();

                base.Draw(gameTime);
            }
           
        }
    }
}
