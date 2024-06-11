//Author: Colin Wang
//File Name: FireBall.cs
//Project Name: PASS3 a dungeon crawler
//Created Date: June 8, 2024
//Modified Date: June 10, 2024
//Description: Mob projectile class for the game. Should inherit from Projectile class


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PASS3V4
{
    public class FireBall : Projectile
    {

        // image source variables of the fireball
        public const int IMG_SRC_X = 64;
        public const int IMG_SRC_Y = 384;
        public const int IMG_SRC_WIDTH = 48;
        public const int IMG_SRC_HEIGHT = 32;

        // base speed and damage of the fireball
        public const int BASE_SPEED = 7;
        public const int BASE_DAMAGE = 5;

        // image of the fireball
        private static Texture2D img = Assets.fireballImg;

        /// <summary>
        /// Constructs a new FireBall with specified speed, angle, center position and damage
        /// </summary>
        /// <param name="graphicsDevice">Graphics device to draw the FireBall to</param>
        /// <param name="speed">Speed of the FireBall when it is flying</param>
        /// <param name="angle">Angle of the FireBall when it is fired</param>
        /// <param name="centerPos">Center position of the FireBall when it is fired</param>
        /// <param name="damage">Damage the FireBall will cause when it hits a target</param>
        public FireBall(GraphicsDevice graphicsDevice, float speed, float angle, Vector2 centerPos, int damage = BASE_DAMAGE) :
            base(graphicsDevice, img, new Rectangle(0, 0, IMG_SRC_WIDTH, IMG_SRC_HEIGHT), speed, angle, centerPos, new Vector2(0, 0), new Vector2(IMG_SRC_WIDTH / 2, IMG_SRC_HEIGHT / 2), damage)
        {
            // Call the base constructor of the Projectile class and pass in the necessary parameters.
            // The base constructor initializes the projectile's properties such as image, source rectangle, speed, angle,
            // center position, offset, origin and damage.
        }
    }
}
