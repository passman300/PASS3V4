//Author: Colin Wang
//File Name: LargeSword.cs
//Project Name: PASS3 a dungeon crawler
//Created Date: May 21, 2024
//Modified Date: June 10, 2024
//Description: Largesword which the player swings. However the collision doesn't work

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PASS3V4
{
    public class LargeSword : Weapon
    {
        // The base damage of a large sword
        private const int DAMAGE = 10;

        // The base rotation speed of a large sword
        private const float BASE_ROTATION_SPEED = 0.1f;

        // The x-coordinate of the upper-left corner of the large sword image in the sprite sheet
        private const int IMG_SRC_X = 64;

        // The y-coordinate of the upper-left corner of the large sword image in the sprite sheet
        private const int IMG_SRC_Y = 128;

        // The width of the large sword image
        private const int IMG_SRC_WIDTH = 32;

        // The height of the large sword image
        private const int IMG_SRC_HEIGHT = 64;
        /// <summary>
        /// Initializes a new instance of the LargeSword class with the specified graphics device and center position.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device to draw the large sword to.</param>
        /// <param name="centerPosition">The center position of the large sword.</param>
        public LargeSword(GraphicsDevice graphicsDevice, Vector2 centerPosition)
            : base(graphicsDevice, new Rectangle(IMG_SRC_X, IMG_SRC_Y, IMG_SRC_WIDTH, IMG_SRC_HEIGHT), centerPosition, new Vector2(0, -IMG_SRC_HEIGHT), new Vector2(IMG_SRC_WIDTH / 2, IMG_SRC_HEIGHT), BASE_ROTATION_SPEED)
        {
            // Set the Id of the large sword to LargeSword
            Id = WeaponType.LargeSword;

            // Set the damage of the large sword to the base damage
            Damage = DAMAGE;
        }
    }
}
