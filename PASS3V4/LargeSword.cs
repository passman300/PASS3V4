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

        private const int IMG_SRC_X = 64;
        private const int IMG_SRC_Y = 128;
        private const int IMG_SRC_WIDTH = 32;
        private const int IMG_SRC_HEIGHT = 64;

        // QUESTION: ask about maggic values
        public LargeSword(GraphicsDevice graphicsDevice, Vector2 centerPosition) :
            base(graphicsDevice, new Rectangle(IMG_SRC_X, IMG_SRC_Y, IMG_SRC_WIDTH, IMG_SRC_HEIGHT), centerPosition, new Vector2(0, -IMG_SRC_HEIGHT), new Vector2(IMG_SRC_WIDTH / 2, IMG_SRC_HEIGHT), BASE_ROTATION_SPEED)
        {
            Id = WeaponType.LargeSword;
            
            Damage = DAMAGE;
        }
    }
}
