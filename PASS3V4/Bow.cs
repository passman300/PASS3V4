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

        private const int IMAGE_SOURCE_X = 0;
        private const int IMAGE_SOURCE_Y = 384;
        private const int IMAGE_SOURCE_WIDTH = 32;
        private const int IMAGE_SOURCE_HEIGHT = 64;

        private BowState state = BowState.Idle;

        private Arrow chargingArrow = null;
        private Arrow flyingArrow = null;
        private bool isShoot = false;

        private Timer coolDown = new Timer(BASE_COOLDOWN, true); // cool down time before next arrow can be charged (500 ms)

        public Bow(GraphicsDevice graphicsDevice, Vector2 centerPosition) : 
            base(graphicsDevice, new Rectangle(IMAGE_SOURCE_X, IMAGE_SOURCE_Y, IMAGE_SOURCE_WIDTH, IMAGE_SOURCE_HEIGHT), centerPosition, new Vector2(0, 0), new Vector2(IMAGE_SOURCE_WIDTH / 2, IMAGE_SOURCE_HEIGHT / 2), true, true, 2)
        {
            Id = WeaponType.Bow;
            
            frames[(int)BowState.Idle] = Util.Crop(weaponSetImg, new Rectangle(IMAGE_SOURCE_X, IMAGE_SOURCE_Y, IMAGE_SOURCE_WIDTH, IMAGE_SOURCE_HEIGHT));
            frames[(int)BowState.Charging] = Util.Crop(weaponSetImg, new Rectangle(IMAGE_SOURCE_X + IMAGE_SOURCE_WIDTH, IMAGE_SOURCE_Y, IMAGE_SOURCE_WIDTH, IMAGE_SOURCE_HEIGHT));
        }

        public (bool IsShoot, Arrow Arrow) GetFlyingArrow() => (isShoot, flyingArrow);

        private void CreateArrow(GraphicsDevice graphicsDevice)
        {
            chargingArrow = new Arrow(graphicsDevice, hitBox.Center.ToVector2(), angle, 10);
        }


        public override void Update(GraphicsDevice graphicsDevice, GameTime gameTime, Vector2 position, MouseState mouse, MouseState prevMouse)
        {
            base.Update(graphicsDevice, gameTime, position, mouse, prevMouse);

            coolDown.Update(gameTime);

            if (mouse.LeftButton == ButtonState.Pressed && coolDown.IsFinished()) // charge arrow
            {
                state = BowState.Charging;

                if (chargingArrow == null)
                {
                    CreateArrow(graphicsDevice);
                }

                isShoot = false;
                chargingArrow.UpdateCharging(position, angle);
            }
            else if (prevMouse.LeftButton == ButtonState.Pressed && mouse.LeftButton != ButtonState.Pressed && chargingArrow != null) // shoot the arrow
            {
                if (chargingArrow != null)
                {
                    chargingArrow.State = Arrow.ArrowState.Flying;
                    flyingArrow = chargingArrow; 
                    isShoot = true;
                    chargingArrow = null;
                }
                state = BowState.Idle;

                coolDown.ResetTimer(true);
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
