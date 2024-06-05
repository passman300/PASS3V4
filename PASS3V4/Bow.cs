using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameUtility;
using System;
using Microsoft.Xna.Framework.Content;
using System.ComponentModel.Design.Serialization;
using Microsoft.Xna.Framework.Input;
using System.Net;
using System.Collections.Generic;

namespace PASS3V4
{
    public class Bow : Weapon
    {
        public enum BowState
        {
            Idle,
            Charging,
        }

        private const int DAMAGE = 5;
        private const int BASE_COOLDOWN = 500;
        private const int CHARGE_TIME = 3000;

        private const int IMG_SRC_X = 0;
        private const int IMG_SRC_Y = 384;
        private const int IMG_SRC_WIDTH = 32;
        private const int IMG_SRC_HEIGHT = 64;

        private BowState state = BowState.Idle;

        private Arrow chargingArrow = null;
        private Arrow flyingArrow = null;
        private bool isShoot = false;

        private Timer coolDown = new Timer(BASE_COOLDOWN, true); // cool down time before next arrow can be charged (500 ms)
        private Timer chargeTimer = new Timer(CHARGE_TIME, false);

        public Bow(GraphicsDevice graphicsDevice, Vector2 centerPosition) : 
            base(graphicsDevice, new Rectangle(IMG_SRC_X, IMG_SRC_Y, IMG_SRC_WIDTH, IMG_SRC_HEIGHT), centerPosition, new Vector2(0, 0), new Vector2(IMG_SRC_WIDTH / 2, IMG_SRC_HEIGHT / 2), true, true, 2)
        {
            Id = WeaponType.Bow;
            
            frames[(int)BowState.Idle] = Util.Crop(weaponSetImg, new Rectangle(IMG_SRC_X, IMG_SRC_Y, IMG_SRC_WIDTH, IMG_SRC_HEIGHT));
            frames[(int)BowState.Charging] = Util.Crop(weaponSetImg, new Rectangle(IMG_SRC_X + IMG_SRC_WIDTH, IMG_SRC_Y, IMG_SRC_WIDTH, IMG_SRC_HEIGHT));
        }

        public (bool IsShoot, Arrow Arrow) GetFlyingArrow() => (isShoot, flyingArrow);

        private void CreateArrow(GraphicsDevice graphicsDevice)
        {
            chargingArrow = new Arrow(graphicsDevice, Arrow.BASE_SPEED, angle, hitBox.Center.ToVector2(), Arrow.BASE_DAMAGE);
        }


        public override void Update(GraphicsDevice graphicsDevice, GameTime gameTime, Vector2 position, MouseState mouse, MouseState prevMouse)
        {
            base.Update(graphicsDevice, gameTime, position, mouse, prevMouse);

            coolDown.Update(gameTime);

            

            if (mouse.LeftButton == ButtonState.Pressed && coolDown.IsFinished()) // charge arrow
            {
                // check if the charge timer has started
                if (chargeTimer.IsInactive()) 
                    chargeTimer.ResetTimer(true);

                chargeTimer.Update(gameTime);
                state = BowState.Charging;

                if (chargingArrow == null)
                {
                    CreateArrow(graphicsDevice);
                }

                isShoot = false;
                chargingArrow.UpdateCharging(position, angle, (float)(chargeTimer.GetTimePassed() / CHARGE_TIME));
                System.Diagnostics.Debug.WriteLine(chargeTimer.GetTimePassed());
            }
            else if (prevMouse.LeftButton == ButtonState.Pressed && mouse.LeftButton != ButtonState.Pressed && chargingArrow != null) // shoot the arrow
            {
                if (chargingArrow != null)
                {
                    chargingArrow.PlayerState = Arrow.ArrowState.Flying;
                    flyingArrow = chargingArrow; 
                    isShoot = true;
                    chargingArrow = null;
                }
                state = BowState.Idle;

                coolDown.ResetTimer(true);
                chargeTimer.ResetTimer(false);
            }
            else if (prevMouse.LeftButton != ButtonState.Pressed && mouse.LeftButton != ButtonState.Pressed) // idling the bow, not charging
            {
                state = BowState.Idle;
                chargingArrow = null;
                isShoot = false;
            }
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(img, rec, Color.White);
            spriteBatch.Draw(frames[(int)state], position - offset, sourceRec, Color.White, angle, origin, 1, SpriteEffects.None, 0);

            DrawChargingArrow(spriteBatch);

            if (isDebug)
            {
                //degbugHitBox.Draw(spriteBatch, Color.Red, false);
                spriteBatch.DrawString(Assets.debugFont, coolDown.GetTimeRemainingAsString(Timer.FORMAT_SEC_MIL), new Vector2(10, 50), Color.White);
                spriteBatch.DrawString(Assets.debugFont, angle.ToString(), new Vector2(10, 100), Color.White);
            }
        }

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
