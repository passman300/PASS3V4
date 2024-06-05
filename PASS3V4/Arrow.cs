using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameUtility;
using System;
using Microsoft.Xna.Framework.Content;
using System.ComponentModel.Design.Serialization;
using Microsoft.Xna.Framework.Input;
using System.Reflection.Metadata;


namespace PASS3V4
{
    public class Arrow : Projectile
    {
        public enum ArrowState 
        {
            Charging,
            Flying
        }

        public const int IMG_SRC_X = 64;
        public const int IMG_SRC_Y = 384;
        public const int IMG_SRC_WIDTH = 48;
        public const int IMG_SRC_HEIGHT = 32;

        public const int BASE_SPEED = 10;
        public const int BASE_DAMAGE = 5;

        private static Texture2D img = Util.Crop(Assets.weaponSetImg, new Rectangle(IMG_SRC_X, IMG_SRC_Y, IMG_SRC_WIDTH, IMG_SRC_HEIGHT));

        public static bool isDebug = false;
        private GameRectangle degbugHitBox;

        public int Damage { get; private set; }

        public ArrowState PlayerState { get; set; }

        public Arrow(GraphicsDevice graphicsDevice, float speed, float angle, Vector2 centerPos, int damage = BASE_DAMAGE):
            base(graphicsDevice, img, new Rectangle(0, 0, IMG_SRC_WIDTH, IMG_SRC_HEIGHT), speed, angle, centerPos, new Vector2(0,0), new Vector2(IMG_SRC_WIDTH / 2, IMG_SRC_HEIGHT / 2), damage)
        {


            InitializeHitBox(graphicsDevice, IMG_SRC_WIDTH, IMG_SRC_HEIGHT);
        }

        private void InitializeHitBox(GraphicsDevice graphicsDevice, int hitBoxWidth, int hitBoxHeight)
        {
            HitBoxWidth = hitBoxWidth;
            HitBoxHeight = hitBoxHeight;

            HitBox = new Rectangle((int)(Position.X) - hitBoxWidth / 2, (int)(Position.Y) - hitBoxHeight, hitBoxWidth, hitBoxHeight);
            degbugHitBox = new GameRectangle(graphicsDevice, HitBox);
        }

        public void UpdateCharging(Vector2 centerPos, float angle, float percentCharged = 1f)
        {
            // update the position of the weapon 
            Position = centerPos + Offset;

            // update the angle 
            Angle = angle;

            // update the hitbox of the weapon
            // HitBox = new Rectangle((int)(position.X - HitBoxWidth / 2), (int)(position.Y - HitBoxHeight / 2), (int)HitBoxWidth, (int)HitBoxHeight);
            HitBox = new Rectangle((int)Position.X, (int)Position.Y, (int)HitBoxWidth, (int)HitBoxHeight);

            // update the velocity based on the angle
            Velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

            Velocity.Normalize(); // normalize the vector
            Velocity *= Speed * percentCharged; // multiply the vector by the speed

            // update the position of the hitbox
            if (isDebug) degbugHitBox.TranslateTo(HitBox.X, HitBox.Y);
        }

        public void UpdateFlying()
        {
            Position += Velocity;
            HitBox = new Rectangle((int)(Position.X - HitBoxWidth / 2), (int)(Position.Y - HitBoxHeight / 2), (int)HitBoxWidth, (int)HitBoxHeight);

            if (isDebug) degbugHitBox.TranslateTo(HitBox.X, HitBox.Y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Img, Position - Offset, SrcRec, Color.White, Angle, Origin, 1, SpriteEffects.None, 0);

            // draw the hitbox
            if (isDebug)
            {
                degbugHitBox.Draw(spriteBatch, Color.Red, false);
                spriteBatch.DrawString(Assets.debugFont, Velocity.ToString(), HitBox.Center.ToVector2() - new Vector2(10, 10), Color.White);
            }
         }
    }
}
