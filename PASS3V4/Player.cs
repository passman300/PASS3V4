//Author: Colin Wang
//File Name: Player.cs
//Project Name: PASS3 a dungeon crawler
//Created Date: May 5, 2024
//Modified Date: June 10, 2024
//Description: Main player class for the game, has the basic properties and methods for the player

using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace PASS3V4
{
    public class Player : Entity
    {
        // possible play states
        public enum PlayerState
        {
            Idle,
            Walk,
            Run,
            Attack,
            Hurt,
            Die
        }

        const float SPEED_BASE = 4f;
        const float SPRINT_SCALE = 1.25f;

        // all hurt box and collision development for player
        public const int HURT_BOX_WIDTH = 32;
        public const int HURT_BOX_HEIGHT = 56;
        public const int HURT_BOX_OFFSET_X = 16;
        public const int HURT_BOX_OFFSET_Y = 8;


        // all feet images should be never seen.
        const int FEET_OFFSET_X = 0;
        const int FEET_OFFSET_Y = 41;
        const int FEET_HEIGHT = 15;
        const int FEET_WIDTH = 32;

        // You ok? hope you feel better by areas

        const int BREAD_CRUMB_DELAY = 70;
        const int MAX_BREAD_CRUMBS = 10;

        // maxium number of weapons each play has
        const int NUM_WEAPONS = 2;

        // current state
        public static PlayerState state = PlayerState.Idle;

        // player's weapons
        public static BreadCrumbs breadCrumbs = new();
        private Timer crumbTimer = new(BREAD_CRUMB_DELAY, true); // timer for bread crumbs

        private Weapon[] weapons = new Weapon[NUM_WEAPONS];
        private int activeWeapon = 0;

        // DEBUG
        public static bool isDebug = false;

        public Player(ContentManager content, GraphicsDevice graphicsDevice, string csvFilePath = "Player/Player.csv") : base(content, graphicsDevice)
        {

            // Load animations
            anim = Animation.LoadAnimations("AnimationSheets/" + csvFilePath, content);

            // Initialize hurt box and feet box
            InitializeRectangles(HURT_BOX_WIDTH, HURT_BOX_HEIGHT, HURT_BOX_OFFSET_X, HURT_BOX_OFFSET_Y, FEET_WIDTH, FEET_HEIGHT, FEET_OFFSET_X, FEET_OFFSET_Y);

            // Move the player to the center of the screen
            MoveFeetCenterPosition(Game1.ScreenCenter);

            // set the player's speed to the base speed
            Speed = SPEED_BASE;

            // give the player some weapons
            SetupWeapon(0, new LargeSword(graphicsDevice, centerPosition));
            SetupWeapon(1, new Bow(graphicsDevice, centerPosition));
        }

        /// <summary> 
        /// Get the player's center position
        /// </summary>
        /// <returns></returns>
        public static Vector2 GetPlayerCenterPosition() => new Vector2(Game1.ScreenCenter.X, Game1.ScreenCenter.Y);


        /// <summary> 
        /// Give the player a weapon at a specified index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="weapon"></param>
        private void SetupWeapon(int index, Weapon weapon)
        {
            weapons[index] = weapon;
        }

        /// <summary>
        /// Gets the players arrow which is has just shot
        /// </summary>
        /// <param name="IsShot"></param>
        /// <param name="GetFlyingArrow("></param>
        /// <returns> bool is true if the player shot an arrow </returns>
        public (bool IsShot, Arrow Arrow) GetFlyingArrow()
        {
            if (weapons[activeWeapon].GetType() == typeof(Bow)) 
                return ((Bow)weapons[activeWeapon]).GetFlyingArrow();

            return (false, null);
        }

        /// <summary> 
        /// clears the bread crumbs of the player
        /// </summary>
        public void ClearBreadCrumbs() => breadCrumbs.Clear();

        /// <summary> <summary>
        /// Update the player based on the users input
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="kb"></param>
        /// <param name="prevKb"></param>
        /// <param name="mouse"></param>
        /// <param name="prevMouse"></param>
        /// <param name="wallRecs"></param>
        public override void Update(GameTime gameTime, KeyboardState kb, KeyboardState prevKb, MouseState mouse, MouseState prevMouse, Rectangle[] wallRecs = null)
        {   
            // update animation
            anim[(int)state].Update(gameTime);

            // check what state the player is in
            switch (state)
            {
                case PlayerState.Idle:
                    UpdateIdle(gameTime, kb);
                    break;
                case PlayerState.Walk:
                    UpdateWalk(gameTime, kb, wallRecs);
                    break;
                case PlayerState.Run:
                    UpdateWalk(gameTime, kb, wallRecs);
                    break;
            }

            // always update the hurt boxes
            UpdateRecs(true, (int)state);

            // update bread crumbs
            UpdateBreadCrumbs(gameTime);

            // update weapons
            if (kb.IsKeyDown(Keys.E) && prevKb.IsKeyUp(Keys.E)) activeWeapon = (activeWeapon + 1) % 2;

            // update weapons
            weapons[activeWeapon].Update(graphicsDevice, gameTime, centerPosition, mouse, prevMouse);
        }

        /// <summary>
        /// Updates the player's movement when in the walking or running state.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="kb">The current state of the keyboard.</param>
        /// <param name="wallRecs">The array of wall collision rectangles.</param>
        private void UpdateWalk(GameTime gameTime, KeyboardState kb, Rectangle[] wallRecs)
        {
            // Check if the player is moving in any direction
            if ((kb.IsKeyDown(Keys.W) ^ kb.IsKeyDown(Keys.S)) || (kb.IsKeyDown(Keys.A) ^ kb.IsKeyDown(Keys.D)))
            {
                // Determine the direction of movement
                bool up = kb.IsKeyDown(Keys.W);
                bool down = kb.IsKeyDown(Keys.S);
                bool left = kb.IsKeyDown(Keys.A);
                bool right = kb.IsKeyDown(Keys.D);

                bool isSprinting = kb.IsKeyDown(Keys.LeftShift);

                // Determine the direction based on input
                if (left && !right) direction = LEFT;
                else if (right && !left) direction = RIGHT;

                // Determine the velocity based on input
                // Check all cases for left right input
                if ((right && left) || (!right && !left)) Velocity = new Vector2(0, Velocity.Y); // got to create new vector for properties
                else if (right && !left) Velocity = new Vector2(1, Velocity.Y);
                else if (left && !right) Velocity = new Vector2(-1, Velocity.Y);

                // Check all cases for up down input
                if ((up && down) || (!up && !down)) Velocity = new Vector2(Velocity.X, 0);
                else if (up && !down) Velocity = new Vector2(Velocity.X, -1); 
                else if (down && !up) Velocity = new Vector2(Velocity.X, 1);

                // Normalize velocity and set speed based on state
                if (Velocity.LengthSquared() > 0)
                {
                    Velocity = Vector2.Normalize(Velocity) * Speed;

                    if (isSprinting) state = PlayerState.Run;
                    else state = PlayerState.Walk;
                }

                // Adjust velocity based on sprint state
                if (isSprinting) Velocity *= SPRINT_SCALE;

                // Move the player based on velocity and wall collisions
                Move(wallRecs, Velocity.X, Velocity.Y);

                // Remove last breadcrumb if maximum is exceeded
                if (breadCrumbs.Size > MAX_BREAD_CRUMBS)
                {
                    breadCrumbs.RemoveLast();
                }
            }
            else state = PlayerState.Idle;
        }

        protected override void Move(Rectangle[] wallRecs, float x, float y)
        {
            base.Move(wallRecs, x, y);
        }

        /// <summary>
        /// Checks for wall collision and adjusts the velocity if necessary.
        /// </summary>
        /// <param name="wallRecs">An array of wall rectangles.</param>
        /// <param name="x">The x-coordinate of the velocity.</param>
        /// <param name="y">The y-coordinate of the velocity.</param>
        /// <returns>A tuple containing a boolean indicating if a collision occurred and the adjusted velocity.</returns>
        protected override (bool IsCollided, Vector2 Velocity) CheckWallCollision(Rectangle[] wallRecs, float x, float y)
        {
            // Check for wall collision and adjust velocity if necessary
            (bool isCollided, Vector2 velocity) temp = base.CheckWallCollision(wallRecs, x, y);

            // Adjust x-velocity if it exceeds the maximum speed
            if (Math.Abs(temp.velocity.X) > Speed * (PlayerState.Run == state ? SPRINT_SCALE : 1))
            {
                temp.velocity.X = 0;
            }

            // Adjust y-velocity if it exceeds the maximum speed
            if (Math.Abs(temp.velocity.Y) > Speed * (PlayerState.Run == state ? SPRINT_SCALE : 1))
            {
                temp.velocity.Y = 0;
            }

            return temp;
        }

        /// <summary> <summary>
        /// Update the input for the player
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="kb"></param>
        private void UpdateIdle(GameTime gameTime, KeyboardState kb)
        {
            if ((kb.IsKeyDown(Keys.W) ^ kb.IsKeyDown(Keys.S)) || (kb.IsKeyDown(Keys.A) ^ kb.IsKeyDown(Keys.D))) state = PlayerState.Walk;
        }

        /// <summary> 
        /// Update the breadcrumbs after the player moves
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateBreadCrumbs(GameTime gameTime)
        {
            if (breadCrumbs.Size == 0) // check if list is empty
            {
                breadCrumbs.AddLast(centerPosition);
            }

            if (breadCrumbs.PeakFirst() != centerPosition) // check if the player doesn't serve the middle.
            {
                crumbTimer.Update(gameTime);

                // Add breadcrumb if timer is finished
                if (crumbTimer.IsFinished())
                {
                    breadCrumbs.AddFirst(centerPosition);

                    crumbTimer.ResetTimer(true);
                }
            }
        }


        /// <summary>
        /// Draw the player and its associated weapons to the SpriteBatch.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to draw to.</param>
        /// <param name="filler">Optional parameter to draw filler rectangles for debugging.</param>
        public override void Draw(SpriteBatch spriteBatch, bool filler = false)
        {
            // Draw the player's animation based on the direction and state.
            if (direction == LEFT)
            {
                anim[(int)state].Draw(spriteBatch, Color.White, SpriteEffects.FlipHorizontally);
            }
            else if (direction == RIGHT)
            {
                anim[(int)state].Draw(spriteBatch, Color.White);
            }

            // Draw the active weapon.
            weapons[activeWeapon].Draw(spriteBatch);

            if (isDebug) // DEBUG
            {
                // Draw debug information.
                spriteBatch.DrawString(Assets.debugFont, $" position: X: {position.X}, Y: {position.Y}", new Vector2(10, 10), Color.White);
                spriteBatch.DrawString(Assets.debugFont, $" direction: {direction}", new Vector2(10, 30), Color.White);
                spriteBatch.DrawString(Assets.debugFont, $" velocity: {Velocity}", new Vector2(10, 50), Color.White);
                spriteBatch.DrawString(Assets.debugFont, $" weapon angle: {weapons[activeWeapon].GetAngle()}", new Vector2(10, 70), Color.White);

                // Draw debug hitboxes.
                debugFeetRec.Draw(spriteBatch, Color.Red, false);
                debugHurtBox.Draw(spriteBatch, Color.Green, false);
                debugImgBox.Draw(spriteBatch, Color.Purple, false);

                // Draw breadcrumbs if enabled.
                if (true)
                {
                    breadCrumbs.PrintList();

                    foreach (Vector2 v in breadCrumbs.ToList())
                    {
                        spriteBatch.Draw(Assets.pixel, new Rectangle((int)v.X, (int)v.Y, 3, 3), Color.Red);
                    }
                }
            }
        }



    }
}
