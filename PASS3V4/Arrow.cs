//Author: Colin Wang
//File Name: Arrow.cs
//Project Name: PASS3 a dungeon crawler
//Created Date: May 24, 2024
//Modified Date: June 10, 2024
//Description: Arrow class which inherits from Projectile, and is used by the player to shoot mobs

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameUtility;
using System;


namespace PASS3V4
{
    /// <summary>
    /// Represents an Arrow, a weapon that can be shot and travel in a straight line
    /// </summary>
    public class Arrow : Projectile
    {
        /// <summary>
        /// Represents the different states the Arrow can be in
        /// </summary>
        public enum ArrowState
        {
            Charging, // Arrow is being charged and is not yet flying
            Flying // Arrow has been charged and is flying
        }

        // Source X coordinate of the Arrow in the Weapon Texture
        public const int IMG_SRC_X = 64;

        // Source Y coordinate of the Arrow in the Weapon Texture
        public const int IMG_SRC_Y = 384;


        // Source Width of the Arrow in the Weapon Texture
        public const int IMG_SRC_WIDTH = 48;

        // Source Height of the Arrow in the Weapon Texture
        public const int IMG_SRC_HEIGHT = 32;

        // Base Speed of the Arrow when it is flying
        public const int BASE_SPEED = 7;

        // Base Damage of the Arrow when it hits a target
        public const int BASE_DAMAGE = 5;

        // Texture of the Arrow
        private static Texture2D img = Util.Crop(Assets.weaponSetImg, new Rectangle(IMG_SRC_X, IMG_SRC_Y, IMG_SRC_WIDTH, IMG_SRC_HEIGHT));

        // Current state of the Arrow
        public ArrowState PlayerState { get; set; }

        /// <summary>
        /// Constructs a new Arrow with specified speed, angle, center position and damage
        /// </summary>
        /// <param name="graphicsDevice">Graphics device to draw the Arrow to</param>
        /// <param name="speed">Speed of the Arrow when it is flying</param>
        /// <param name="angle">Angle of the Arrow when it is fired</param>
        /// <param name="centerPos">Center position of the Arrow when it is fired</param>
        /// <param name="damage">Damage the Arrow will cause when it hits a target</param>
        public Arrow(GraphicsDevice graphicsDevice, float speed, float angle, Vector2 centerPos, int damage = BASE_DAMAGE) :
            base(graphicsDevice, img, new Rectangle(0, 0, IMG_SRC_WIDTH, IMG_SRC_HEIGHT), speed, angle, centerPos, new Vector2(0, 0), new Vector2(IMG_SRC_WIDTH / 2, IMG_SRC_HEIGHT / 2), damage)
        {
        }


        /// <summary>
        /// Updates the position, angle and velocity of the Arrow when it is charging
        /// </summary>
        /// <param name="centerPos">Center position of the Arrow when it is firing</param>
        /// <param name="angle">Angle of the Arrow when it is fired</param>
        /// <param name="percentCharged">Percentage the Arrow has been charged (1.0 means fully charged, 0.0 means not charged yet)</param>
        public void UpdateCharging(Vector2 centerPos, float angle, float percentCharged = 1f)
        {
            // update the position of the weapon 
            Position = centerPos + Offset; // set the position of the Arrow to the center position plus the offset

            // update the angle 
            Angle = angle; // set the angle of the Arrow to the specified angle

            // update the hitbox of the weapon
            // HitBox = new Rectangle((int)(position.X - HitBoxWidth / 2), (int)(position.Y - HitBoxHeight / 2), (int)HitBoxWidth, (int)HitBoxHeight); // create a new hitbox with the correct dimensions
            HitBox = new Rectangle((int)Position.X, (int)Position.Y, (int)HitBoxWidth, (int)HitBoxHeight); // create a new hitbox with the correct dimensions

            // update the velocity based on the angle
            Velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)); // create a new vector with the correct angle and magnitude

            Velocity.Normalize(); // normalize the vector
            Velocity *= Speed * percentCharged; // multiply the vector by the speed and the charge percentage

            // update the position of the hitbox
            if (isDebug) degbugHitBox.TranslateTo(HitBox.X, HitBox.Y); // update the position of the debug hitbox to the position of the regular hitbox
        }
    }
}
