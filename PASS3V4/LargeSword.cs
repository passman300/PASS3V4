using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameUtility;
using System;
using Microsoft.Xna.Framework.Content;

namespace PASS3V4
{
    public class LargeSword : Weapon
    {
        private const int DAMAGE = 10;

        private const float BASE_ROTATION_SPEED = 0.1f;

        private const int IMAGE_SOURCE_X = 64;
        private const int IMAGE_SOURCE_Y = 128;
        private const int IMAGE_SOURCE_WIDTH = 32;
        private const int IMAGE_SOURCE_HEIGHT = 64;

        // QUESTION: ask about maggic values
        public LargeSword(GraphicsDevice graphicsDevice, Vector2 centerPosition) :
            base(graphicsDevice, new Rectangle(IMAGE_SOURCE_X, IMAGE_SOURCE_Y, IMAGE_SOURCE_WIDTH, IMAGE_SOURCE_HEIGHT), centerPosition, new Vector2(0, -IMAGE_SOURCE_HEIGHT), new Vector2(IMAGE_SOURCE_WIDTH / 2, IMAGE_SOURCE_HEIGHT), BASE_ROTATION_SPEED)
        {
            Id = WeaponType.LargeSword;
            
            Damage = DAMAGE;
        }
    }
}
