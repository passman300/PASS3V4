using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameUtility;
using System;
using Microsoft.Xna.Framework.Content;
using System.ComponentModel.Design.Serialization;
using Microsoft.Xna.Framework.Input;
using System.Reflection.Metadata;
using static PASS3V4.Bow;
using static PASS3V4.Player;


namespace PASS3V4
{
    public class Arrow
    {
        public enum ArrowState 
        {
            Charging,
            Flying
        }

        public int Damage { get; private set; }

        public const int IMAGE_SOURCE_X = 64;
        public const int IMAGE_SOURCE_Y = 384;
        public const int IMAGE_SOURCE_WIDTH = 48;
        public const int IMAGE_SOURCE_HEIGHT = 32;

        private static Texture2D img = Util.Crop(Assets.weaponSetImg, new Rectangle(IMAGE_SOURCE_X, IMAGE_SOURCE_Y, IMAGE_SOURCE_WIDTH, IMAGE_SOURCE_HEIGHT));
        private Rectangle sourceRec;

        private Vector2 velocity;
        private float speed;

        private float angle;

        private Vector2 position; // should start from the center of the player
        private Vector2 offset;
        private Vector2 origin;

        private Rectangle hitBox;
        private float hitBoxWidth;
        private float hitBoxHeight;

        private GameRectangle degbugHitBox;

        public ArrowState State { get; set; }

        public Arrow(GraphicsDevice graphicsDevice, 
            Vector2 centerPos, float angle, float speed = 1, int damage = 1)
        {
            position = centerPos + offset;
            this.origin = new Vector2(IMAGE_SOURCE_WIDTH / 2, IMAGE_SOURCE_HEIGHT / 2); // pivot point

            this.angle = angle;
            this.speed = speed;
            this.Damage = damage;

            sourceRec = new Rectangle(0, 0, IMAGE_SOURCE_WIDTH, IMAGE_SOURCE_HEIGHT);

            InitializeHitBox(graphicsDevice, IMAGE_SOURCE_WIDTH, IMAGE_SOURCE_HEIGHT);

        }

        public float GetAngle() => angle;

        public Rectangle GetImageSourceRec() => sourceRec;

        private void InitializeHitBox(GraphicsDevice graphicsDevice, int hitBoxWidth, int hitBoxHeight)
        {
            this.hitBoxWidth = hitBoxWidth;
            this.hitBoxHeight = hitBoxHeight;

            hitBox = new Rectangle((int)(position.X) - hitBoxWidth / 2, (int)(position.Y) - hitBoxHeight, hitBoxWidth, hitBoxHeight);
            degbugHitBox = new GameRectangle(graphicsDevice, hitBox);
        }

        public void UpdateCharging(Vector2 position, float angle)
        {
            // update the position of the weapon 
            this.position = position + offset;

            // update the angle 
            this.angle = angle;

            // update the hitbox of the weapon
            hitBox.X = (int)(position.X - hitBoxWidth / 2);
            hitBox.Y = (int)(position.Y - hitBoxHeight / 2);

            velocity.X = (float)Math.Cos(angle);
            velocity.Y = (float)Math.Sin(angle);

            velocity.Normalize(); // normalize the 
            velocity *= speed;

            degbugHitBox.TranslateTo(hitBox.X, hitBox.Y);
        }

        public void UpdateFlying()
        {
            position += velocity;
            hitBox.X = (int)(position.X - hitBoxWidth / 2);
            hitBox.Y = (int)(position.Y - hitBoxHeight / 2);

            degbugHitBox.TranslateTo(hitBox.X, hitBox.Y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(img, position - offset, sourceRec, Color.White, angle, origin, 1, SpriteEffects.None, 0);
            spriteBatch.DrawString(Assets.debugFont, velocity.ToString(), hitBox.Center.ToVector2() - new Vector2(10, 10), Color.White);

            // draw the hitbox
            degbugHitBox.Draw(spriteBatch, Color.Red, false);
        }
    }
}
