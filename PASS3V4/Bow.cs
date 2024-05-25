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

        public const int ID = 2;
        private const int DAMAGE = 5;

        private const int IMAGE_SOURCE_X = 0;
        private const int IMAGE_SOURCE_Y = 384;
        private const int IMAGE_SOURCE_WIDTH = 32;
        private const int IMAGE_SOURCE_HEIGHT = 64;

        private BowState state = BowState.Idle;

        private Arrow chargingArrow = null;
        private List<Arrow> flyingArrows = new List<Arrow>();

        public Bow(GraphicsDevice graphicsDevice, Vector2 centerPosition) : 
            base(graphicsDevice, new Rectangle(IMAGE_SOURCE_X, IMAGE_SOURCE_Y, IMAGE_SOURCE_WIDTH, IMAGE_SOURCE_HEIGHT), centerPosition, new Vector2(0, 0), new Vector2(IMAGE_SOURCE_WIDTH / 2, IMAGE_SOURCE_HEIGHT / 2), true, true, 2)
        {
            frames[(int)BowState.Idle] = Util.Crop(weaponSetImg, new Rectangle(IMAGE_SOURCE_X, IMAGE_SOURCE_Y, IMAGE_SOURCE_WIDTH, IMAGE_SOURCE_HEIGHT));
            frames[(int)BowState.Charging] = Util.Crop(weaponSetImg, new Rectangle(IMAGE_SOURCE_X + IMAGE_SOURCE_WIDTH, IMAGE_SOURCE_Y, IMAGE_SOURCE_WIDTH, IMAGE_SOURCE_HEIGHT));
        }

        private void CreateArrow(GraphicsDevice graphicsDevice)
        {
            chargingArrow = new Arrow(graphicsDevice, hitBox.Center.ToVector2(), angle);
        }

        private void UpdateArrows()
        {
            foreach (Arrow arrow in flyingArrows)
            {
                if (arrow.State == Arrow.ArrowState.Flying) arrow.UpdateFlying();
            }
        }

        public override void Update(GraphicsDevice graphicsDevice, GameTime gameTime, Vector2 position, MouseState mouse, MouseState prevMouse)
        {
            base.Update(graphicsDevice, gameTime, position, mouse, prevMouse);

            if (mouse.LeftButton == ButtonState.Pressed)
            {
                state = BowState.Charging;

                if (chargingArrow == null)
                {
                    CreateArrow(graphicsDevice);
                }

                chargingArrow.UpdateCharging(position, angle);
            }
            else if (prevMouse.LeftButton == ButtonState.Pressed && mouse.LeftButton != ButtonState.Pressed)
            { 
                if (chargingArrow != null)
                {
                    chargingArrow.State = Arrow.ArrowState.Flying;
                    flyingArrows.Add(chargingArrow);
                    chargingArrow = null;
                }
                state = BowState.Idle; 
            }

            UpdateArrows();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(img, rec, Color.White);
            spriteBatch.Draw(frames[(int)state], position - offset, sourceRec, Color.White, angle, origin, 1, SpriteEffects.None, 0);

            DrawArrows(spriteBatch);

            spriteBatch.DrawString(Assets.debugFont, angle.ToString(), new Vector2(10, 70), Color.White);

            // draw the hitbox
            degbugHitBox.Draw(spriteBatch, Color.Red, false);
        }

        private void DrawArrows(SpriteBatch spriteBatch)
        {
            // draw charging arrow
            if (chargingArrow != null)
            {
                chargingArrow.Draw(spriteBatch);
            }

            // draw flying arrows
            foreach (Arrow arrow in flyingArrows)
            {
                arrow.Draw(spriteBatch);
            }
        }
    }
}
