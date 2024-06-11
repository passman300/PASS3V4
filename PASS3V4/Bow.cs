//Author: Colin Wang
//File Name: Bow.cs
//Project Name: PASS3 a dungeon crawler
//Created Date: May 21, 2024
//Modified Date: June 10, 2024
//Description: Bow class which inherits from Weapon, and is used by the player to shoot arrows. The aiming of the bow is handled by the player mouse


using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameUtility;
using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;


namespace PASS3V4
{
    /// <summary>
    /// Represents a bow weapon that can be used to shoot arrows
    /// </summary>
    public class Bow : Weapon
    {
        /// <summary>
        /// Possible states of the bow
        /// </summary>
        public enum BowState
        {
            /// <summary>
            /// Bow is in idle state
            /// </summary>
            Idle,
            /// <summary>
            /// Bow is in charging state
            /// </summary>
            Charging,
        }

        // Constants for the bow
        private const int DAMAGE = 5;
        private const int BASE_COOLDOWN = 500;
        private const int CHARGE_TIME = 3000;

        private const int IMG_SRC_X = 0;
        private const int IMG_SRC_Y = 384;
        private const int IMG_SRC_WIDTH = 32;
        private const int IMG_SRC_HEIGHT = 64;

        // Variables for the bow state
        private BowState state = BowState.Idle;

        // Variables for the arrow
        private Arrow chargingArrow = null;
        private Arrow flyingArrow = null;
        private bool isShoot = false;

        // Variables for the timers
        private Timer coolDown = new Timer(BASE_COOLDOWN, true); // cool down time before next arrow can be charged (500 ms)
        private Timer chargeTimer = new Timer(CHARGE_TIME, false);

        /// <summary>
        /// Constructs a new Bow with specified center position
        /// </summary>
        /// <param name="graphicsDevice">Graphics device to draw the bow to</param>
        /// <param name="centerPosition">Center position of the bow</param>
        public Bow(GraphicsDevice graphicsDevice, Vector2 centerPosition) : 
            base(graphicsDevice, new Rectangle(IMG_SRC_X, IMG_SRC_Y, IMG_SRC_WIDTH, IMG_SRC_HEIGHT), centerPosition, new Vector2(0, 0), new Vector2(IMG_SRC_WIDTH / 2, IMG_SRC_HEIGHT / 2), true, true, 2)
        {
            Id = WeaponType.Bow; // set the id of the weapon
            
            // set the frames of the bow
            frames[(int)BowState.Idle] = Util.Crop(weaponSetImg, new Rectangle(IMG_SRC_X, IMG_SRC_Y, IMG_SRC_WIDTH, IMG_SRC_HEIGHT));
            frames[(int)BowState.Charging] = Util.Crop(weaponSetImg, new Rectangle(IMG_SRC_X + IMG_SRC_WIDTH, IMG_SRC_Y, IMG_SRC_WIDTH, IMG_SRC_HEIGHT));
        }

        /// <summary>
        /// Gets whether the bow has been shot and returns the flying arrow
        /// </summary>
        /// <returns>Tuple with IsShoot and Arrow</returns>
        public (bool IsShoot, Arrow Arrow) GetFlyingArrow() => (isShoot, flyingArrow);

        /// <summary>
        /// Creates a new charging arrow
        /// </summary>
        /// <param name="graphicsDevice">Graphics device to draw the arrow to</param>
        private void CreateArrow(GraphicsDevice graphicsDevice)
        {
            chargingArrow = new Arrow(graphicsDevice, Arrow.BASE_SPEED, angle, hitBox.Center.ToVector2(), Arrow.BASE_DAMAGE);
        }


        /// <summary>
        /// Updates the bow and the charging and flying arrows
        /// </summary>
        public override void Update(GraphicsDevice graphicsDevice, GameTime gameTime, Vector2 position, MouseState mouse, MouseState prevMouse)
        {
            // Call the base update function
            base.Update(graphicsDevice, gameTime, position, mouse, prevMouse);

            // Update the cooldown shoot timer
            coolDown.Update(gameTime);

            
            // check if the mouse is being pressed, and if the cooldown is finished
            if (mouse.LeftButton == ButtonState.Pressed && coolDown.IsFinished()) // charge arrow
            {
                // check if the charge timer has started
                if (chargeTimer.IsInactive()) 
                    chargeTimer.ResetTimer(true);

                chargeTimer.Update(gameTime);
                state = BowState.Charging;

                // check if the charging arrow is null, and if so, create a new one
                if (chargingArrow == null)
                {
                    CreateArrow(graphicsDevice);
                }

                // set the isShoot to false
                isShoot = false;

                //chargingArrow.UpdateCharging(position, angle, (float)(chargeTimer.GetTimePassed() / CHARGE_TIME)); // with speed up
                chargingArrow.UpdateCharging(position, angle); // with speed up

            }
            // check if the mouse is not being pressed, and if there is already a charging arrow
            else if (prevMouse.LeftButton == ButtonState.Pressed && mouse.LeftButton != ButtonState.Pressed && chargingArrow != null) // shoot the arrow
            {
                if (chargingArrow != null)
                {
                    // transfer the charging arrow to the flying arrow
                    chargingArrow.PlayerState = Arrow.ArrowState.Flying;
                    flyingArrow = chargingArrow; 
                    isShoot = true;
                    chargingArrow = null;
                }
                
                // set the state to idle
                state = BowState.Idle;

                // reset the timers
                coolDown.ResetTimer(true);
                chargeTimer.ResetTimer(false);
            }
            // check if the mouse is not being pressed, and if there is no charging arrow 
            else if (prevMouse.LeftButton != ButtonState.Pressed && mouse.LeftButton != ButtonState.Pressed) // idling the bow, not charging
            {
                // set the state to idle
                state = BowState.Idle;
                chargingArrow = null;
                isShoot = false;
            }
        }


        /// <summary>
        /// Draws the bow and the charging arrow
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(img, rec, Color.White);
            spriteBatch.Draw(frames[(int)state], position - offset, sourceRec, Color.White, angle, origin, 1, SpriteEffects.None, 0);

            // draw the charging arrow
            DrawChargingArrow(spriteBatch);

            if (isDebug) // DEBUG
            {
                //degbugHitBox.Draw(spriteBatch, Color.Red, false);
                spriteBatch.DrawString(Assets.debugFont, coolDown.GetTimeRemainingAsString(Timer.FORMAT_SEC_MIL), new Vector2(10, 50), Color.White);
                spriteBatch.DrawString(Assets.debugFont, angle.ToString(), new Vector2(10, 100), Color.White);
            }
        }

        /// <summary>
        /// Draws the charging arrow
        /// </summary>
        private void DrawChargingArrow(SpriteBatch spriteBatch)
        {
            // draw charging arrow
            if (chargingArrow != null)
            {
                chargingArrow.Draw(spriteBatch);
            }
        }
    }
}
