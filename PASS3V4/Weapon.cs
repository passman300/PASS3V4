//Author: Colin Wang
//File Name: Weapon.cs
//Project Name: PASS3 a dungeon crawler
//Created Date: May 18, 2024
//Modified Date: June 10, 2024
//Description: The parent class of all weapons, contains the damage, and some properties of all existing weapons

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameUtility;
using System;
using Microsoft.Xna.Framework.Input;

namespace PASS3V4
{
    public class Weapon
    {
        /// <summary>
        /// Represents the type of the weapon
        /// </summary>
        public enum WeaponType
        {
            None,
            SmallSword,
            LargeSword,
            Bow
        }

        public WeaponType Id { get; protected set; } // the id of the weapon

        public int Damage { get; protected set; } // the damage of the weapon


        protected float rotationSpeed; // the rotation speed of the weapon
        protected float rotationMultiplier = 1; // the rotation multiplier of the weapon

        protected static Texture2D weaponSetImg = Assets.weaponSetImg; // the image of the weapon set
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

        // DEBUG
        protected GameRectangle degbugHitBox;
        public bool isDebug = false; 

        /// <summary>
        /// Initialize the weapon
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="imgSourceRec"></param>
        /// <param name="centerPosition"></param>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <param name="rotationSpeed"></param>
        /// <param name="angle"></param>
        /// <param name="isAnimated"></param>
        public Weapon(GraphicsDevice graphicsDevice, Rectangle imgSourceRec, Vector2 centerPosition, Vector2 offset, Vector2 origin, float rotationSpeed, float angle = 0, bool isAnimated = false)
        {
            this.offset = offset;
            position = centerPosition + offset;
            this.origin = origin; // pivot point
            this.rotationSpeed = rotationSpeed;

            this.angle = angle;
            InitializeHitBox(graphicsDevice, imgSourceRec.Width, imgSourceRec.Height);

            // crop the image
            sourceRec = new Rectangle(0, 0, imgSourceRec.Width, imgSourceRec.Height);
            img = Util.Crop(weaponSetImg, imgSourceRec);
        }

        /// <summary>
        /// Initialize the weapon constructor
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="imgSourceRec"></param>
        /// <param name="centerPosition"></param>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <param name="isFollowMouse"></param>
        /// <param name="isAnimated"></param>
        /// <param name="numFrames"></param>
        public Weapon(GraphicsDevice graphicsDevice, Rectangle imgSourceRec, Vector2 centerPosition, Vector2 offset, Vector2 origin, bool isFollowMouse = false, bool isAnimated = false, int numFrames = 1)
        {
            this.offset = offset;
            position = centerPosition + offset;
            this.origin = origin; // pivot point

            sourceRec = new Rectangle(0, 0, imgSourceRec.Width, imgSourceRec.Height);

            InitializeHitBox(graphicsDevice, imgSourceRec.Width, imgSourceRec.Height);

            img = Util.Crop(weaponSetImg, imgSourceRec);

            // set the flags of follow mouse and animated as specified
            this.isFollowMouse = isFollowMouse;
            this.isAnimated = isAnimated;

            // initialize the frames of the weapon
            frames = new Texture2D[numFrames];
        }

        /// <summary>
        /// Get the angle of the weapon
        /// </summary>
        /// <returns></returns>
        public float GetAngle() => angle;

        /// <summary>
        /// Get the hitbox of the weapon
        /// </summary>
        /// <returns></returns>
        public Rectangle GetHitBox() => hitBox;


        /// <summary>
        /// Initialize the hitbox
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="hitBoxWidth"></param>
        /// <param name="hitBoxHeight"></param>
        protected virtual void InitializeHitBox(GraphicsDevice graphicsDevice, int hitBoxWidth, int hitBoxHeight)
        {

            // initialize the hitbox
            this.hitBoxWidth = hitBoxWidth;
            this.hitBoxHeight = hitBoxHeight;
            hitBox = new Rectangle((int)(position.X) - hitBoxWidth / 2, (int)(position.Y) - hitBoxHeight, hitBoxWidth, hitBoxHeight);
            
            
            // DEBUG
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

        /// <summary>
        /// Check if the weapon is colliding with another
        /// </summary>
        /// <param name="otherRec"></param>
        /// <param name="otherAngle"></param>
        /// <returns></returns>
        public bool IsColliding(Rectangle otherRec, float otherAngle = 0)
        {
            return Util.RotatedCollision(hitBox, otherRec, angle, otherAngle);
        }

        /// <summary>
        /// Draw the weapon
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(img, position - offset, sourceRec, Color.White, angle, origin, 1, SpriteEffects.None, 0);

            spriteBatch.DrawString(Assets.debugFont, angle.ToString(), new Vector2(10, 70), Color.White);

            // draw the hitbox
            // DEBUG
            if (isDebug) degbugHitBox.Draw(spriteBatch, Color.Red, false);
        }

    }
}
