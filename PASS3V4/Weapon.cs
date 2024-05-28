using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameUtility;
using System;
using Microsoft.Xna.Framework.Input;

namespace PASS3V4
{
    public class Weapon
    {
        public enum WeaponType
        {
            None,
            SmallSword,
            LargeSword,
            Bow
        }

        protected const int WEAPON_SET_IMG_WIDTH = 4;

        public WeaponType Id { get; protected set; }

        public int Damage { get; protected set; }


        protected float rotationSpeed;
        protected float rotationMultiplier = 1;

        protected static Texture2D weaponSetImg = Assets.weaponSetImg;
        protected Texture2D img; // the image of the weapon
        protected Rectangle sourceRec; // source rectangle used to rotate the img
        
        protected bool isAnimated = false;
        protected Texture2D[] frames;

        protected Vector2 position; // should start from the center of the player
        protected Vector2 offset;
        protected Vector2 origin;
        protected float angle; // angle should be in radian

        protected bool isFollowMouse = false;

        protected Rectangle hitBox;
        protected float hitBoxWidth;
        protected float hitBoxHeight;
        
        protected GameRectangle degbugHitBox;
        public bool isDebug = false;

        public Weapon(GraphicsDevice graphicsDevice, Rectangle imgSourceRec, Vector2 centerPosition, Vector2 offset, Vector2 origin, float rotationSpeed, float angle = 0, bool isAnimated = false)
        {
            this.offset = offset;
            position = centerPosition + offset;
            this.origin = origin; // pivot point
            this.rotationSpeed = rotationSpeed;

            sourceRec = new Rectangle(0, 0, imgSourceRec.Width, imgSourceRec.Height);

            InitializeHitBox(graphicsDevice, imgSourceRec.Width, imgSourceRec.Height);

            img = Util.Crop(weaponSetImg, imgSourceRec);
        }

        public Weapon(GraphicsDevice graphicsDevice, Rectangle imgSourceRec, Vector2 centerPosition, Vector2 offset, Vector2 origin, bool isFollowMouse = false, bool isAnimated = false, int numFrames = 1)
        {
            this.offset = offset;
            position = centerPosition + offset;
            this.origin = origin; // pivot point

            sourceRec = new Rectangle(0, 0, imgSourceRec.Width, imgSourceRec.Height);

            InitializeHitBox(graphicsDevice, imgSourceRec.Width, imgSourceRec.Height);

            img = Util.Crop(weaponSetImg, imgSourceRec);

            this.isFollowMouse = isFollowMouse;
            this.isAnimated = isAnimated;

            frames = new Texture2D[numFrames];
        }

        public float GetAngle() => angle;
        public Rectangle GetHitBox() => hitBox;


        protected virtual void InitializeHitBox(GraphicsDevice graphicsDevice, int hitBoxWidth, int hitBoxHeight)
        {
            this.hitBoxWidth = hitBoxWidth;
            this.hitBoxHeight = hitBoxHeight;

            hitBox = new Rectangle((int)(position.X) - hitBoxWidth / 2, (int)(position.Y) - hitBoxHeight, hitBoxWidth, hitBoxHeight);
            degbugHitBox = new GameRectangle(graphicsDevice, hitBox);
        }

        /// <summary>
        /// update the weapon position, angle and hitbox
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="position"></param>
        public virtual void Update(GraphicsDevice graphicsDevice, GameTime gameTime, Vector2 position, MouseState mouse, MouseState prevMouse)
        {
            // update the position of the weapon 
            this.position = position + offset;

            // update the hitbox of the weapon
            //hitBox.X = (int)(hitBoxHeight / 2 * Math.Cos(angle - MathHelper.PiOver2) + position.X - hitBoxWidth / 2);
            hitBox.X = (int)(Math.Abs(offset.Y) / 2 * Math.Cos(angle - MathHelper.PiOver2) + position.X - hitBoxWidth / 2);

            hitBox.Y = (int)(Math.Abs(offset.Y) / 2 * Math.Sin(angle - MathHelper.PiOver2) + position.Y - hitBoxHeight / 2);

            if (isDebug) degbugHitBox.TranslateTo(hitBox.X, hitBox.Y);

            if (isFollowMouse) angle = (float)Math.Atan2(mouse.Y - position.Y, mouse.X - position.X);

            else
            {
                // update the angle 
                angle += rotationSpeed;

                angle %= (float)Math.Tau; // two pi
            }
        }

        public bool IsColliding(Rectangle otherRec, float otherAngle = 0)
        {
            return Util.RotatedCollision(hitBox, otherRec, angle, otherAngle);
        }


        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(img, position - offset, sourceRec, Color.White, angle, origin, 1, SpriteEffects.None, 0);

            spriteBatch.DrawString(Assets.debugFont, angle.ToString(), new Vector2(10, 70), Color.White);

            // draw the hitbox
            if (isDebug) degbugHitBox.Draw(spriteBatch, Color.Red, false);
        }

    }
}
